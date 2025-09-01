using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI speakerNameText;

    [SerializeField] private float typingDuration = 1f;

    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;

    [SerializeField] private TextMeshProUGUI OptionA;
    [SerializeField] private TextMeshProUGUI OptionB;


    public void SetText(string textToType, string optionAText = null, string optionBText = null)
    {
        dialogueText.text = "";
        float duration = typingDuration * textToType.Length / 236;

        dialogueText.DOText(textToType, duration, richTextEnabled: true)
            .OnComplete(() => SetOptions(optionAText, optionBText));
    }

    public void SetOptions(string optionAText = null, string optionBText = null)
    {
        GameObject optionAGameObject = OptionA.transform.parent.gameObject;
        GameObject optionBGameObject = OptionB.transform.parent.gameObject;

        // Hide by default
        optionAGameObject.SetActive(false);
        optionBGameObject.SetActive(false);

        if (!string.IsNullOrEmpty(optionAText))
        {
            optionAGameObject.SetActive(true);
            OptionA.text = optionAText;
        }

        if (!string.IsNullOrEmpty(optionBText))
        {
            optionBGameObject.SetActive(true);
            OptionB.text = optionBText;
        }
    }

    public void SetSpeakerName(string speaker)
    {
        speakerNameText.text = speaker;
    }


    public void SetImages(Sprite leftSprite = null, Sprite rightSprite = null)
    {
        if (leftImage != null)
        {
            if (leftSprite == null)
            {
                leftImage.sprite = null;
                leftImage.enabled = false;
            }
            else
            {
                leftImage.sprite = leftSprite;
                leftImage.enabled = true;
            }
        }

        if (rightImage != null)
        {
            if (rightSprite == null)
            {
                rightImage.sprite = null;
                rightImage.enabled = false;
            }
            else
            {
                rightImage.sprite = rightSprite;
                rightImage.enabled = true;
            }
        }
    }
}
