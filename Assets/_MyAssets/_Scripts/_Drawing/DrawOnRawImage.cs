using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TransparentOverlayDraw : MonoBehaviour
{
	[Header("UI References")]
	public RawImage overlayImage; // The transparent overlay you paint on

	[Header("Brush Settings")]
	public Color drawColor = Color.black;
	public int brushSize = 10;

	public enum DrawMode { Draw, Erase }
	public DrawMode currentMode = DrawMode.Draw;

	private Texture2D overlayTexture;
	private Vector2? _positionToDraw;
	private Vector2? _lastPositionToDraw;
	private Camera _camera;
	private Color[] _brushCache;
	private bool _initialized = false;

	public bool isEnabled = true;

	void Start()
	{
		_camera = Camera.main;

		if (overlayImage == null)
		{
			Debug.LogError("Overlay RawImage is not assigned!");
			return;
		}

		InitializeOverlay();
	}

	void InitializeOverlay()
	{
		// Try to use existing texture if it exists
		Texture2D existing = overlayImage.texture as Texture2D;

		if (existing != null)
		{
			overlayTexture = new Texture2D(existing.width, existing.height, TextureFormat.RGBA32, false);
			overlayTexture.SetPixels(existing.GetPixels());
			overlayTexture.Apply(false);
		}
		else
		{
			Rect rect = overlayImage.rectTransform.rect;
			int width = Mathf.Max(1, Mathf.RoundToInt(rect.width));
			int height = Mathf.Max(1, Mathf.RoundToInt(rect.height));

			overlayTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
			ClearDrawing();
		}

		overlayImage.texture = overlayTexture;
		GenerateBrushCache();
		_initialized = true;
	}

	void Update()
	{
		if (!_initialized) return;

		GatherDrawPosition();

		if (isEnabled)
		{
			if (_positionToDraw.HasValue)
			{
				DrawOnOverlay(_positionToDraw.Value);
			}
			else
			{
				_lastPositionToDraw = null; // reset when no touch
			}
		}

	}

	public void SetBrushColor(Color newColor)
	{
		drawColor = newColor;
	}

	private void GatherDrawPosition()
	{
		_positionToDraw = null;

		if (Touchscreen.current == null || EventSystem.current == null)
			return;

		foreach (var touch in Touchscreen.current.touches)
		{
			if (!touch.press.isPressed) continue;

			Vector2 pos = touch.position.ReadValue();
			var pointerData = new PointerEventData(EventSystem.current) { position = pos };
			var results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerData, results);

			foreach (var result in results)
			{
				if (result.gameObject == overlayImage.gameObject)
				{
					_positionToDraw = pos;
					return;
				}
			}
		}
	}

	private void DrawOnOverlay(Vector2 screenPosition)
	{
		if (!_initialized || overlayTexture == null || _brushCache == null) return;

		Vector2 texPos = ScreenToTextureCoords(screenPosition);

		// Clamp to texture bounds to prevent full-fill
		if (texPos.x < 0 || texPos.x >= overlayTexture.width || texPos.y < 0 || texPos.y >= overlayTexture.height)
			return;

		if (_lastPositionToDraw.HasValue)
		{
			Vector2 lastTexPos = ScreenToTextureCoords(_lastPositionToDraw.Value);
			DrawLine(lastTexPos, texPos);
		}
		else
		{
			DrawCircle(texPos);
		}

		_lastPositionToDraw = screenPosition;
		overlayTexture.Apply(false);
	}

	private void DrawLine(Vector2 start, Vector2 end)
	{
		int x0 = Mathf.RoundToInt(start.x);
		int y0 = Mathf.RoundToInt(start.y);
		int x1 = Mathf.RoundToInt(end.x);
		int y1 = Mathf.RoundToInt(end.y);

		int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
		int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
		int err = dx + dy;

		while (true)
		{
			DrawCircle(new Vector2(x0, y0));

			if (x0 == x1 && y0 == y1) break;
			int e2 = 2 * err;
			if (e2 >= dy) { err += dy; x0 += sx; }
			if (e2 <= dx) { err += dx; y0 += sy; }
		}
	}

	private void DrawCircle(Vector2 texPos)
	{
		int texX = Mathf.RoundToInt(texPos.x);
		int texY = Mathf.RoundToInt(texPos.y);
		int diameter = brushSize * 2 + 1;

		Color colorToDraw = currentMode == DrawMode.Draw ? drawColor : Color.clear;

		for (int i = 0; i < diameter; i++)
		{
			int px = texX + i - brushSize;
			if (px < 0 || px >= overlayTexture.width) continue;

			for (int j = 0; j < diameter; j++)
			{
				int py = texY + j - brushSize;
				if (py < 0 || py >= overlayTexture.height) continue;

				int index = j * diameter + i;
				if (_brushCache[index].a > 0)
					overlayTexture.SetPixel(px, py, colorToDraw);
			}
		}
	}

	private void GenerateBrushCache()
	{
		int diameter = brushSize * 2 + 1;
		_brushCache = new Color[diameter * diameter];

		for (int i = 0; i < diameter; i++)
		{
			for (int j = 0; j < diameter; j++)
			{
				int dx = i - brushSize;
				int dy = j - brushSize;
				_brushCache[j * diameter + i] = (dx * dx + dy * dy <= brushSize * brushSize) ? Color.white : Color.clear;
			}
		}
	}

	private Vector2 ScreenToTextureCoords(Vector2 screenPosition)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			overlayImage.rectTransform,
			screenPosition,
			_camera,
			out Vector2 localPoint
		);

		Rect rect = overlayImage.rectTransform.rect;
		float normX = Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x);
		float normY = Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y);

		return new Vector2(normX * overlayTexture.width, normY * overlayTexture.height);
	}

	public void ClearDrawing()
	{
		if (overlayTexture == null) return;

		Color[] clearColors = new Color[overlayTexture.width * overlayTexture.height];
		for (int i = 0; i < clearColors.Length; i++)
			clearColors[i] = Color.clear;

		overlayTexture.SetPixels(clearColors);
		overlayTexture.Apply(false);

		_lastPositionToDraw = null; // Reset last draw position
	}

	// Optional: erase at a specific position immediately
	public void EraseAt(Vector2 screenPosition)
	{
		currentMode = DrawMode.Erase;
		_positionToDraw = screenPosition;
		DrawOnOverlay(screenPosition);
		currentMode = DrawMode.Draw;
	}
}
