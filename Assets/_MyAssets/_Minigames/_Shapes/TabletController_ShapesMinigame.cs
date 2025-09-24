using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

public class TabletController_ShapesMinigame : MonoBehaviour
{

    [Header("Levels")]
    [SerializeField] AudioSource audioSource;


    public Func<UniTask> onGameCompleted;

    void Awake()
    {
        var canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    private void Start()
    {
    }
    bool _isTracing;
    void Update()
    {
        HandlePartTraceInput();
    }

    public async void StartGame()
    {
        await UniTask.Delay(500);
        PlayIntroductionVideo();
    }

    public async UniTask EndGame()
    {

        if (onGameCompleted != null) await onGameCompleted.Invoke();
     
        ImageView_Round1.SetActive(false);
        ImageView_Round2.SetActive(false);
        VideoView.SetActive(false);
    }

    #region VIDEO VIEW

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
        CanvasGroup backgroundCanvasGroup = background_Round1.GetComponent<CanvasGroup>();

        backgroundCanvasGroup.alpha = 0;
        background_Round1.texture = startImage_Round1;

        await videoCanvasGroup.DOFade(0f, 1f).AsyncWaitForCompletion();
        EnableImageView_Round1();
        
        audioSource.clip = startAudio_Round1;
        audioSource.Play();

        await backgroundCanvasGroup.DOFade(1f, 1f).AsyncWaitForCompletion();

        var imageButton = background_Round1.GetComponent<Button>();
        imageButton.enabled = true;

        imageButton.onClick.AddListener(() =>
        {
            imageButton.enabled = false;
            imageButton.onClick.RemoveAllListeners();
            LoadTraceMinigame(0);
        });

        
    }
	#endregion




	#region IMAGE VIEW ROUND 1
	[Space]
	[Space]
	[Header("Round 1 - Image View")]
    [SerializeField] GameObject ImageView_Round1;
    [SerializeField] RawImage background_Round1;
    [SerializeField] Texture2D startImage_Round1;
	[SerializeField] AudioClip startAudio_Round1;

	void EnableImageView_Round1()
	{
		ImageView_Round1.SetActive(true);
		ImageView_Round2.SetActive(false);

		VideoView.SetActive(false);
	}

	#region ROUND 1: Trace Shape
	[Header("Trace Shape Objects")]

    [SerializeField] GameObject circleParent;
    [SerializeField] GameObject triangleParent;
    [SerializeField] GameObject squareParent;

    [SerializeField] MinigameShapes_Round1_Level[] traceLevels;


    List<TraceableShapePart> squarePartsTotal;
    List<TraceableShapePart> circlePartsTotal;
    List<TraceableShapePart> trianglePartsTotal;

    List<GameObject> squarePartsFilled;
    List<GameObject> circlePartsFilled;
    List<GameObject> trianglePartsFilled;

    [SerializeField] ParticleEffect completeTraceEffect;
	[SerializeField] ParticleEffect confettiEffect;

	int _currentTraceLevel;


    async void LoadTraceMinigame(int levelIndex)
    {
        var level = traceLevels[levelIndex];
        _currentTraceLevel = levelIndex;
        var canvasGroup = background_Round1.GetComponent<CanvasGroup>();


        DeactivateTraceableShapes();
        InitTraceableShape(level.type);

        await canvasGroup.DOFade(0, 0.5f).AsyncWaitForCompletion();
        background_Round1.texture = level.imageTrace;
        await canvasGroup.DOFade(1, 0.5f).AsyncWaitForCompletion();

        audioSource.Stop();
        audioSource.clip = level.clip;
        audioSource.Play();
    }   

    async void OnCompletedLevel_Round1()
    {
        _currentTraceLevel++;
        // next level
        if(_currentTraceLevel < traceLevels.Length)
        {
            LoadTraceMinigame(_currentTraceLevel);
        }
        // go to Round 2
        else
        {
            // Fade from round_1 visual
			var canvasGroup_1 = background_Round1.GetComponent<CanvasGroup>();
			var canvasGroup_2 = background_Round2.GetComponent<CanvasGroup>();
            canvasGroup_2.alpha = 0;

			await canvasGroup_1.DOFade(0, 0.6f).AsyncWaitForCompletion();
            
            // Fade to round 2 visual
            EnableImageView_Round2();

			background_Round2.texture = startImage_Round2;
			canvasGroup_2.DOFade(1, 0.6f);
            
            // Play round 2 intro
            audioSource.clip = startAudio_Round2;
            audioSource.Play();

            // Play visual effect
            await UniTask.Delay(200);
			confettiEffect.Play();

            // Set up button that starts the first level
            var round2Button = background_Round2.GetComponent<Button>();
            round2Button.enabled = true;

            background_Round2.GetComponent<Button>().onClick.AddListener(() =>
            {
                LoadLevel_Round2(0);
                round2Button.enabled = false;
            });
		}
	}

