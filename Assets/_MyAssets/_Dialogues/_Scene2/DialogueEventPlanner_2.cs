using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogueEventPlanner_2 : DialogueEventPlanner_Base
{
    DialogueManager _dialogueManager;
    PlayerManager _playerManager;
    PlayerMovementController _playerMovementController;

    [SerializeField] Transform margaret;

    [SerializeField] SCR_DialogueNode margaretSadDialogue;
    [SerializeField] SCR_DialogueNode returnToMargaretDialogue;
    [SerializeField] DialogueArea returnToMargaretDialogueArea;


    private void Start()
    {
        _dialogueManager = FindAnyObjectByType<DialogueManager>();
        _playerManager = FindAnyObjectByType<PlayerManager>();
        _playerMovementController = FindAnyObjectByType<PlayerMovementController>();

        CreateEvent("StartDialogueMargaret_1", DialogueEvent.OnDialogueEvent.START_NODE, LookAtMargaret);
        CreateEvent("StartDialogueMargaret_1", DialogueEvent.OnDialogueEvent.OPTION_B, SpawnMargaretSadDialogue);

        CreateEvent("NoTalk", DialogueEvent.OnDialogueEvent.END_NODE, SpawnReturnToMargaretDialogue);
        CreateEvent("GoTalk", DialogueEvent.OnDialogueEvent.START_NODE, LookAtMargaret);

        CreateEvent("GoToSnackShopDialogue", DialogueEvent.OnDialogueEvent.START_NODE, LookAtMargaret);
    }

    async UniTask SpawnMargaretSadDialogue()
    {
        _playerMovementController.EnableMovement();

        await UniTask.Delay(4000);
        await LookAtMargaret();
        
        _dialogueManager.DialogueToStart = margaretSadDialogue;
        _dialogueManager.StartDialogue();
    }

    async UniTask SpawnReturnToMargaretDialogue()
    {
        await _playerManager.ReturnToOriginalPlayerView();

        await UniTask.Delay(9000);
        _dialogueManager.DialogueToStart = returnToMargaretDialogue;
        _dialogueManager.StartDialogue();
        returnToMargaretDialogueArea.gameObject.SetActive(true);

    }

    async UniTask LookAtMargaret()
    {
        await _playerManager.CameraLookAt(margaret, playerLooksThereToo: true);
    }

}
