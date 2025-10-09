using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TabletMinigameController_Story : MonoBehaviour
{
	bool _isOnRound1 = true;

	[Header("Particles")]
	[SerializeField] ParticleEffect sparksEffect;
	[SerializeField] ParticleEffect confettiEffect;

	[Space]
	[Header("Audio")]
	[SerializeField] AudioSource audioSource;
	[SerializeField] DrawCanvas drawCanvas;

	private void Start()
	{
		StartGame();
	}

	void StartGame()
	{
		drawCanvas.gameObject.SetActive(true);
		drawCanvas.Initialize();
		drawCanvas.gameObject.SetActive(false);

		InitalizeDrawAreas_Round1();

		//PlayIntroductionVideo();

		//Test
		EnableImageView_Round1();
		LoadLevel_RememberStory(0);


	}

	#region Draw Canvas Methods

	public void OnFinishRound_1()
	{
		EnableImageView_Round1();
		LoadNextLevel_Round1();
	}

	void EnableDrawCanvas(string canvasText = "")
	{
		ImageView_Round1.SetActive(false);
		VideoView.SetActive(false);
		ImageView_Round2.SetActive(false);

		drawCanvas.gameObject.SetActive(true);
		drawCanvas.StartSession(canvasText);		
	}

	public void OnFinishRound_2()
	{
		Texture2D textureDrawn = drawCanvas.canvas.GetOverlayTextureCopy();

		EnableImageView_Round2();
		LoadNext_Round2();
	}
	#endregion

	int GetCompletedNumber(DrawClickableArea[] areas)
	{
		int i = 0;
		foreach (DrawClickableArea area in areas)
		{
			if (area.IsDrawn)
				i++;
		}

		return i;
	}

	#region Video View
	[Header("Video View")]
	[SerializeField] GameObject VideoView;
	[SerializeField] VideoPlayer mainVideoPlayer;
	[SerializeField] VideoClip introductionVideo;


	void EnableVideoView()
	{
		ImageView_Round1.SetActive(false);
		//ImageView_Round2.SetActive(false);

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

		await videoCanvasGroup.DOFade(0f, 1f).AsyncWaitForCompletion();
		EnableImageView_Round1();
		LoadLevel_RememberStory(0);
		await backgroundCanvasGroup.DOFade(1f, 1f).AsyncWaitForCompletion();
			
		PlayVideoButton.onClick.AddListener(PlayAssistVideo);
		PlayAudioButton.onClick.AddListener(PlayAssistAudio);
		vp.loopPointReached -= OnIntroVideoEnd;

	}
	#endregion

	#region ROUND 1
	[Space]
	[Space]
	[Header("Round 1 - Image View")]
	[Space]
	[SerializeField] GameObject ImageView_Round1;
	[SerializeField] RawImage backgroundImage_round1;
	[SerializeField] Level_RememberStory[] levels_round1;

	[Header("Round 1 - Buttons")]
	public Button PlayVideoButton;
	public Button PlayAudioButton;
	public AudioClip assistAudioClip;

	[Header("Round 1 - Final View Assets")]
	[SerializeField] Texture2D finalImage_round1;
	public Button nextRoundButton;

	int _currentLevel_round1;
	List<DrawClickableArea> drawAreas_round1;

	void InitalizeDrawAreas_Round1()
	{
		drawAreas_round1 = new List<DrawClickableArea>();

		foreach(var level in levels_round1)
		{
			drawAreas_round1.Add(level.drawArea);
			level.drawArea.Disable();
			level.drawArea.onFinish = OnFinishRound_1;
		}
	}

	void EnableImageView_Round1()
	{
		ImageView_Round1.SetActive(true);

		drawCanvas.gameObject.SetActive(false);
		ImageView_Round2.SetActive(false);
		VideoView.SetActive(false);
	}

	#region Assist Buttons
	void PlayAssistAudio()
	{
		audioSource.Stop();
		audioSource.clip = assistAudioClip;
		audioSource.Play();
	}

	async void PlayAssistVideo()
	{
		PlayVideoButton.enabled = false;
		PlayAudioButton.enabled = false;

		var button = VideoView.GetComponent<Button>();
		button.enabled = true;

		var canvasGroup = mainVideoPlayer.GetComponent<CanvasGroup>();
		await backgroundImage_round1.GetComponent<CanvasGroup>().DOFade(0f, 1f).AsyncWaitForCompletion();

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
			button.onClick.AddListener(EndVideo);
		};
	}

	async void OnAssistVideoEnd(VideoPlayer vp)
	{
		CanvasGroup videoCanvasGroup = vp.GetComponent<CanvasGroup>();
		CanvasGroup backgroundCanvasGroup = backgroundImage_round1.GetComponent<CanvasGroup>();

		backgroundCanvasGroup.alpha = 0;

		await videoCanvasGroup.DOFade(0f, 1f).AsyncWaitForCompletion();
		EnableImageView_Round1();
		LoadLevel_RememberStory(_currentLevel_round1);

		await backgroundCanvasGroup.DOFade(1f, 1f).AsyncWaitForCompletion();

		PlayVideoButton.enabled = true;
		PlayAudioButton.enabled = true;

		vp.loopPointReached -= OnAssistVideoEnd;
	}

	void EndVideo()
	{
		var button = VideoView.GetComponent<Button>();

		mainVideoPlayer.Stop();
		OnAssistVideoEnd(mainVideoPlayer);
		button.onClick.RemoveListener(EndVideo);
	}
	#endregion

	#region Levels Navigation
	async void LoadNextLevel_Round1()
	{
		_currentLevel_round1 = GetCompletedNumber(drawAreas_round1.ToArray());

		if (_currentLevel_round1 < levels_round1.Length)
		{
			LoadLevel_RememberStory(_currentLevel_round1);
		}
		else
		{
			backgroundImage_round1.texture = finalImage_round1;
			nextRoundButton.gameObject.SetActive(true);
			nextRoundButton.onClick.AddListener(ToRound2);
		}
	}

	void LoadLevel_RememberStory(int index)
	{
		nextRoundButton.gameObject.SetActive(false);

		// Apply textures
		var level = levels_round1[index];

		backgroundImage_round1.texture = level.backgroundImage;
		level.drawArea.Enable();
	}

	async void ToRound2()
	{
		var backgroundCanvas_1 = backgroundImage_round1.GetComponent<CanvasGroup>();
		var backgroundCanvas_2 = backgroundImage_round2.GetComponent<CanvasGroup>();
		backgroundCanvas_2.alpha = 0f;

		// Fade round_1
		await backgroundCanvas_1.DOFade(0f, 1f).AsyncWaitForCompletion();

		// Start round_2
		EnableImageView_Round2();
		LoadLevel_Round2(0);
		await backgroundCanvas_2.DOFade(1f, 1f).AsyncWaitForCompletion();


	}
	#endregion
	

	[Serializable]
	public class Level_RememberStory
	{
		public Texture2D backgroundImage;
		public DrawClickableArea drawArea;
	}

	#endregion
	
	#region ROUND 2
	[Space]
	[Space]
	[Header("Round 2 - View")]
	[Space]
	[SerializeField] GameObject ImageView_Round2;
	[SerializeField] RawImage backgroundImage_round2;

	[SerializeField] Texture2D finalImage_round2;
	[SerializeField] Texture2D initialImage_round2;

	[Header("Round 2 - Levels")]
	[SerializeField] Level_Round2[] levels_round2;

	int _currentLevel_round2 = 0;
	List<Texture2D> _imagesDrawn_round2 = new List<Texture2D>();

	void LoadLevel_Round2(int index)
	{
		var level = levels_round2[index];
		level.Level.SetActive(true);
		level.InitializeLevel();

		backgroundImage_round2.texture = level.backgroundImage;
	}

	async void LoadNext_Round2()
	{
		var currentLevel = levels_round2[_currentLevel_round2];
		
		// change level
		if (currentLevel.IsCompleted())
		{
			if (_currentLevel_round2 < levels_round2.Length - 1)
			{
				currentLevel.nextButton.gameObject.SetActive(true);
			}
			else
			{
				currentLevel.Level.SetActive(false);
				backgroundImage_round2.texture = finalImage_round2;
			}
		}
	}

	async void OnNextClicked_round2()
	{
		var currentLevel = levels_round2[_currentLevel_round2];
		_currentLevel_round2++;
		await currentLevel.FadeOutLevelVisuals();
		LoadLevel_Round2(_currentLevel_round2);
		levels_round2[_currentLevel_round2].FadeInLevelVisuals();
	}

	void EnableImageView_Round2()
	{
		_isOnRound1 = false;

		ImageView_Round2.SetActive(true);

		ImageView_Round1.SetActive(false);
		drawCanvas.gameObject.SetActive(false);
		VideoView.SetActive(false);
	}

	#region

	#endregion

	[Serializable]
	public class Level_Round2
	{
		public GameObject Level;
		public Texture2D backgroundImage;

		public DrawClickableArea[] DrawAreas;

		public Button audioHintButton;
		public Button nextButton;

		public AudioClip hintClip;

		TabletMinigameController_Story _tablet;

		public void InitializeLevel()
		{
			_tablet = FindAnyObjectByType<TabletMinigameController_Story>();
			nextButton.gameObject.SetActive(false);
			nextButton.onClick.AddListener(_tablet.OnNextClicked_round2);

			foreach (var drawArea in DrawAreas)
			{
				drawArea.Enable();
				drawArea.onFinish = _tablet.OnFinishRound_2;
			}

			audioHintButton.onClick.AddListener(() =>
			{
				_tablet.audioSource.Stop();
				_tablet.audioSource.clip = hintClip;
				_tablet.audioSource.Play();
			});
		}

		public async UniTask FadeOutLevelVisuals(float time = 0.4f)
		{
			foreach (var drawing in DrawAreas)
			{
				drawing.DrawingDisplay.DOFade(0f, time);
			}

			audioHintButton.gameObject.SetActive(false);
			nextButton.gameObject.SetActive(false);

			await _tablet.backgroundImage_round2.DOFade(0f,time).AsyncWaitForCompletion();
			Level.SetActive(false);
		}

		public async void FadeInLevelVisuals(float time = 0.4f)
		{
			foreach (var drawing in DrawAreas)
			{
				await drawing.DrawingDisplay.DOFade(0f, 0.1f).AsyncWaitForCompletion();
			}

			foreach (var drawing in DrawAreas)
			{
				drawing.DrawingDisplay.DOFade(1f, time);
			}

			await _tablet.backgroundImage_round2.DOFade(1f, time).AsyncWaitForCompletion();
		}

		public bool IsCompleted()
		{
			foreach(var d in DrawAreas)
			{
				if(!d.IsDrawn) return false;
			}

			return true;
		}

		public void OnClickDrawHere()
		{
			_tablet.EnableDrawCanvas();
		}
	}

	#endregion
}