    void DeactivateTraceableShapes()
    {
        circleParent.SetActive(false);
        triangleParent.SetActive(false);
        squareParent.SetActive(false);
    }

    async void TransitionToNext_Round1(List<TraceableShapePart> partList)
    {
        var level = traceLevels[_currentTraceLevel];
        DeactivateTraceableShapes();

        completeTraceEffect.Play();
        await UniTask.Delay(500);

        background_Round1.texture = level.imageDone;

        await UniTask.Delay(1000);

		OnCompletedLevel_Round1();

    }

    public async void OnPartTrace(TraceableShapePart part, List<GameObject> listToFill)
    {
        if (part == null) return;
        if (part.isTraced) return;

        var canvasGroup = part.GetComponent<CanvasGroup>();
        var rawImage = part.GetComponent<RawImage>();

        if (canvasGroup == null || rawImage == null)
            return;

        // Reset
        canvasGroup.alpha = 0f;
        listToFill.Add(part.gameObject);
        part.isTraced = true;

        // Check if shape is completed
        if(part.fullList.Count == part.traceableList.Count)
        {
            TransitionToNext_Round1(part.fullList);
            return;
        }

        // Kill any previous tween on this canvasGroup
        part.currentTween?.Kill();

        // Fade in
        part.currentTween = canvasGroup
            .DOFade(1f, 0.4f)
            .SetEase(Ease.InOutSine);

        await part.currentTween.AsyncWaitForCompletion();

        // Delay
        await UniTask.Delay(1500);

        // Fade out
        part.currentTween = canvasGroup
            .DOFade(0f, 0.2f)
            .SetEase(Ease.InOutSine);

        await part.currentTween.AsyncWaitForCompletion();

        part.currentTween = null;

        // Another callback here if you want
        part.isTraced = false;
        listToFill.Remove(part.gameObject);
    }

    void InitTraceableShape(MinigameShapes_Shape_Type type)
    {
        switch (type)
        {
            case MinigameShapes_Shape_Type.triangle:
                triangleParent.SetActive(true);
                trianglePartsFilled = new List<GameObject>();

                trianglePartsTotal = triangleParent.GetComponentsInChildren<TraceableShapePart>(includeInactive: true).ToList();
                foreach (var part in trianglePartsTotal)
                {
                    part.Init(trianglePartsFilled, trianglePartsTotal);
                }
                break;

            case MinigameShapes_Shape_Type.square:
                squareParent.SetActive(true);
                squarePartsFilled = new List<GameObject>();
                squarePartsTotal = squareParent.GetComponentsInChildren<TraceableShapePart>(includeInactive: true).ToList();

                foreach (var part in squarePartsTotal)
                {
                    part.Init(squarePartsFilled, squarePartsTotal);
                }
                break;

            case MinigameShapes_Shape_Type.circle:
                circleParent.SetActive(true);
                circlePartsFilled = new List<GameObject>();

                circlePartsTotal = circleParent.GetComponentsInChildren<TraceableShapePart>(includeInactive: true).ToList();
                foreach (var part in circlePartsTotal)
                {
                    part.Init(circlePartsFilled, circlePartsTotal);
                }
                break;
        }
    }

    private void HandlePartTraceInput()
    {
        if (Touchscreen.current == null || EventSystem.current == null)
            return;

        foreach (var touch in Touchscreen.current.touches)
        {
            if (!touch.press.isPressed)
                continue;

            int fingerId = touch.touchId.ReadValue();

            // Check if this finger is over UI
            if (EventSystem.current.IsPointerOverGameObject(fingerId))
            {
                Vector2 pos = touch.position.ReadValue();

                // Convert into a PointerEventData so we can query the exact UI element
                var pointerData = new PointerEventData(EventSystem.current)
                {
                    position = pos
                };

                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                foreach (var result in results)
                {
                    var part = result.gameObject.GetComponent<TraceableShapePart>();
                    if (part != null)
                    {
                        OnPartTrace(part, part.traceableList);
                    }
                }
            }
        }
    }

