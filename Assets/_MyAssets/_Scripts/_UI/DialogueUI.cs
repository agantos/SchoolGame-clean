using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DialogueUI : MonoBehaviour
{
    #region Variables
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

    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private Button nextButton;
    #endregion


    #region Events
    public event Action OnStepFinished_NodeMethods;

    public void ClearOnStepFinished()
    {
        OnStepFinished_NodeMethods = null;
    }
    #endregion

    #region Audio/Visual__Changes
    public void UpdateTextView(string textToType, string speakerName, string optionAText = null, string optionBText = null, AudioClip clip = null)
    {
        dialogueText.text = "";
        float duration = typingDuration * textToType.Length / 50;
        if (clip != null) duration = clip.length;

        SetSpeakerName(speakerName);

        dialogueText.DOText(textToType, duration, richTextEnabled: true)
            .OnComplete(() => {
                SetOptions(optionAText, optionBText);
                if (optionAText == null && optionBText == null)
                {
                    EnableNext();
                }

                OnStepFinished_NodeMethods?.Invoke();
            });

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

    public void PlayAudio(AudioClip audioClip)
    {
        if (audioClip == null) return;
        _audioSource.Stop();
        _audioSource.clip = audioClip;
        _audioSource.Play();
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
    #endregion

    #region Mono
    private void Start()
    {
        var _dialogueManager = FindAnyObjectByType<DialogueManager>();

        OptionA_button.onClick.AddListener(_dialogueManager.OnOptionAPicked);
        OptionB_button.onClick.AddListener(_dialogueManager.OnOptionBPicked);

        OptionA_button.gameObject.SetActive(false);
        OptionB_button.gameObject.SetActive(false);

        nextButton.onClick.AddListener(_dialogueManager.OnNextPressed);
        nextButton.onClick.AddListener(DisableNext);

        nextButton.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }
    #endregion

    #region Next
    public void EnableNext()
    {
        nextButton.gameObject.SetActive(true);
    }

    public void DisableNext()
    {
        nextButton.gameObject.SetActive(false);
    }
    #endregion
}
