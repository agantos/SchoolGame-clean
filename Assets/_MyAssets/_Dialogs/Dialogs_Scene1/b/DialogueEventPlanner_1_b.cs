using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogueEventPlanner_1 : DialogueEventPlanner_Base
{

    [SerializeField] private CinemachineCameraTransition toClassroom;
	[SerializeField] private DoorAnimationController doorController;


	private void Start()
    {
        CreateEvent("ToClassroom", DialogueEvent.OnDialogueEvent.OPTION_A, EnterClassroom);

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