	[Serializable]
	public class MinigameShapes_Round1_Level
	{
		public Texture2D imageTrace;
		public Texture2D imageDone;
		public MinigameShapes_Shape_Type type;
		public AudioClip clip;
	}
	#endregion

	#endregion


	#region IMAGE VIEW ROUND 2
	[Space]
	[Space]
	[Header("Round 2 - Image View")]
	[SerializeField] GameObject ImageView_Round2;
    [SerializeField] RawImage background_Round2;
	[SerializeField] Texture2D startImage_Round2;
	[SerializeField] AudioClip startAudio_Round2;
    [SerializeField] Texture2D finalImage;

	[Space]
	[SerializeField] Button circleButton;
	[SerializeField] Button squareButton;
	[SerializeField] Button triangleButton;
	
    [Space]
	[SerializeField] MinigameShapes_Round2_Level[] levels_round2;
    [SerializeField] AudioClip[] correctSounds;
    [SerializeField] AudioClip[] wrongSounds;


    int _currentLevel_round2;

    void PlayRandomSound(AudioClip[] sounds)
    {
        var index = UnityEngine.Random.Range(0, correctSounds.Length);

        audioSource.Stop();
        audioSource.clip = sounds[index];
        audioSource.Play();
    }

    void EnableImageView_Round2()
	{
		ImageView_Round1.SetActive(false);
		ImageView_Round2.SetActive(true);

		VideoView.SetActive(false);
	}

    async void LoadLevel_Round2(int index)
    {
        if (index > levels_round2.Length - 1) return;

        var canvasGroup = background_Round2.GetComponent<CanvasGroup>();

        _currentLevel_round2 = index;

        MinigameShapes_Round2_Level level = levels_round2[index];

        await canvasGroup.DOFade(0, 0.5f).AsyncWaitForCompletion();
        background_Round2.texture = level.bakcgroundImage;
        await canvasGroup.DOFade(1, 0.5f).AsyncWaitForCompletion();

        squareButton.onClick.RemoveAllListeners();
        circleButton.onClick.RemoveAllListeners();
        triangleButton.onClick.RemoveAllListeners();


        switch (level.correctAnswer)
        {
            case MinigameShapes_Shape_Type.triangle:
                triangleButton.onClick.AddListener(OnCorrectChoice);
                
                circleButton.onClick.AddListener(OnWrongChoice);
                squareButton.onClick.AddListener(OnWrongChoice);
                break;
            case MinigameShapes_Shape_Type.circle:
                circleButton.onClick.AddListener(OnCorrectChoice);
                
                triangleButton.onClick.AddListener(OnWrongChoice);
                squareButton.onClick.AddListener(OnWrongChoice);
                break;
            case MinigameShapes_Shape_Type.square:
                squareButton.onClick.AddListener(OnCorrectChoice);

                triangleButton.onClick.AddListener(OnWrongChoice);
                circleButton.onClick.AddListener(OnWrongChoice);
                break;
        }
    }

    async void OnCorrectChoice()
    {
        var canvasGroup = background_Round2.GetComponent<CanvasGroup>();

        _currentLevel_round2 += 1;
        PlayRandomSound(correctSounds);

        // Load Next Level
        if (_currentLevel_round2 < levels_round2.Length)
        {
            LoadLevel_Round2(_currentLevel_round2);
        }
        // Go to end image
        else
        {
            squareButton.onClick.RemoveAllListeners();
            circleButton.onClick.RemoveAllListeners();
            triangleButton.onClick.RemoveAllListeners();

            await canvasGroup.DOFade(0, 1).AsyncWaitForCompletion();
            confettiEffect.Play();
            background_Round2.texture = finalImage;
            await canvasGroup.DOFade(1, 1).AsyncWaitForCompletion();
            await UniTask.Delay(500);
            confettiEffect.Play();

            var button = background_Round2.GetComponent<Button>();
            button.enabled = true;
            button.onClick.AddListener(async () => { await EndGame(); });
        }
    }

    void OnWrongChoice()
    {
        PlayRandomSound(wrongSounds);
    }

    [Serializable]
    public class MinigameShapes_Round2_Level
    {
        public MinigameShapes_Shape_Type correctAnswer;
        public Texture2D bakcgroundImage;
    }

	#endregion

	public enum MinigameShapes_Shape_Type { 
        triangle,square,circle
    }

}
