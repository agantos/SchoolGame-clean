using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DrawOnRawImage : MonoBehaviour
{
	public RawImage rawImage;
	public Color drawColor = Color.black;
	public int brushSize = 10;

	private Texture2D texture;
	private Texture2D _baseTexture;

	private Vector2? _positionToDraw;
	private Vector2? _lastPositionToDraw; // track previous touch position
	private Camera _camera;

	void Start()
	{
		_camera = Camera.main;

		_baseTexture = rawImage.texture as Texture2D;

		// Clone texture to make it writable
		RenderTexture rt = RenderTexture.GetTemporary(_baseTexture.width, _baseTexture.height);
		Graphics.Blit(_baseTexture, rt);

		RenderTexture.active = rt;
		texture = new Texture2D(_baseTexture.width, _baseTexture.height, TextureFormat.RGBA32, false);
		texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
		texture.Apply();

		RenderTexture.active = null;
		RenderTexture.ReleaseTemporary(rt);

		rawImage.texture = texture;
	}

	void Update()
	{
		GatherDrawPosition();

		if (_positionToDraw.HasValue)
		{
			DrawOnTexture(_positionToDraw.Value);
		}
		else
		{
			_lastPositionToDraw = null;
		}
	}

	private void GatherDrawPosition()
	{
		_positionToDraw = null;

		if (Touchscreen.current == null || EventSystem.current == null)
			return;

		foreach (var touch in Touchscreen.current.touches)
		{
			if (!touch.press.isPressed)
				continue;

			Vector2 pos = touch.position.ReadValue();

			// Raycast UI
			var pointerData = new PointerEventData(EventSystem.current) { position = pos };
			var results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerData, results);

			foreach (var result in results)
			{
				if (result.gameObject == rawImage.gameObject)
				{
					_positionToDraw = pos;
					return;
				}
			}
		}
	}

	private void DrawOnTexture(Vector2 screenPosition)
	{
		Vector2 texPos = ScreenToTextureCoords(screenPosition);

		if (_lastPositionToDraw.HasValue)
		{
			Vector2 lastTexPos = ScreenToTextureCoords(_lastPositionToDraw.Value);
			DrawLineOnTexture(lastTexPos, texPos);
		}
		else
		{
			DrawCircleOnTexture(texPos);
		}

		_lastPositionToDraw = screenPosition;
		texture.Apply();
	}

	private void DrawLineOnTexture(Vector2 start, Vector2 end)
	{
		float distance = Vector2.Distance(start, end);
		int steps = Mathf.CeilToInt(distance);

		for (int i = 0; i <= steps; i++)
		{
			Vector2 point = Vector2.Lerp(start, end, i / distance);
			DrawCircleOnTexture(point);
		}
	}

	private void DrawCircleOnTexture(Vector2 texPos)
	{
		int texX = Mathf.RoundToInt(texPos.x);
		int texY = Mathf.RoundToInt(texPos.y);

		for (int i = -brushSize; i < brushSize; i++)
		{
			for (int j = -brushSize; j < brushSize; j++)
			{
				int px = texX + i;
				int py = texY + j;
				if (px >= 0 && px < texture.width && py >= 0 && py < texture.height)
				{
					if (i * i + j * j <= brushSize * brushSize)
						texture.SetPixel(px, py, drawColor);
				}
			}
		}
	}

	private Vector2 ScreenToTextureCoords(Vector2 screenPosition)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			rawImage.rectTransform,
			screenPosition,
			_camera,
			out Vector2 localPoint
		);

		Rect rect = rawImage.rectTransform.rect;
		float normX = Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x);
		float normY = Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y);

		return new Vector2(normX * texture.width, normY * texture.height);
	}

	public void ClearTexture()
	{
		Color[] clearColors = new Color[texture.width * texture.height];
		for (int i = 0; i < clearColors.Length; i++) clearColors[i] = Color.white;
		texture.SetPixels(clearColors);
		texture.Apply();
	}
}
