using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TabletController_Minigame1 : MonoBehaviour
{
    
    [Header("Levels")]
    [SerializeField] Level[] levels;
    [SerializeField] AudioSource audioSource;

    public Func<UniTask> onGameCompleted;

    void Awake()
    {
        var canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    private void Start()
    {
        _backgroundGroup = backgroundImage.gameObject.GetComponent<CanvasGroup>();
		
        var finalButton = FinalView.GetComponent<Button>();
        finalButton.onClick.AddListener(async () => { await EndGame(); });
	}

	public async void StartGame()
    {
        await UniTask.Delay(500);
        PlayIntroductionVideo();
    }

    public async UniTask EndGame()
    {
		ImageView.SetActive(false);
		VideoView.SetActive(false);

		if (onGameCompleted != null) await onGameCompleted.Invoke();

    }

    #region VIDEO VIEW

    [Header("Video View")]
    [SerializeField] GameObject VideoView;
    [SerializeField] VideoPlayer mainVideoPlayer;
    [SerializeField] VideoClip introductionVideo;
    [SerializeField] VideoClip feelingsVideo;

    void PlayIntroductionVideo()
    {
        var button = VideoView.GetComponent<Button>();
        button.enabled = false;

        EnableVideoView();
        mainVideoPlayer.clip = introductionVideo;
        mainVideoPlayer.Play();

        // Subscribe to event when video finishes
        mainVideoPlayer.loopPointReached += OnIntroVideoEnd;
    }

    void OnIntroVideoEnd(VideoPlayer vp)
    {
        // Remove old listeners and add the new one
        var button = VideoView.GetComponent<Button>();
        button.enabled = true;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(StartFeelingsVideo);

        //unsubscribe so it doesn’t trigger multiple times
        vp.loopPointReached -= OnIntroVideoEnd;
    }

    async void StartFeelingsVideo()
    {
        var button = VideoView.GetComponent<Button>();
        button.enabled = false;

        VideoView.GetComponent<Button>().onClick.RemoveAllListeners();
        mainVideoPlayer.clip = feelingsVideo;
        await UniTask.Delay(250);
        mainVideoPlayer.Play();

        mainVideoPlayer.loopPointReached += onFeelingsVideoEnded;

    }

    void onFeelingsVideoEnded(VideoPlayer vp)
    {
        EnableImageView();
        SetLevel(levels[0], false);

        mainVideoPlayer.loopPointReached -= onFeelingsVideoEnded;
    }

    #endregion

    void EnableVideoView()
    {
        ImageView.SetActive(false);
        VideoView.SetActive(true);
    }

    void EnableImageView()
    {
        ImageView.SetActive(true);
        VideoView.SetActive(false);
    }


    #region IMAGE VIEW
    [Header("Image View")]
    [SerializeField] GameObject ImageView;
    [SerializeField] Button leftOptionButton;
    [SerializeField] Button rightOptionButton;
    [SerializeField] VideoPlayer middleVideoPlayer;
    [SerializeField] RawImage backgroundImage;
    [SerializeField] GameObject FinalView;

    CanvasGroup _backgroundGroup;

    [SerializeField] AudioClip[] correctSounds;
    [SerializeField] AudioClip[] wrongSounds;


    int _currentLevelIndex = 0;

    void NextLevel()
    {
        _currentLevelIndex++;

        if (_currentLevelIndex < levels.Length) { 
            SetLevel(levels[_currentLevelIndex]);   
        }
        else
        {
            TransitionToFinalImage();
        }
    }

    void TransitionToFinalImage()
    {
        var button = FinalView.GetComponent<Button>();
        button.enabled = false;
        middleVideoPlayer.transform.parent.gameObject.SetActive(false);
        CanvasGroup finalGroup = FinalView.GetComponent<CanvasGroup>();

        finalGroup.alpha = 0f;
        FinalView.SetActive(true);

        // Build DOTween sequence
        Sequence seq = DOTween.Sequence();

		seq.Append(_backgroundGroup.DOFade(0f, 0.5f)) 
           .Append(finalGroup.DOFade(1f, 0.5f))      
           .OnComplete(() =>
           {
			   ImageView.gameObject.SetActive(false);
               button.enabled = true;
           });

    }

    void SetLevel(Level level, bool playTransition = true)
    {
        EnableImageView();

        // Remove old listeners
        leftOptionButton.onClick.RemoveAllListeners();
        rightOptionButton.onClick.RemoveAllListeners();

        if (level.isLeftCorrect)
        {
            leftOptionButton.onClick.AddListener(OnRightChoice);
            rightOptionButton.onClick.AddListener(OnWrongChoice);
        }
        else
        {
            leftOptionButton.onClick.AddListener(OnWrongChoice);
            rightOptionButton.onClick.AddListener(OnRightChoice);
        }

        // Kill any ongoing tweens on this group to avoid overlap
        _backgroundGroup.DOKill();

		if (playTransition) {
			middleVideoPlayer.transform.parent.gameObject.SetActive(false);

			DOTween.Sequence()
                .Append(_backgroundGroup.DOFade(0f, 0.5f))
                .AppendCallback(() =>
                {
                    backgroundImage.texture = level.image;
                })
                .Append(_backgroundGroup.DOFade(1f, 1f))
                .Play().OnComplete(async () =>
				{
					middleVideoPlayer.transform.parent.gameObject.SetActive(true);
                    await UniTask.WaitForEndOfFrame();
					await UniTask.Delay(300);
				});
        }


        // Video setup
        middleVideoPlayer.Stop();
        middleVideoPlayer.isLooping = true;
        middleVideoPlayer.clip = level.clip;
        middleVideoPlayer.Play();
    }

    async void OnRightChoice()
    {
        try
        {
            audioSource.Stop();
            audioSource.clip = correctSounds[UnityEngine.Random.Range(0, correctSounds.Length)];
            audioSource.Play();
            NextLevel();
            await UniTask.Delay(2000);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Async error: {ex}");
        }
    }

    async void OnWrongChoice()
    {
        try
        {
            audioSource.Stop();
            audioSource.clip = wrongSounds[UnityEngine.Random.Range(0, wrongSounds.Length)];
            audioSource.Play();
            await UniTask.Delay(1000);

        }
        catch (Exception ex)
        {
            Debug.LogError($"Async error: {ex}");
        }
    }

    #endregion

  
    #region

    #endregion

    #region

    #endregion

    #region

    #endregion

    #region

    #endregion

    #region

    #endregion

    [Serializable]
    public struct Level
    {
        public Texture2D image;
        public VideoClip clip;
        public bool isLeftCorrect;
    }

}
