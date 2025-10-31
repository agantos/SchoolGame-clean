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

		PlayIntroductionVideo();

		//Test
		//EnableImageView_Round1();
		//LoadLevel_Round1(0);
	}

	#region Draw Canvas Methods

	public void OnFinishRound_1()
	{
		EnableImageView_Round1();
		LoadNextLevel_Round1();
	}

	public void OnFinishRound_2()
	{
		Texture2D textureDrawn = drawCanvas.canvas.GetOverlayTextureCopy();
		_justDrawnArea = drawCanvas.ui_text.text;

		if (_justDrawnArea.Contains("begin")){
			drawing_StoryStart = textureDrawn;
			Debug.Log("Start");
		}
		else if (_justDrawnArea.Contains("event"))
		{
			drawing_StoryMiddle = textureDrawn;
			Debug.Log("Middle");

		}
		else if (_justDrawnArea.Contains("end"))
		{
			Debug.Log("End");

			drawing_StoryEnd = textureDrawn;
		}

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
		LoadLevel_Round1(0);
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
		ImageView_Round3.SetActive(false);
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
		LoadLevel_Round1(_currentLevel_round1);

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
		sparksEffect.PlayDisplaced(0.0005f, 0f, 0.0006f);
		_currentLevel_round1 = GetCompletedNumber(drawAreas_round1.ToArray());

		if (_currentLevel_round1 < levels_round1.Length)
		{
			LoadLevel_Round1(_currentLevel_round1);
		}
		else
		{
			backgroundImage_round1.texture = finalImage_round1;
			nextRoundButton.gameObject.SetActive(true);
			nextRoundButton.onClick.AddListener(ToRound2);
		}
	}

	void LoadLevel_Round1(int index)
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
		await backgroundCanvas_1.DOFade(0f, 0.4f).AsyncWaitForCompletion();

		// Start round_2
		EnableImageView_Round2();
		//LoadLevel_Round2(0);
		Round2_Begin();
		await backgroundCanvas_2.DOFade(1f, 0.4f).AsyncWaitForCompletion();


	}

	void EnableImageView_Round3()
	{
		ImageView_Round3.SetActive(true);


		drawCanvas.gameObject.SetActive(false);
		ImageView_Round1.SetActive(false);
		ImageView_Round2.SetActive(false);
		VideoView.SetActive(false);
	}

	async void ToRound3()
	{
		var backgroundCanvas_2 = backgroundImage_round2.GetComponent<CanvasGroup>();
		var backgroundCanvas_3 = backgroundImage_round3.GetComponent<CanvasGroup>();
		backgroundCanvas_3.alpha = 0f;

		// Fade round_1
		await backgroundCanvas_2.DOFade(0f, 0.4f).AsyncWaitForCompletion();

		// Start round_2
		EnableImageView_Round3();
		LoadLevel_Round3(0);
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
	Texture2D drawing_StoryStart;
	Texture2D drawing_StoryMiddle;
	Texture2D drawing_StoryEnd;
	string _justDrawnArea;


	void Round2_Begin()
	{
		backgroundImage_round2.texture = initialImage_round2;
		var button = backgroundImage_round2.GetComponent<Button>();

		button.enabled = true;
		button.onClick.AddListener(async () =>
		{
			var backgroundCanvas_2 = backgroundImage_round2.GetComponent<CanvasGroup>();
			button.enabled = false;

			await backgroundCanvas_2.DOFade(0f, 0.4f).AsyncWaitForCompletion();
			LoadLevel_Round2(0);
			await backgroundCanvas_2.DOFade(1f, 0.4f).AsyncWaitForCompletion();
			
		});
	}

	void LoadLevel_Round2(int index)
	{
		var level = levels_round2[index];
		level.Level.SetActive(true);
		level.InitializeLevel();

		backgroundImage_round2.texture = level.backgroundImage;
	}

	async void LoadNext_Round2()
	{
		sparksEffect.PlayDisplaced(0.00062f, 0f, 0.00034f);
		_currentLevel_round1 = GetCompletedNumber(drawAreas_round1.ToArray());

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
				var button = backgroundImage_round2.GetComponent<Button>();
				button.enabled = true;
				button.onClick.AddListener(ToRound3);
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
		ImageView_Round3.SetActive(false);
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
	}

	#endregion

	#region ROUND 3
	[Space]
	[Space]
	[Header("Round 3")]
	[SerializeField] GameObject ImageView_Round3;
	[SerializeField] RawImage backgroundImage_round3;
	[SerializeField] Level_Round3[] levels_round3;

	int _currentLevel_round3 = 0;


	async void LoadLevel_Round3(int index)
	{
		var level = levels_round3[index];

		backgroundImage_round3.texture = level.backgroundImage;

		Texture2D[] textures = { 
			drawing_StoryStart,
			drawing_StoryMiddle,
			drawing_StoryEnd
		};

		level.levelRoot.SetActive(true);
		level.Initialize(textures, index);


		backgroundImage_round3.GetComponent<CanvasGroup>().DOFade(1f, 0.4f);
		await level.SpawnImages();			
		level.NextButton.onClick.AddListener(LoadNextLevel_Round3);
	}

	async void LoadNextLevel_Round3()
	{
		if(_currentLevel_round3 < levels_round3.Length-1)
		{
			var level = levels_round3[_currentLevel_round3];
			//backgroundImage_round3.GetComponent<CanvasGroup>().DOFade(0f, 0.4f);
			//await level.FadeVisuals(0.4f);
			level.levelRoot.SetActive(false);

			_currentLevel_round3++;


			LoadLevel_Round3(_currentLevel_round3);
		}
		else
		{
			//EndGame
		}
	}

	[Serializable]
	public class Level_Round3
	{
		public GameObject levelRoot;
		public Texture2D backgroundImage;
		public RawImage[] imagesToDisplay;
		public Button NextButton;

		public void Initialize(Texture2D[] textures, int index)
		{
			if(imagesToDisplay.Length > 1)
			{
				int j = index - 1;
				for (int i = 0; i < imagesToDisplay.Length; i++)
				{
					imagesToDisplay[i].texture = textures[j];
					j++;

				}
			}
			else
			{
				imagesToDisplay[0].texture = textures[index];
			}


			if (imagesToDisplay.Length > 0)
			{
				imagesToDisplay[imagesToDisplay.Length - 1].transform.localScale = Vector3.zero;
			}

			NextButton.gameObject.SetActive(false);
		}

		public async UniTask FadeVisuals(float time)
		{
			foreach (var item in imagesToDisplay) {
				await item.DOFade(0, time).AsyncWaitForCompletion();
			}
		}

		public RawImage GetImageToFill()
		{
			return imagesToDisplay[imagesToDisplay.Length - 1];
		}

		public async UniTask SpawnImages()
		{
			for (int i = 0; i < imagesToDisplay.Length; i++)
			{
				var image = imagesToDisplay[i];

				if (i == imagesToDisplay.Length - 1)
				{
					await SpawnIconAnimated(image);
				}
				else SpawnIconImmediately(image);
				
			}		
		}

		void SpawnIconImmediately(RawImage image)
		{
			image.transform.localScale = Vector3.one;
		}

		async UniTask SpawnIconAnimated(RawImage imageToAnimate)
		{
			Transform t = imageToAnimate.transform;

			// Start at 0 scale
			t.localScale = Vector3.zero;

			// Sequence for cartoony pop
			Sequence seq = DOTween.Sequence();

			// Step 1: Quickly stretch up and squash sideways (overshoot)
			seq.Append(t.DOScaleX(1.2f, 0.35f).SetEase(Ease.OutBack));
			seq.Join(t.DOScaleY(0.8f, 0.35f).SetEase(Ease.OutBack));

			// Step 2: Bounce to normal scale
			seq.Append(t.DOScaleX(1f, 0.3f).SetEase(Ease.OutBack));
			seq.Join(t.DOScaleY(1f, 0.3f).SetEase(Ease.OutBack));

			await seq.AsyncWaitForCompletion();

			NextButton.gameObject.SetActive(true);
		}
	}

	#endregion
}
