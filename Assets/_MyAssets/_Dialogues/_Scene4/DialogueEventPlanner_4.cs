using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Video;

public class DialogueEventPlanner_4 : DialogueEventPlanner_Base
{

    ScreenFader _screenFader;
    DialogueManager _dialogueManager;
    PlayerManager _playerManager;
    ProjectorController _projector;
    CinemachineCameraChanger _cameraChanger;
    SceneController _sceneController;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera classRoomView;

    [SerializeField] private CinemachineCamera attemptLeaveClassroomView;
    [SerializeField] private CinemachineCamera lookTeacherView;

    [SerializeField] private CinemachineCamera leaveClassroomView;

    [Header("Transforms")]
    [SerializeField] private Transform exitCameraTransform;

    [Header("Dialogues")]
    [SerializeField] private SCR_DialogueNode wantToGoToToilet;
    [SerializeField] private SCR_DialogueNode startVideo;

	[Header("Videos")]
	[SerializeField] private VideoClip rulesVideo;

    private TransitionToCameraArea transitionArea;

	private void Start()
    {
        _screenFader = FindAnyObjectByType<ScreenFader>();
        _playerManager = FindAnyObjectByType<PlayerManager>();
        _cameraChanger = FindAnyObjectByType<CinemachineCameraChanger>();
        _projector = FindAnyObjectByType<ProjectorController>();
        _sceneController = FindAnyObjectByType<SceneController>();

        transitionArea = FindAnyObjectByType<TransitionToCameraArea>();
        transitionArea.callback = StartRulesVideoDialogue;

		_dialogueManager = FindAnyObjectByType<DialogueManager>();
		_dialogueManager.EventPlanner = this;


		CreateEvent("StartVideo", DialogueEvent.OnDialogueEvent.END_NODE, PlayProjectorVideo);
        
        CreateEvent("AskOrNo", DialogueEvent.OnDialogueEvent.OPTION_A, AttemptLeaveClassroom);
        CreateEvent("4_3_bad", DialogueEvent.OnDialogueEvent.END_NODE, TransitionToPlayerCam);

        CreateEvent("4_4_good", DialogueEvent.OnDialogueEvent.END_NODE, LeaveClassroom);
    }

    async UniTask StartRulesVideoDialogue()
    {
        _playerManager.PlayerMovementController.DisableMovement();
		await UniTask.Delay(1000);
		_dialogueManager.DialogueToStart = startVideo;
		_dialogueManager.StartDialogue();
	}

	async UniTask PlayProjectorVideo()
	{
		_playerManager.PlayerMovementController.DisableMovement();
		await UniTask.Delay(1000);
		_projector.onProjectorClosed += PlayWantToiletDialogue;
		_projector.OpenProjectorVideo(rulesVideo);
	}

	async UniTask PlayWantToiletDialogue()
	{
		await _cameraChanger.TransitionToCam(classRoomView);

		await UniTask.Delay(2000);
		_dialogueManager.DialogueToStart = wantToGoToToilet;
		_dialogueManager.StartDialogue();
	}

	async UniTask TransitionToPlayerCam()
    {
        _cameraChanger.PositionPlayerToActiveCamera();
		_ = _cameraChanger.TransitionBackToPlayerCamera();
        await _sceneController.LoadNextScene();
    }

    async UniTask AttemptLeaveClassroom()
    {
        _ = _cameraChanger.TransitionThroughCams(attemptLeaveClassroomView, lookTeacherView);
        await UniTask.Delay(2000);
        _cameraChanger.PositionPlayerToActiveCamera();
		_ = _cameraChanger.TransitionBackToPlayerCamera();
    }

    async UniTask LeaveClassroom()
    {
        await UniTask.Delay(500);

        await _cameraChanger.TransitionToCam(leaveClassroomView);
        _cameraChanger.PositionPlayerToActiveCamera();
		_ = _cameraChanger.TransitionBackToPlayerCamera();
		await _sceneController.LoadNextScene();
	}
}
