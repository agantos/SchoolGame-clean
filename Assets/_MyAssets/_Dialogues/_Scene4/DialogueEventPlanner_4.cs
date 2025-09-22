using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class DialogueEventPlanner_4 : DialogueEventPlanner_Base
{

    ScreenFader _screenFader;
    DialogueManager _dialogueManager;
    PlayerManager _playerManager;
    CinemachineCameraChanger _cameraChanger;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera classRoomView;

    [SerializeField] private CinemachineCamera attemptLeaveClassroomView;
    [SerializeField] private CinemachineCamera lookTeacherView;

    [SerializeField] private CinemachineCamera leaveClassroomView;

    [Header("Transforms")]
    [SerializeField] private Transform exitCameraTransform;

    [Header("Dialogues")]
    [SerializeField] private SCR_DialogueNode wantToGoToToilet;

    private void Start()
    {
        _screenFader = FindAnyObjectByType<ScreenFader>();
        _dialogueManager = FindAnyObjectByType<DialogueManager>();
        _playerManager = FindAnyObjectByType<PlayerManager>();
        _cameraChanger = FindAnyObjectByType<CinemachineCameraChanger>();


        CreateEvent("EnterClassroom", DialogueEvent.OnDialogueEvent.OPTION_A, EnterClassroom);
        
        CreateEvent("AskOrNo", DialogueEvent.OnDialogueEvent.OPTION_A, AttemptLeaveClassroom);
        CreateEvent("4_3_bad", DialogueEvent.OnDialogueEvent.END_NODE, TransitionToPlayerCam);

        CreateEvent("4_4_good", DialogueEvent.OnDialogueEvent.END_NODE, LeaveClassroom);
    }

    async UniTask EnterClassroom()
    {
        //toClassroom.PerformTransitions();
        _cameraChanger.TransitionToCam(classRoomView);
        await UniTask.Delay(2500);

        await _screenFader.PerformFadeTransition(3, 4, 3, message: "The lesson starts and time goes by...");
        _dialogueManager.DialogueToStart = wantToGoToToilet;
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
        _cameraChanger.PositionPlayerToActiveCamera();
        _cameraChanger.TransitionBackToPlayerCamera();
    }

    async UniTask LeaveClassroom()
    {
        await UniTask.Delay(500);

        await _cameraChanger.TransitionToCam(leaveClassroomView);
        _cameraChanger.PositionPlayerToActiveCamera();
        _cameraChanger.TransitionBackToPlayerCamera();
    }
}
