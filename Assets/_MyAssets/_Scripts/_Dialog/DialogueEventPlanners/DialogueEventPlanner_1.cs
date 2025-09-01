using UnityEngine;

public class DialogueEventPlanner_1 : DialogueEventPlanner_Base
{

    private void Start()
    {
        CreateEvent("FirstNode", DialogueEvent.OnDialogueEvent.OPTION_A, PrintOptionA);
        CreateEvent("FirstNode", DialogueEvent.OnDialogueEvent.OPTION_B, PrintOptionB);
        CreateEvent("FirstNode", DialogueEvent.OnDialogueEvent.START_NODE, StartEvent);
        CreateEvent("BadEnding", DialogueEvent.OnDialogueEvent.END_NODE, BadEnding);
        CreateEvent("GoodEnding", DialogueEvent.OnDialogueEvent.END_NODE, GoodEnding);


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
