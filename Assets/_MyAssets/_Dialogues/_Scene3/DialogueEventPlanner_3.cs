using UnityEngine;
using Cysharp.Threading.Tasks;


public class DialogueEventPlanner_3 : DialogueEventPlanner_Base
{
    DialogueManager _dialogueManager;
    ScreenFader _screenFader;
    PlayerManager _playerManager;
    CinemachineCameraChanger _cameraChanger;
    SceneController _sceneController;

    [SerializeField] Transform margaret;
    
    [SerializeField] Transform afterQueuePosition;
    [SerializeField] Transform afterQueueRotation;
    [SerializeField] SCR_DialogueNode afterQueueDialogue;

    [Header("Dialogues")]
    [SerializeField] SCR_DialogueNode notAskedDialogue;
    [SerializeField] SCR_DialogueNode notAskedConsequences;


	[Header("Dialogue Area")]
	[SerializeField] DialogueArea askDialogueArea;
	[SerializeField] DialogueArea MargaretEndDialog;
	private void Start()
    {
        _dialogueManager = FindAnyObjectByType<DialogueManager>();
        _playerManager = FindAnyObjectByType<PlayerManager>();
        _screenFader = FindAnyObjectByType<ScreenFader>();
        _cameraChanger = FindAnyObjectByType<CinemachineCameraChanger>();
        _sceneController = FindAnyObjectByType<SceneController>();

        CreateEvent("Polite_1", DialogueEvent.OnDialogueEvent.START_NODE, LookAtMargaret);
        CreateEvent("Polite_1", DialogueEvent.OnDialogueEvent.END_NODE, QueueTransitionPolite);

        CreateEvent("3_2_Rude", DialogueEvent.OnDialogueEvent.START_NODE, LookAtMargaret);
        CreateEvent("3_2_Rude", DialogueEvent.OnDialogueEvent.END_NODE, QueueTransitionRude);

        CreateEvent("3_6_NoAsk", DialogueEvent.OnDialogueEvent.END_NODE, NotAsked);
		CreateEvent("3_7_AskMilk", DialogueEvent.OnDialogueEvent.END_NODE, SpawnBiscuitDialogArea);

		CreateEvent("3_10_FinalDialogue", DialogueEvent.OnDialogueEvent.END_NODE, LoadScene4);
	}

	async UniTask SpawnBiscuitDialogArea()
    {
		MargaretEndDialog.gameObject.SetActive(true);

	}

	async UniTask NotAsked()
    {
        _playerManager.PlayerMovementController.EnableMovement();
        await UniTask.Delay(10000);

		_dialogueManager.DialogueToStart = notAskedConsequences;
		_dialogueManager.StartDialogue();
        askDialogueArea.gameObject.SetActive(true);
	}
    async UniTask QueueTransitionPolite()
    {
		await UniTask.Delay(1000);

		_playerManager.PlayerMovementController.DisableMovement();
		await _screenFader.PerformFadeTransition(5, 3.5f, 4, "A moment later...", callBack: PositionAndRotatePeter);
		_playerManager.PlayerMovementController.DisableMovement();

		_dialogueManager.DialogueToStart = afterQueueDialogue;
        _dialogueManager.StartDialogue();
    }

	async UniTask QueueTransitionRude()
	{
		await PlayNumbers();
		await UniTask.Delay(1000);
		
        _playerManager.PlayerMovementController.DisableMovement();
		await _screenFader.PerformFadeTransition(5, 3.5f, 4, "A moment later...", callBack: PositionAndRotatePeter);
		_playerManager.PlayerMovementController.DisableMovement();
		_dialogueManager.DialogueToStart = afterQueueDialogue;
		_dialogueManager.StartDialogue();
	}

	async UniTask PlayNumbers()
	{
		var playNumbers = FindAnyObjectByType<NumberAnimationController>();
		await playNumbers.StartAnimation(20);
	}

	async void PositionAndRotatePeter()
    {
		_playerManager.PlayerMovementController.EnableMovement();
		_playerManager.MoveToPosition(afterQueuePosition.transform.position);
		await UniTask.NextFrame();
		_playerManager.LookImmediately(afterQueueRotation);

	}

	async UniTask LookAtMargaret()
    {
        await _playerManager.CameraLookAt(margaret, playerLooksThereToo: true);
    }

    async UniTask LoadScene4()
    {
		await _sceneController.LoadNextScene();

	}
}
