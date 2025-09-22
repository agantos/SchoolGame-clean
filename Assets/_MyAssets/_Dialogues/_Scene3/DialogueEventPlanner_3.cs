using UnityEngine;
using Cysharp.Threading.Tasks;


public class DialogueEventPlanner_3 : DialogueEventPlanner_Base
{
    DialogueManager _dialogueManager;
    ScreenFader _screenFader;
    PlayerManager _playerManager;
    CinemachineCameraChanger _cameraChanger;

    [SerializeField] Transform margaret;
    
    [SerializeField] Transform afterQueuePosition;
    [SerializeField] Transform afterQueueRotation;
    [SerializeField] SCR_DialogueNode afterQueueDialogue;

    [SerializeField] private CinemachineCameraTransition toNotAskingCamera;
    [SerializeField] SCR_DialogueNode notAskedDialogue;

    [SerializeField] SCR_DialogueNode askDialogue;




    private void Start()
    {
        _dialogueManager = FindAnyObjectByType<DialogueManager>();
        _playerManager = FindAnyObjectByType<PlayerManager>();
        _screenFader = FindAnyObjectByType<ScreenFader>();
        _cameraChanger = FindAnyObjectByType<CinemachineCameraChanger>();

        CreateEvent("Polite_1", DialogueEvent.OnDialogueEvent.START_NODE, LookAtMargaret);
        CreateEvent("Polite_1", DialogueEvent.OnDialogueEvent.END_NODE, QueueTransition);

        CreateEvent("3_2_Rude", DialogueEvent.OnDialogueEvent.START_NODE, LookAtMargaret);
        CreateEvent("3_2_Rude", DialogueEvent.OnDialogueEvent.END_NODE, QueueTransition);

        CreateEvent("3_5", DialogueEvent.OnDialogueEvent.OPTION_B, NotAsked);
        CreateEvent("3_6_NoAsk", DialogueEvent.OnDialogueEvent.END_NODE, GoAsk);

    }

    async UniTask NotAsked()
    {
        await toNotAskingCamera.PerformTransitions();
        await UniTask.Delay(200);
    }

    async UniTask GoAsk()
    {
        _cameraChanger.TransitionBackToPlayerCamera();
        await UniTask.Delay(2000);

        _dialogueManager.DialogueToStart = askDialogue;
        _dialogueManager.StartDialogue();
    }

    async UniTask QueueTransition()
    {
        await _screenFader.PerformFadeTransition(2, 2, 3, "A moment later...", callBack: PositionAndRotatePeter);
        _dialogueManager.DialogueToStart = afterQueueDialogue;
        _dialogueManager.StartDialogue();
    }

    void PositionAndRotatePeter()
    {
        _playerManager.MoveToPosition(afterQueuePosition.transform.position);
        _playerManager.LookImmediately(afterQueueRotation);
    }

    async UniTask LookAtMargaret()
    {
        await _playerManager.CameraLookAt(margaret, playerLooksThereToo: true);
    }
}
