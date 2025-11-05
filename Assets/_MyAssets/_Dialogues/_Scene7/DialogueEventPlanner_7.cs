using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogueEventPlanner_7 : DialogueEventPlanner_Base
{
	DialogueManager _dialogueManager;
	PlayerManager _playerManager;
	PlayerMovementController _playerMovementController;
	SceneController _sceneController;

	[SerializeField] Transform Nick;
	[SerializeField] Transform Greg;
	[SerializeField] Transform focusFirstDialogue;

	[SerializeField] SCR_DialogueNode PeterEndWait;
	[SerializeField] SCR_DialogueNode PeterFalls;
	[SerializeField] SCR_DialogueNode CleanWound;

	private void Start()
	{
		_dialogueManager = FindAnyObjectByType<DialogueManager>();
		_playerManager = FindAnyObjectByType<PlayerManager>();
		_playerMovementController = FindAnyObjectByType<PlayerMovementController>();
		_sceneController = FindAnyObjectByType<SceneController>();


		CreateEvent("7_1_PeterWantsToPlay", DialogueEvent.OnDialogueEvent.START_NODE, LookAtFirstDialoguePoint);

		CreateEvent("7_2_PeterStealsBall", DialogueEvent.OnDialogueEvent.END_NODE, StartPeterEndWait);
		CreateEvent("7_3_1_PeterWaits", DialogueEvent.OnDialogueEvent.END_NODE, StartPeterEndWait);

		CreateEvent("7_3_2_PeterEndWait", DialogueEvent.OnDialogueEvent.END_NODE, StartPeterPeterFalls);

		CreateEvent("7_5_NoTalkAboutBlood", DialogueEvent.OnDialogueEvent.END_NODE, StartCleanWound);		
	}

	async UniTask LookAtFirstDialoguePoint()
	{
		await _playerManager.CameraLookAt(focusFirstDialogue, playerLooksThereToo: true);
	}

	async UniTask StartPeterEndWait()
	{
		_dialogueManager.DialogueToStart = PeterEndWait;
		_dialogueManager.StartDialogue();
	}

	async UniTask StartPeterPeterFalls()
	{
		await UniTask.Delay(1500);
		_dialogueManager.DialogueToStart = PeterFalls;
		_dialogueManager.StartDialogue();
	}

	async UniTask StartCleanWound()
	{
		_dialogueManager.DialogueToStart = CleanWound;
		_dialogueManager.StartDialogue();
	}

}
