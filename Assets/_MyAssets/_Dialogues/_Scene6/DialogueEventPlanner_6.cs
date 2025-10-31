using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class DialogueEventPlanner_6 : DialogueEventPlanner_Base
{
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera sitDownView;
    [SerializeField] private CinemachineCamera attemptLeaveClassroomView;
    [SerializeField] private CinemachineCamera lookTeacherView;
    [SerializeField] private CinemachineCamera tabletView;

    [Header("Dialogues")]

    [SerializeField] private SCR_DialogueNode badEndingDialogue;
    [SerializeField] private SCR_DialogueNode boredDialogue;
    [SerializeField] private SCR_DialogueNode startGame;


	ScreenFader _screenFader;
    DialogueManager _dialogueManager;
    PlayerManager _playerManager;
    CinemachineCameraChanger _cameraChanger;

    TabletController_ShapesMinigame _minigame_shapes_controller;
    TabletAnimationController _tabletAnimationController;

    TransitionToCameraArea _sitDownArea;

    private void Start()
    {
        _screenFader = FindAnyObjectByType<ScreenFader>();
        _playerManager = FindAnyObjectByType<PlayerManager>();
        _cameraChanger = FindAnyObjectByType<CinemachineCameraChanger>();
        _tabletAnimationController = FindAnyObjectByType<TabletAnimationController>();
        _minigame_shapes_controller = FindAnyObjectByType<TabletController_ShapesMinigame>();

        _sitDownArea = FindAnyObjectByType<TransitionToCameraArea>();
        _sitDownArea.callback = StartBoredDialogue;

		_dialogueManager = FindAnyObjectByType<DialogueManager>();
        _dialogueManager.EventPlanner = this;

        CreateEvent("dillema", DialogueEvent.OnDialogueEvent.OPTION_A, AttemptLeaveClassroom);
        CreateEvent("bad_1", DialogueEvent.OnDialogueEvent.END_NODE, StartBadEndingDialogue);

        CreateEvent("bad_2", DialogueEvent.OnDialogueEvent.END_NODE, BadEndingEvent);
        CreateEvent("good", DialogueEvent.OnDialogueEvent.END_NODE, GoodEndingEvent);

		CreateEvent("startGame", DialogueEvent.OnDialogueEvent.END_NODE, OpenTabletAndGame);

	}

	#region Dialogue - Sit Transitions

	async UniTask StartBoredDialogue()
    {
		_playerManager.PlayerMovementController.DisableMovement();
		await UniTask.Delay(2500);

        _dialogueManager.DialogueToStart = boredDialogue;
        _dialogueManager.StartDialogue();
    }

    async UniTask BadEndingEvent()
    {
        // sit down
        await _cameraChanger.TransitionToCam(sitDownView);
        await _screenFader.PerformFadeTransition(3, 2, 3, message: "Time passes quickly...");
        await UniTask.Delay(1000);
		StartGameDialogue();
	}

	async UniTask GoodEndingEvent()
    {
        await _screenFader.PerformFadeTransition(3, 2, 3, message: "Time passes quickly...");
		await UniTask.Delay(1000);
        StartGameDialogue();
    }

	async UniTask StartGameDialogue()
	{
		_dialogueManager.DialogueToStart = startGame;
		_dialogueManager.StartDialogue();
	}

	async UniTask StartBadEndingDialogue()
    {
        _dialogueManager.DialogueToStart = badEndingDialogue;
        _dialogueManager.StartDialogue();
    }

    async UniTask TransitionToPlayerCam()
    {
        _cameraChanger.PositionPlayerToActiveCamera();
        _cameraChanger.TransitionBackToPlayerCamera();
    }

    async UniTask AttemptLeaveClassroom()
    {
        _cameraChanger.TransitionThroughCams(attemptLeaveClassroomView, lookTeacherView);
        await UniTask.Delay(2000);
    }
    #endregion

    #region Tablet & Minigame

    async UniTask OpenTabletAndGame()
    {
        // Change Camera, slide Tablet out, open Game
        await UniTask.Delay(200);
        await _cameraChanger.TransitionToCam(tabletView);
        await UniTask.Delay(300);

        await _tabletAnimationController.SlideTabletOut();
        _minigame_shapes_controller.StartGame();
        await UniTask.Delay(300);
        _playerManager.PlayerMovementController.DisableMovement();

        // Set what to do on ending of the Game
        _minigame_shapes_controller.onGameCompleted += OnCompleteMinigame;
    }

    async UniTask OnCompleteMinigame()
    {
        // Slide tablet animation with camera
        await UniTask.Delay(300);
        _tabletAnimationController.SlideTabletIn();
        await UniTask.Delay(1200);
        await _cameraChanger.TransitionToCam(sitDownView);
        await UniTask.Delay(2000);
    }
    #endregion



}
