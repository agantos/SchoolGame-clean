using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogueEventPlanner11 : DialogueEventPlanner_Base
{
	DialogueManager _dialogueManager;
	PlayerManager _playerManager;
	PlayerMovementController _playerMovementController;
	SceneController _sceneController;

	[SerializeField] Transform margaret;
	[SerializeField] SCR_DialogueNode byeMargaretDialogue;



	private void Start()
	{
		_dialogueManager = FindAnyObjectByType<DialogueManager>();
		_playerManager = FindAnyObjectByType<PlayerManager>();
		_playerMovementController = FindAnyObjectByType<PlayerMovementController>();
		_sceneController = FindAnyObjectByType<SceneController>();

		CreateEvent("MargaretSad", DialogueEvent.OnDialogueEvent.END_NODE, StartByeMargaretDialogue);
		CreateEvent("ByePeter", DialogueEvent.OnDialogueEvent.START_NODE, LookAtMargaret);		
	}

	async UniTask StartByeMargaretDialogue()
	{
		_dialogueManager.DialogueToStart = byeMargaretDialogue;
		_dialogueManager.StartDialogue();
	}

	async UniTask LookAtMargaret()
	{
		await _playerManager.CameraLookAt(margaret, playerLooksThereToo: true);
	}
}
