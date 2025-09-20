using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DialogueUI : MonoBehaviour
{
	#region Variables
	[Header("Dialogue View")]
    [SerializeField] DialogueView dialogueView;

	[Header("Dialogue Text")]

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float typingDuration = 1f;
	private Tween _activeTextTween;

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

    DialogueManager _dialogueManager;
    #endregion

    #region Events
    public Func<UniTask> OnStepFinished_NodeMethods;

    public void ClearOnStepFinished()
    {
        OnStepFinished_NodeMethods = null;
    }
    #endregion

    public void SetView(int index)
    {
        if(dialogueView != null)
        {
            dialogueView.ToView(index);
        }
    }

    #region Audio/Visual__Changes
    public void UpdateTextView(string textToType, string speakerName, string optionAText = null, string optionBText = null, AudioClip clip = null)
    {
        EnableNext();

        dialogueText.text = "";
        float duration = typingDuration * textToType.Length / 50;
        if (clip != null) duration = clip.length;

        SetSpeakerName(speakerName);

        _activeTextTween = dialogueText.DOText(textToType, duration, richTextEnabled: true)
            .OnComplete(async () => {                
                bool hasOptions = optionAText != null || optionBText != null;
                if (hasOptions) DisableNext();

				_audioSource.Stop();
                _audioSource.clip = null;
				_activeTextTween = null;

                if (OnStepFinished_NodeMethods != null)
                {
                    await OnStepFinished_NodeMethods.Invoke();
                }

                SetOptions(optionAText, optionBText);
            });

    }

	public void ResetView()
    {
		GameObject optionAGameObject = OptionA.transform.parent.gameObject;
		GameObject optionBGameObject = OptionB.transform.parent.gameObject;

		optionAGameObject.SetActive(false);
		optionBGameObject.SetActive(false);
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
		_dialogueManager = FindAnyObjectByType<DialogueManager>();

        OptionA_button.onClick.AddListener(_dialogueManager.OnOptionAPicked);
        OptionB_button.onClick.AddListener(_dialogueManager.OnOptionBPicked);

        OptionA_button.gameObject.SetActive(false);
        OptionB_button.gameObject.SetActive(false);

        nextButton.onClick.AddListener(OnNext);

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

	public bool DialogHasCompleted()
	{
		return _activeTextTween == null || !_activeTextTween.IsPlaying();
	}

	public void CompleteTextTween()
	{
		if (_activeTextTween != null && _activeTextTween.IsActive() && _activeTextTween.IsPlaying())
		{
			_activeTextTween.Complete();
		}
	}

	void OnNext()
	{
		if (_activeTextTween != null && _activeTextTween.IsActive() && _activeTextTween.IsPlaying())
		{
			CompleteTextTween();
		}
		else
		{
			if (_dialogueManager != null)
			{
				_dialogueManager.OnNextPressed();
			}
		}
	}

	#endregion
}
