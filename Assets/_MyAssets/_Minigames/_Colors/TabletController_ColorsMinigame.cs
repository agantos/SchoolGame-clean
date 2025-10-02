using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TabletController_Colorsinigame : MonoBehaviour
{
	[Header("Particles")]
	[SerializeField] ParticleEffect sparksEffect;
	[SerializeField] ParticleEffect confettiEffect;
	
	[Space]
	[Header("Audio")]
	[SerializeField] AudioSource audioSource;

	[SerializeField] AudioClip[] correctSounds;
	[SerializeField] AudioClip[] wrongSounds;

	[Space]
	[Header("Overlay")]
	[SerializeField] CanvasGroup wrongOverlay;
	[SerializeField] CanvasGroup correctOverlay;

	private void Start()
	{
		wrongOverlay.alpha = 0f;
		correctOverlay.alpha = 0f;

		StartGame();
		//InitializeRound2();
	}

	void StartGame()
	{
		foreach(ChoiceButton choice in choices)
		{
			choice.Initialize(false, async () => { onClickChoice(choice); });
		}

		PlayIntroductionVideo();
	}

	#region Video View
	[Header("Video View")]
	[SerializeField] GameObject VideoView;
	[SerializeField] VideoPlayer mainVideoPlayer;
	[SerializeField] VideoClip introductionVideo;


	void EnableVideoView()
	{
		ImageView_Round1.SetActive(false);
		ImageView_Round2.SetActive(false);

		VideoView.SetActive(true);
	}

	async void PlayIntroductionVideo()
	{
		var button = VideoView.GetComponent<Button>();
		button.enabled = false;

		var canvasGroup = mainVideoPlayer.GetComponent<CanvasGroup>();

		EnableVideoView();
		mainVideoPlayer.clip = introductionVideo;
		canvasGroup.alpha = 0;
		mainVideoPlayer.Play();
		mainVideoPlayer.Pause();
		mainVideoPlayer.prepareCompleted += async (mainVideoPlayer) =>
		{
			await UniTask.WaitForEndOfFrame();

			await canvasGroup.DOFade(1, 2).AsyncWaitForCompletion();
			mainVideoPlayer.Play();

			// Subscribe to event when video finishes
			mainVideoPlayer.loopPointReached += OnIntroVideoEnd;
		};

	}

	async void OnIntroVideoEnd(VideoPlayer vp)
	{
		CanvasGroup videoCanvasGroup = vp.GetComponent<CanvasGroup>();
		CanvasGroup backgroundCanvasGroup = backgroundImage_round1.GetComponent<CanvasGroup>();

		backgroundCanvasGroup.alpha = 0;
		backgroundImage_round1.texture = startImage_Round1;

		await videoCanvasGroup.DOFade(0f, 1f).AsyncWaitForCompletion();

		EnableImageView_Round1();

		await backgroundCanvasGroup.DOFade(1f, 1f).AsyncWaitForCompletion();

		var imageButton = backgroundImage_round1.GetComponent<Button>();
		imageButton.enabled = true;

		imageButton.onClick.AddListener(async () =>
		{
			imageButton.enabled = false;
			imageButton.onClick.RemoveAllListeners();
			await backgroundCanvasGroup.DOFade(0f, 1f).AsyncWaitForCompletion();
			LoadFindColorLevel(0);
		});



	}
	#endregion

	#region IMAGE VIEW ROUND 1
	[Space]
	[Space]
	[Header("Round 1 - Image View")]
	[Space]
	[SerializeField] GameObject ImageView_Round1;
	[SerializeField] Texture2D startImage_Round1;
	public RawImage backgroundImage_round1;

	[Space]
	[Header("Levels")]
	public ChoiceButton[] choices;
	public FindColorLevel[] levels;

	int _currentLevel = 0;



	void EnableImageView_Round1()
	{
		ImageView_Round1.SetActive(true);
		ImageView_Round2.SetActive(false);

		VideoView.SetActive(false);
	}

	async void LoadNextLevel()
	{
		var backgroundCanvas = backgroundImage_round1.GetComponent<CanvasGroup>();

		if (_currentLevel < levels.Length - 1)
		{
			_currentLevel++;
			await backgroundCanvas.DOFade(0, 0.7f).AsyncWaitForCompletion();
			LoadFindColorLevel(_currentLevel);
		}
		// Load round 2
		else
		{
			var round2CanvasGroup = backgroundImage_round2.GetComponent<CanvasGroup>();

			//fade round1 image
			confettiEffect.Play();
			await UniTask.Delay(1000);
			await backgroundCanvas.DOFade(0, 1f).AsyncWaitForCompletion();

			//change image views
			round2CanvasGroup.alpha = 0f;
			backgroundImage_round2.texture = startImage_Round2;
			await UniTask.NextFrame();
			EnableImageView_Round2();

			// fade in round 2 starting image
			await round2CanvasGroup.DOFade(1, 1f).AsyncWaitForCompletion();
			confettiEffect.Play();
			await UniTask.Delay(1000);

			//fade transition round 2 game image
			await round2CanvasGroup.DOFade(0, 1f).AsyncWaitForCompletion();
			backgroundImage_round2.texture = mainImage_Round2;
			await round2CanvasGroup.DOFade(1, 1f).AsyncWaitForCompletion();

			InitializeGameRound2();
		}
	}

	async void onClickChoice(ChoiceButton choiceButton)
	{
		if (choiceButton.isCorrect)
		{
			//play sound

			//play effect
			sparksEffect.Play();
			await correctOverlay.DOFade(0.75f, 0.2f).AsyncWaitForCompletion();
			await correctOverlay.DOFade(0f, 0.2f).AsyncWaitForCompletion();

			var currentLevel = levels[_currentLevel];


			if (!currentLevel.correctChoicesSelected.Contains(choiceButton))
			{
				currentLevel.correctChoicesSelected.Add(choiceButton);
			}

			// go to next level
			if (currentLevel.correctChoicesSelected.Count == currentLevel.correctChoices.Length)
			{
				LoadNextLevel();
			}
		}
		else
		{
			// play sound

			// play effects
			await wrongOverlay.DOFade(0.75f, 0.2f).AsyncWaitForCompletion();
			await wrongOverlay.DOFade(0f, 0.2f).AsyncWaitForCompletion();
		}
	}

	void LoadFindColorLevel(int index)
	{
		var level = levels[index];
		var canvasGroup = backgroundImage_round1.GetComponent<CanvasGroup>();
		backgroundImage_round1.texture = level.background;

		canvasGroup.DOFade(1, 0.7f);

		foreach (var choice in choices)
		{
			choice.Initialize(false, null);

			foreach (var correctChoice in level.correctChoices)
			{
				if (choice == correctChoice)
				{
					choice.Initialize(true, null);
					break;
				}
			}

		}
	}

	[Serializable]
	public class FindColorLevel
	{
		public ChoiceButton[] correctChoices;
		public List<ChoiceButton> correctChoicesSelected;
		public Texture2D background;
	}
	#endregion

	#region IMAGE VIEW ROUND 2
	[Space]
	[Space]
	[Header("Round 2 - Image View")]
	[Space]
	[Header("Image Components")]
	[SerializeField] GameObject ImageView_Round2;
	[SerializeField] Texture2D startImage_Round2;
	[SerializeField] Texture2D endImage;
	[SerializeField] Texture2D mainImage_Round2;
	[SerializeField] RawImage backgroundImage_round2;

	[Header("Draw Components")]

	[SerializeField] TransparentOverlayDraw drawCanvas;
	[SerializeField] List<ColorZoneCheck> colorsCheck;
	[SerializeField] List<ColorZoneSet> colorsSet;

	int _correctAnswers;

	void EnableImageView_Round2()
	{
		ImageView_Round1.SetActive(false);
		ImageView_Round2.SetActive(true);

		VideoView.SetActive(false);
	}

	async void InitializeGameRound2()
	{
		drawCanvas.enabled = true;
		drawCanvas.ClearDrawing();
		_correctAnswers = 0;

		foreach (var colorSet in colorsSet) {
			colorSet._enabled = true;
			colorSet.onEnter = () =>
			{
				OnColorEnter(colorSet);
			};
		}

		foreach(var colorZoneCheck in colorsCheck)
		{
			colorZoneCheck.onWrongMatch = OnWrongMatch;
			colorZoneCheck.onCorrectMatch = OnCorrectMatch;
		}

	}

	async void ResetRound2()
	{
		drawCanvas.enabled = false;

		drawCanvas.ClearDrawing();
		_correctAnswers = 0;

		foreach (var colorSet in colorsSet)
		{
			colorSet._enabled = true;
		}

		await UniTask.Delay(1000);
		drawCanvas.enabled = true;

	}

	async UniTask OnWrongMatch()
	{
		ResetRound2();

		// play effects
		await wrongOverlay.DOFade(0.75f, 0.2f).AsyncWaitForCompletion();
		await wrongOverlay.DOFade(0f, 0.2f).AsyncWaitForCompletion();

	}

	async UniTask OnCorrectMatch()
	{
		drawCanvas.enabled = false;


		foreach (var colorSet in colorsSet)
		{
			colorSet._enabled = true;
		}

		_correctAnswers++;

		if(_correctAnswers == colorsCheck.Count)
		{
			PlayEndSequence();
		}
		else
		{
			sparksEffect.Play();
			await correctOverlay.DOFade(0.75f, 0.2f).AsyncWaitForCompletion();
			await correctOverlay.DOFade(0f, 0.2f).AsyncWaitForCompletion();

			drawCanvas.enabled = true;
		}
	}

	async void PlayEndSequence()
	{
		drawCanvas.enabled = false;

		var canvasGroup = backgroundImage_round2.GetComponent<CanvasGroup>();

		confettiEffect.Play();
		await UniTask.Delay(1400);
		confettiEffect.Play();
		await UniTask.Delay(1400);
		confettiEffect.Play();

		drawCanvas.ClearDrawing();

		await canvasGroup.DOFade(0, 1f).AsyncWaitForCompletion();
		backgroundImage_round2.texture = endImage;
		await canvasGroup.DOFade(1, 1f).AsyncWaitForCompletion();

		await UniTask.Delay(500);
		confettiEffect.Play();
		await UniTask.Delay(1400);
		confettiEffect.Play();
	}

	void OnColorEnter(ColorZoneSet currentColor)
	{
		foreach (var colorSet in colorsSet)
		{
			if(colorSet != currentColor)
			colorSet._enabled = false;
		}
	}

	#endregion

}
