using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogueEventPlanner8 : DialogueEventPlanner_Base
{
	DialogueManager _dialogueManager;
	PlayerManager _playerManager;
	PlayerMovementController _playerMovementController;
	SceneController _sceneController;

	[SerializeField] Transform Nick;
	[SerializeField] Transform MrsAnne;

	[SerializeField] SCR_DialogueNode MrsAnneCame;
	[SerializeField] SCR_DialogueNode Apology;

	private void Start()
	{
		_dialogueManager = FindAnyObjectByType<DialogueManager>();
		_playerManager = FindAnyObjectByType<PlayerManager>();
		_playerMovementController = FindAnyObjectByType<PlayerMovementController>();
		_sceneController = FindAnyObjectByType<SceneController>();

		CreateEvent("8_1_PushNick", DialogueEvent.OnDialogueEvent.STEP, NickFallsDown, 1);
		CreateEvent("8_1_PushNick", DialogueEvent.OnDialogueEvent.END_NODE, EnableMrsAnne);

		CreateEvent("8_3_NotApology", DialogueEvent.OnDialogueEvent.END_NODE, StartApology);
		CreateEvent("8_4_Apology", DialogueEvent.OnDialogueEvent.START_NODE, OnApology);
	}

	async UniTask EnableMrsAnne()
	{
		MrsAnne.gameObject.SetActive(true);
		
		await LookAtTeacher();

		_dialogueManager.DialogueToStart = MrsAnneCame;
		_dialogueManager.StartDialogue();
	}

	async UniTask StartApology()
	{
		_dialogueManager.DialogueToStart = Apology;
		_dialogueManager.StartDialogue();
	}

	async UniTask OnApology()
	{
		await _playerManager.CameraLookAt(Nick, playerLooksThereToo: true);

	}

	async UniTask LookAtTeacher()
	{
		await _playerManager.CameraLookAt(MrsAnne, playerLooksThereToo: true);
	}

	async UniTask NickFallsDown()
	{
		await _playerManager.CameraLookAt(Nick, playerLooksThereToo: true);
	}
}