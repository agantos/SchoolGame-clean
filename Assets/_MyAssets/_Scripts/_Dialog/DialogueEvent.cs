using UnityEngine;
using UnityEngine.Events;

public class DialogueEvent
{
    public UnityEvent eventToCall;
    public string NodeTag;
    public OnDialogueEvent callTime;
    
    public enum OnDialogueEvent
    {
        START_NODE,
        END_NODE,
        OPTION_A,
        OPTION_B,
    }
}
