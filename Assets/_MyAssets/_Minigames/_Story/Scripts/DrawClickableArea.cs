using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DrawClickableArea : MonoBehaviour
{	
	public DrawCanvas DrawCanvas;
	public RawImage DrawingDisplay;
	public string CanvasText;

	CanvasGroup _drawingDisplayCanvasGroup;

	Texture2D _imageDrawn;
	Button _button;

	public bool IsDrawn;

	public Action onFinish;

	private void Awake()
	{
		_button = GetComponent<Button>();
		_drawingDisplayCanvasGroup = DrawingDisplay.gameObject.GetComponent<CanvasGroup>();
		_drawingDisplayCanvasGroup.alpha = 0f;

		_button.onClick.AddListener(StartDrawing);
		StartHighlight();
	}

	public Texture2D GetDrawnImage {  get { return _imageDrawn; } }

	public void Disable()
	{
		HighlightImage.gameObject.SetActive(false);
		_button.enabled = false;
	}

	public void Enable()
	{
		HighlightImage.gameObject.SetActive(true);
		_button.enabled = true;
	}

	public void StartDrawing()
	{
		DrawCanvas.gameObject.SetActive(true);		
		DrawCanvas.StartSession(CanvasText, DrawingDisplay.texture as Texture2D);	
		DrawCanvas.FinishButton.onClick.AddListener(OnFinishCallback);
	}

	public void EndDrawing()
	{
		IsDrawn = true;

		if (_highlightLoop)
			StopHighlight();
		
		_imageDrawn = DrawCanvas.canvas.GetOverlayTextureCopy();
		_drawingDisplayCanvasGroup.alpha = 1f;
		DrawingDisplay.texture = _imageDrawn;

		DrawCanvas.gameObject.SetActive(false);
		HighlightImage.gameObject.SetActive(false);

		DrawCanvas.FinishButton.onClick.RemoveListener(OnFinishCallback);
	}

	void OnFinishCallback()
	{
		EndDrawing();
		if(onFinish!=null) onFinish.Invoke();
	}

	#region Highlight
	public RawImage HighlightImage;
	bool _highlightLoop = true;

	public void StartHighlight()
	{
		HighlightLoop().Forget();
	}

	private async UniTaskVoid HighlightLoop()
	{
		var highlight = HighlightImage.GetComponent<CanvasGroup>();
		if (highlight == null)
			highlight = highlight.gameObject.AddComponent<CanvasGroup>();

		while (_highlightLoop && this != null && highlight != null)
		{
			await highlight
				.DOFade(1f, 1f)
				.AsyncWaitForCompletion();

			await highlight
				.DOFade(0.5f, 1f)
				.AsyncWaitForCompletion();
		}

		// Reset alpha if loop ends
		highlight.alpha = 1f;
	}

	public void StopHighlight()
	{
		_highlightLoop = false;
		DOTween.Kill(HighlightImage); // stop any active tween safely
	}
	#endregion


}
