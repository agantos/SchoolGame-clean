using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TabletMinigameController_Story : MonoBehaviour
{
	[Header("Particles")]
	[SerializeField] ParticleEffect sparksEffect;
	[SerializeField] ParticleEffect confettiEffect;

	[Space]
	[Header("Audio")]
	[SerializeField] AudioSource audioSource;

	private void Start()
	{
		StartGame();
	}

	void StartGame()
	{
		InitializeColorButtons();
	}

	#region Drawing Canvas
	[SerializeField] TransparentOverlayDraw canvas;
	[SerializeField] List<ColorButton> colorButtons;
	ColorButton _selectedButton;

	[SerializeField] GameObject eraseButton;
	[SerializeField] Sprite eraseButtonSelected;
	[SerializeField] Sprite eraseButtonUnselected;

	void InitializeColorButtons()
	{
		foreach (var cb in colorButtons) { 
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

		if(_selectedButton != null) _selectedButton.Unselect();
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

	#endregion

	#region IMAGE VIEW ROUND 1	

	#endregion

}
