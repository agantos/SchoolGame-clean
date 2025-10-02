using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
	[SerializeField] TransparentOverlayDraw drawCanvas;
	[SerializeField] Sprite unselectedView;
	[SerializeField] Sprite selectedView;
	public Color drawColor = Color.black;

	Button _button;
	Image _image;

	public Func<UniTask> onSelectCallback;
	public Func<UniTask> onUnelectCallback;

	private void Awake()
	{
		_image = GetComponent<Image>();
		_button = GetComponent<Button>();
	}

	public void Select()
	{
		drawCanvas.drawColor = drawColor;
		_image.sprite = selectedView;
	
		onSelectCallback?.Invoke();
	}

	public void Unselect()
	{
		_image.sprite = unselectedView;

		onUnelectCallback?.Invoke();
	}
}
