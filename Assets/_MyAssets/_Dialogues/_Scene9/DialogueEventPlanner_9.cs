using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Video;

public class DialogueEventPlanner_9 : DialogueEventPlanner_Base
{
	#region Private Variables
	private CinemachineCameraChanger _cameraChanger;
	private PlayerMovementController _playerMovementController;
	private PlayerManager _playerManager;
	private ScreenFader _screenFader;
	private SceneController _sceneController;
	private TabletController_ColorsMinigame _colorsMinigame;
	private DialogueManager _dialogueManager;
	private ProjectorController _projectorController;
	private RecyclingMinigameManager _recyclingMinigameManager;

	#endregion

	#region Serializable Fields
	[Header("Camera Transitions")]
	[SerializeField] private CinemachineCamera seatCamera;
	[SerializeField] private CinemachineCamera tabletCamera;

	[Header("Animation Controllers")]
	[SerializeField] private TabletAnimationController tabletAnimationController;
	[SerializeField] private VideoClip colorsVideo;
	[SerializeField] private VideoClip recycleVideo;


	[Header("Dialogs")]
	[SerializeField] private SCR_DialogueNode startColorsGame;
	[SerializeField] private SCR_DialogueNode startRecycleVideo;
	[SerializeField] private SCR_DialogueNode startRecycleGame;



	#endregion

	private void Start()
	{
		_playerMovementController = FindAnyObjectByType<PlayerMovementController>();
		_cameraChanger = FindAnyObjectByType<CinemachineCameraChanger>();
		_screenFader = FindAnyObjectByType<ScreenFader>();
		_sceneController = FindAnyObjectByType<SceneController>();
		_playerManager = FindAnyObjectByType<PlayerManager>();		 
		_dialogueManager = FindAnyObjectByType<DialogueManager>();
		_colorsMinigame = FindAnyObjectByType<TabletController_ColorsMinigame>();
		_recyclingMinigameManager = FindAnyObjectByType<RecyclingMinigameManager>();

		_projectorController = FindAnyObjectByType<ProjectorController>();

		CreateEvent("EnterClassroom", DialogueEvent.OnDialogueEvent.OPTION_A, SitDown);
		CreateEvent("StartColorVideo", DialogueEvent.OnDialogueEvent.END_NODE, StartColorsGame);
		CreateEvent("WatchRecycleVideo", DialogueEvent.OnDialogueEvent.OPTION_A, WatchRecycleVideo);
		CreateEvent("StartRecycleGame", DialogueEvent.OnDialogueEvent.END_NODE, StartRecycleGame);
	}



	async UniTask SitDown()
	{
		_playerManager.PlayerMovementController.DisableMovement();

		await _cameraChanger.TransitionToCam(seatCamera);
		await UniTask.Delay(2000);

		_dialogueManager.DialogueToStart = startColorsGame;
		_dialogueManager.StartDialogue();

	}

	async UniTask StartColorsGame()
	{
		_playerManager.PlayerMovementController.DisableMovement();

		await UniTask.Delay(500);

		_projectorController.OpenProjectorVideo(colorsVideo);
		_projectorController.CleanOnCloseEvent();
		_projectorController.onProjectorClosed += async () =>
		{
			_colorsMinigame.OnEnd = EndColoursGame;
			await _cameraChanger.TransitionToCam(seatCamera);
			await UniTask.Delay(1500);
			await _cameraChanger.TransitionToCam(tabletCamera);
			await tabletAnimationController.SlideTabletOut();
			_colorsMinigame.StartGame();
		};
	
	}

	async UniTask WatchRecycleVideo()
	{
		_projectorController.OpenProjectorVideo(recycleVideo);
		_projectorController.CleanOnCloseEvent();
		_projectorController.onProjectorClosed += async () =>
		{
			await _cameraChanger.TransitionBackToPlayerCamera();

			//Start Recycling Game
			await UniTask.Delay(1000);
			_dialogueManager.DialogueToStart = startRecycleGame;
			_dialogueManager.StartDialogue();
		};
	}

	async UniTask StartRecycleGame()
	{
		_recyclingMinigameManager.StartRecyclingGame();
	}


	async UniTask EndColoursGame()
	{
		await tabletAnimationController.SlideTabletIn();
		await _cameraChanger.TransitionToCam(seatCamera);
		await _screenFader.PerformFadeTransition(1f, 5f, 1f, "The lesson continues until...", callBack: () => {  _ =_cameraChanger.TransitionBackToPlayerCamera(); });
		_playerManager.PlayerMovementController.EnableMovement();

		//Start Recycling Dialog
		await UniTask.Delay(5000);
		_dialogueManager.DialogueToStart = startRecycleVideo;
		_dialogueManager.StartDialogue();

	}

}
