using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

public class DialogueEventPlanner_1 : DialogueEventPlanner_Base
{

    [SerializeField] private CinemachineCameraTransition toClassroom;
	[SerializeField] private CinemachineCameraTransition toSeat;
	[SerializeField] private CinemachineCameraTransition toTablet;

    [SerializeField] private DoorAnimationController doorController;

    [SerializeField] private TabletAnimationController tabletAnimationController;
    private TabletController_Minigame1 _tabletController;

    [SerializeField] private VideoClip dinosaurVideo;

    private PlayerMovementController _playerMovementController;


	private void Start()
    {
        _tabletController = FindAnyObjectByType<TabletController_Minigame1>();
        _playerMovementController = FindAnyObjectByType<PlayerMovementController>();

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
        _tabletController.StartGame();
		await UniTask.Delay(300);
        _playerMovementController.DisableMovement();
	}

	async UniTask EnterClassroom()
    {
        await doorController.OpenDoor();
		await UniTask.Delay(500);
        toClassroom.PerformTransitions();
        await UniTask.Delay(2000);
	}
}
