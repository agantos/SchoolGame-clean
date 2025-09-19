using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

public class DialogueEventPlanner_1 : DialogueEventPlanner_Base
{

	#region Private Variables
	private TabletController_Minigame1 _minigame_1_Controller;
	private CinemachineCameraChanger _cameraChanger;
	private PlayerMovementController _playerMovementController;
	private PlayerManager _playerManager;
	private ScreenFader _screenFader;
	private SceneController _sceneController;
    #endregion

    #region Serializable Fields
    [Header("Camera Transitions")]
	[SerializeField] private CinemachineCameraTransition toClassroom;
	[SerializeField] private CinemachineCameraTransition toSeat;
	[SerializeField] private CinemachineCameraTransition toTablet;

	[Header("Animation Controllers")]
	[SerializeField] private DoorAnimationController doorController;
	[SerializeField] private TabletAnimationController tabletAnimationController;
	[SerializeField] private VideoClip dinosaurVideo;

	[Header("End Positions")]
	[SerializeField] private Transform endPosition;
	[SerializeField] private Transform endLookAt;
	#endregion

	#region

	#endregion


	private void Start()
    {
        _minigame_1_Controller = FindAnyObjectByType<TabletController_Minigame1>();
        _playerMovementController = FindAnyObjectByType<PlayerMovementController>();
        _cameraChanger = FindAnyObjectByType<CinemachineCameraChanger>();
        _screenFader = FindAnyObjectByType<ScreenFader>();
        _sceneController = FindAnyObjectByType<SceneController>();
        _playerManager = FindAnyObjectByType<PlayerManager>();

        CreateEvent("ToClassroom", DialogueEvent.OnDialogueEvent.OPTION_A, EnterClassroom);
		
        CreateEvent("SitInClass", DialogueEvent.OnDialogueEvent.OPTION_A, SitDown);
        CreateEvent("RaisedHand", DialogueEvent.OnDialogueEvent.OPTION_A, OpenTablet);
        CreateEvent("NotRaisedHand", DialogueEvent.OnDialogueEvent.OPTION_A, OpenTablet);
    }

    async UniTask SitDown()
    {
		await UniTask.Delay(100);
		toSeat.PerformTransitions();
		await UniTask.Delay(2500);

	}

    async UniTask OpenTablet()
    {

        await UniTask.Delay(200);
        await toTablet.PerformTransitions();
        await UniTask.Delay(300);

        await tabletAnimationController.SlideTabletOut();
        _minigame_1_Controller.StartGame();
		await UniTask.Delay(300);
        _playerMovementController.DisableMovement();

        _minigame_1_Controller.onGameCompleted += OnCompleteMinigame;
	}

    async UniTask OnCompleteMinigame()
    {
        await UniTask.Delay(300);
        tabletAnimationController.SlideTabletIn();
        await UniTask.Delay(1200);
		await toSeat.PerformTransitions();
        await UniTask.Delay(2000);

        // TransitionScene
        await _screenFader.FadeTransition(1.5f, 2f, 1.5f, "After a while...",
            callBack: async () => {
                _cameraChanger.TransitionBackToPlayerCamera();
				await UniTask.Delay(400);

				_playerManager.MoveToPosition(endPosition.position);
				await UniTask.Yield();
				_playerManager.LookImmediately(endLookAt);

				await _sceneController.LoadNextScene(); 
        });
    }

	async UniTask EnterClassroom()
    {
        await doorController.OpenDoor();
		await UniTask.Delay(500);
        toClassroom.PerformTransitions();
        await UniTask.Delay(2000);
	}
}
