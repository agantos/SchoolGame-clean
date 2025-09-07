using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

public class DialogueEventPlanner_1 : DialogueEventPlanner_Base
{

    [SerializeField] private CinemachineCameraTransition toClassroom;
	[SerializeField] private CinemachineCameraTransition toSeat;
	[SerializeField] private CinemachineCameraTransition toTablet;

    [SerializeField] private DoorAnimationController doorController;

    [SerializeField] private TabletAnimationController tabletController;
    [SerializeField] private VideoClip dinosaurVideo;


	private void Start()
    {
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
        toTablet.PerformTransitions();
        await UniTask.Delay(1000);

        await tabletController.SlideTabletOut();
        await UniTask.Delay(500);


        var tabletVideoPlayer = tabletController.gameObject.GetComponentInChildren<VideoPlayer>();
        tabletVideoPlayer.clip = dinosaurVideo;
        tabletVideoPlayer.Play();
    }

    async UniTask EnterClassroom()
    {
        await doorController.OpenDoor();
		await UniTask.Delay(500);
        toClassroom.PerformTransitions();
        await UniTask.Delay(2000);
	}

    void PrintOptionA()
    {
        Debug.Log("Picked A");
    }

    void PrintOptionB()
    {
        Debug.Log("Picked B");
    }

    void StartEvent()
    {
        Debug.Log("StartedDialogue");
    }

    void BadEnding()
    {
        Debug.Log("Ended a Node");
    }

    void GoodEnding()
    {
        Debug.Log("Ended a Node");
    }
}
