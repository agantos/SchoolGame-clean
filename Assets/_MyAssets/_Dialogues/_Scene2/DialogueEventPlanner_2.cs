using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogueEventPlanner_2 : DialogueEventPlanner_Base
{
    DialogueManager _dialogueManager;
    PlayerManager _playerManager;
    PlayerMovementController _playerMovementController;
	SceneController _sceneController;

	[SerializeField] Transform margaret;

    [SerializeField] SCR_DialogueNode margaretSadDialogue;
    [SerializeField] SCR_DialogueNode returnToMargaretDialogue;
    [SerializeField] DialogueArea returnToMargaretDialogueArea;




	private void Start()
    {
        _dialogueManager = FindAnyObjectByType<DialogueManager>();
        _playerManager = FindAnyObjectByType<PlayerManager>();
        _playerMovementController = FindAnyObjectByType<PlayerMovementController>();
        _sceneController = FindAnyObjectByType<SceneController>();

        CreateEvent("StartDialogueMargaret_1", DialogueEvent.OnDialogueEvent.START_NODE, LookAtMargaret);
        CreateEvent("StartDialogueMargaret_1", DialogueEvent.OnDialogueEvent.OPTION_B, SpawnMargaretSadDialogue);
        CreateEvent("StartDialogueMargaret_1", DialogueEvent.OnDialogueEvent.OPTION_A, ToScene3);

		CreateEvent("NoTalk", DialogueEvent.OnDialogueEvent.END_NODE, SpawnReturnToMargaretDialogue);
        CreateEvent("GoTalk", DialogueEvent.OnDialogueEvent.START_NODE, LookAtMargaret);

        CreateEvent("GoToSnackShopDialogue", DialogueEvent.OnDialogueEvent.START_NODE, LookAtMargaret);
        CreateEvent("GoToSnackShopDialogue", DialogueEvent.OnDialogueEvent.END_NODE, ToScene3);
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

        await UniTask.Delay(6000);
        _dialogueManager.DialogueToStart = returnToMargaretDialogue;
        _dialogueManager.StartDialogue();
        returnToMargaretDialogueArea.gameObject.SetActive(true);

    }

    async UniTask LookAtMargaret()
    {
        await _playerManager.CameraLookAt(margaret, playerLooksThereToo: true);
    }

    async UniTask ToScene3()
    {
        if(_sceneController != null)
        {
            await _sceneController.LoadNextScene();
        }
    }

}
