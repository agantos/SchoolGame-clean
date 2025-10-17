using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DrawCanvas : MonoBehaviour
{

	public TransparentOverlayDraw canvas;

	[Space]
	[Header("Buttons")]
	[SerializeField] List<ColorButton> colorButtons;
	public Button FinishButton;
	ColorButton _selectedButton;

	[SerializeField] GameObject eraseButton;
	[SerializeField] Sprite eraseButtonSelected;
	[SerializeField] Sprite eraseButtonUnselected;

	[Space]
	[Header("Audio")]
	[SerializeField] AudioSource audioSource;

	public TextMeshProUGUI ui_text;
	public void StartSession(string text = "", Texture2D initialTexture = null)
	{
		canvas.ClearDrawing();
		canvas.InitializeOverlay(initialTexture);
		if (initialTexture == null) canvas.ClearDrawing();		

		ui_text.text = text;
	}

	public void Initialize(string text = "")
	{
		ui_text = GetComponentInChildren<TextMeshProUGUI>();

		// Init Color Buttons
		foreach (var cb in colorButtons)
		{
			Button button = cb.GetComponent<Button>();

			button.onClick.AddListener(() =>
			{
				onClickColorButton(cb);
			});
		}

		_selectedButton = colorButtons.First();
		_selectedButton.Select();

		eraseButton.GetComponent<Button>().onClick.AddListener(onClickEraseButton);
	}

	void onClickColorButton(ColorButton button)
	{
		canvas.currentMode = TransparentOverlayDraw.DrawMode.Draw;

		if (_selectedButton != null) _selectedButton.Unselect();
		_selectedButton = button;
		_selectedButton.Select();

		//Handle erase button
		var eraseImage = eraseButton.GetComponent<Image>();
		eraseImage.sprite = eraseButtonUnselected;
	}

	public void onClickEraseButton()
	{
		if (_selectedButton != null) _selectedButton.Unselect();
		_selectedButton = null;

		canvas.currentMode = TransparentOverlayDraw.DrawMode.Erase;

		//visuals
		var eraseImage = eraseButton.GetComponent<Image>();
		eraseImage.sprite = eraseButtonSelected;
	}
}
