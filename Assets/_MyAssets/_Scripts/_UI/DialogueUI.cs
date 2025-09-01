using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class DialogueUI : MonoBehaviour
{
    [Header("Dialogue Text")]

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float typingDuration = 1f;

    [Header("Speaker")]

    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;

    [Header("Options")]

    [SerializeField] private TextMeshProUGUI OptionA;
    [SerializeField] private TextMeshProUGUI OptionB;

    [SerializeField] private Button OptionA_button;
    [SerializeField] private Button OptionB_button;


    private void Start()
    {
        var _dialogueManager = FindAnyObjectByType<DialogueManager>();
        OptionA_button.onClick.AddListener(_dialogueManager.OnOptionAPicked);
        OptionB_button.onClick.AddListener(_dialogueManager.OnOptionBPicked);

    }

    public void UpdateTextView(string textToType, string speakerName, string optionAText = null, string optionBText = null )
    {
        dialogueText.text = "";
        float duration = typingDuration * textToType.Length / 236;

        SetSpeakerName(speakerName);

        dialogueText.DOText(textToType, duration, richTextEnabled: true)
            .OnComplete(() => SetOptions(optionAText, optionBText));

    }

    public void SetDialogueText(string textToType, string optionAText = null, string optionBText = null)
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
