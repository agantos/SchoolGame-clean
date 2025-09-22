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
        completeTraceEffect.gameObject.SetActive(false);
        StartGame();
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
        ImageView.SetActive(false);
        VideoView.SetActive(false);

        if (onGameCompleted != null) await onGameCompleted.Invoke();

    }

    #region VIDEO VIEW

    [Header("Video View")]
    [SerializeField] GameObject VideoView;
    [SerializeField] VideoPlayer mainVideoPlayer;
    [SerializeField] VideoClip introductionVideo;

    void EnableVideoView()
    {
        ImageView.SetActive(false);
        VideoView.SetActive(true);
    }

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

    async void OnIntroVideoEnd(VideoPlayer vp)
    {
        CanvasGroup videoCanvasGroup = vp.GetComponent<CanvasGroup>();
        CanvasGroup backgroundCanvasGroup = background.GetComponent<CanvasGroup>();

        backgroundCanvasGroup.alpha = 0;
        background.texture = startRound1;

        await videoCanvasGroup.DOFade(0f, 1f).AsyncWaitForCompletion();
        EnableImageView();
        await backgroundCanvasGroup.DOFade(1f, 1f).AsyncWaitForCompletion();

        var imageButton = background.GetComponent<Button>();
        imageButton.enabled = true;

        imageButton.onClick.AddListener(() =>
        {
            imageButton.enabled = false;
            imageButton.onClick.RemoveAllListeners();
            LoadTraceMinigame(0);
        });

        
    }

    #endregion


    void EnableImageView()
    {
        ImageView.SetActive(true);
        VideoView.SetActive(false);
    }


    #region IMAGE VIEW
    [Header("Image View")]
    [SerializeField] GameObject ImageView;
    [SerializeField] RawImage background;
    [SerializeField] Texture2D startRound1;
    [SerializeField] Texture2D startRound2;

    #endregion

    #region Trace Shape Part
    [Header("Trace Shape Objects")]

    [SerializeField] GameObject circleParent;
    [SerializeField] GameObject triangleParent;
    [SerializeField] GameObject squareParent;

    [SerializeField] MinigameShapes_Trace_Level[] traceLevels;


    List<TraceableShapePart> squarePartsTotal;
    List<TraceableShapePart> circlePartsTotal;
    List<TraceableShapePart> trianglePartsTotal;

    List<GameObject> squarePartsFilled;
    List<GameObject> circlePartsFilled;
    List<GameObject> trianglePartsFilled;

    [SerializeField] ParticleSystem completeTraceEffect;

    int _currentTraceLevel;


    async void LoadTraceMinigame(int levelIndex)
    {
        var level = traceLevels[levelIndex];
        _currentTraceLevel = levelIndex;
        var canvasGroup = background.GetComponent<CanvasGroup>();


        DeactivateTraceableShapes();
        InitTraceableShape(level.type);

        await canvasGroup.DOFade(0, 0.5f).AsyncWaitForCompletion();
        background.texture = level.imageTrace;
        await canvasGroup.DOFade(1, 0.5f).AsyncWaitForCompletion();

        audioSource.Stop();
        audioSource.clip = level.clip;
        audioSource.Play();
    }   

    async void NextTraceMinigame()
    {
        _currentTraceLevel++;
        if(_currentTraceLevel < traceLevels.Length)
        {
            LoadTraceMinigame(_currentTraceLevel);
        }
        else
        {
            var canvasGroup = background.GetComponent<CanvasGroup>();
            await canvasGroup.DOFade(0, 0.7f).AsyncWaitForCompletion();
            background.texture = startRound2;
            await canvasGroup.DOFade(1, 0.7f).AsyncWaitForCompletion();
        }
    }

    void DeactivateTraceableShapes()
    {
        circleParent.SetActive(false);
        triangleParent.SetActive(false);
        squareParent.SetActive(false);
    }

    async void OnTracedWholeShape(List<TraceableShapePart> partList)
    {
        var level = traceLevels[_currentTraceLevel];
        DeactivateTraceableShapes();

        completeTraceEffect.gameObject.SetActive(true);
        completeTraceEffect.Play();
        await UniTask.Delay(500);

        background.texture = level.imageDone;

        await UniTask.Delay(2000);

        NextTraceMinigame();

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
            OnTracedWholeShape(part.fullList);
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

    void InitTraceableShape(MinigameShapes_Trace_Level_Type type)
    {
        switch (type)
        {
            case MinigameShapes_Trace_Level_Type.triangle:
                triangleParent.SetActive(true);
                trianglePartsFilled = new List<GameObject>();

                trianglePartsTotal = triangleParent.GetComponentsInChildren<TraceableShapePart>(includeInactive: true).ToList();
                foreach (var part in trianglePartsTotal)
                {
                    part.Init(trianglePartsFilled, trianglePartsTotal);
                }
                break;

            case MinigameShapes_Trace_Level_Type.square:
                squareParent.SetActive(true);
                squarePartsFilled = new List<GameObject>();
                squarePartsTotal = squareParent.GetComponentsInChildren<TraceableShapePart>(includeInactive: true).ToList();

                foreach (var part in squarePartsTotal)
                {
                    part.Init(squarePartsFilled, squarePartsTotal);
                }
                break;

            case MinigameShapes_Trace_Level_Type.circle:
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
    #endregion

    [Serializable]
    public class MinigameShapes_Trace_Level
    {
        public Texture2D imageTrace;
        public Texture2D imageDone;
        public MinigameShapes_Trace_Level_Type type;
        public AudioClip clip;
    }

    public enum MinigameShapes_Trace_Level_Type { 
        triangle,square,circle
    }

}
