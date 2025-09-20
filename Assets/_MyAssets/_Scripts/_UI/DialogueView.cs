using System;
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

	[SerializeField] DialogueAlternateView[] alternateViews;


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
		
		dialogueText.fontStyle &= ~TMPro.FontStyles.Italic;

    }

    void ToAlternateView(int index = 0) {
		if (currentAlternateView == index || index >= alternateViews.Length) return;

		var alternateView = alternateViews[index];

		textBackground.texture = alternateView.textBackgroundTexture;

		// Speaker Area
		if (alternateView.speakerAreaTexture)
		{
			speakerArea.texture = alternateView.speakerAreaTexture;
		}
		else
		{
			speakerArea.gameObject.SetActive(false);
		}

		// Italic Text
		if (alternateView.italicDialogueText)
		{
		          dialogueText.fontStyle |= TMPro.FontStyles.Italic;
		      }
		else
		{
			dialogueText.fontStyle &= TMPro.FontStyles.Italic;
		}
		
		currentAlternateView = index;
	}

	[Serializable]
	public struct DialogueAlternateView
	{
		public bool italicDialogueText;
        public Texture2D speakerAreaTexture;
        public Texture2D textBackgroundTexture;
    }
}
