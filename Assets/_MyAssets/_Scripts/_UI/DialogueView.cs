using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueView : MonoBehaviour
{
	[Header("View Objects")]

	[SerializeField] RawImage textBackground;
	[SerializeField] RawImage speakerArea;
	[SerializeField] TextMeshProUGUI dialogueText;


	[Header("Views")]

	[SerializeField] Texture2D normalBackgroundTexture;
	[SerializeField] Texture2D normalSpeakerAreaTexture;

	[SerializeField] Texture2D[] alternateBackgroundTextures;
	[SerializeField] Texture2D[] alternateSpeakerAreaTextures;

	int currentAlternateView = -1; // -1 is for normalView


	public void ToView(int index)
	{
		if(index == -1)
		{
			ToNormalView();
		}
		else
		{
			ToAlternateView(index);
		}
	}

	void ToNormalView()
	{
		if (currentAlternateView == -1) return;

		speakerArea.gameObject.SetActive(true);

		textBackground.texture = normalBackgroundTexture;
		speakerArea.texture = normalSpeakerAreaTexture;

		currentAlternateView = -1;
	}

	void ToAlternateView(int index = 0) {
		if (currentAlternateView == index || index >= alternateBackgroundTextures.Length) return;

		textBackground.texture = alternateBackgroundTextures[index];

		if(speakerArea.texture != null)
		{
			speakerArea.texture = alternateBackgroundTextures[index];
		}
		else
		{
			speakerArea.gameObject.SetActive(false);
		}

		currentAlternateView = index;
	}
}
