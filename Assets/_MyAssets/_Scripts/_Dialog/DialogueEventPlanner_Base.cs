using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueEventPlanner_Base : MonoBehaviour
{
    Dictionary<string, List<DialogueEvent>> events;

    private void Awake()
    {
        events = new Dictionary<string, List<DialogueEvent>>();
    }

    public void OnNodeStart(SCR_DialogueNode node)
    {
        if (events == null || !events.ContainsKey(node.tag)) return;

        foreach (var e in events[node.tag])
        {
            if (e.callTime == DialogueEvent.OnDialogueEvent.START_NODE)
            {
                e.eventToCall.Invoke();
            }
        }
    }

    public void OnNodeEnd(SCR_DialogueNode node)
    {
        if (events == null || !events.ContainsKey(node.tag)) return;

        foreach (var e in events[node.tag])
        {
            if (e.callTime == DialogueEvent.OnDialogueEvent.END_NODE)
            {
                e.eventToCall.Invoke();
            }
        }
    }

    public void OnNodeOptionAPick(SCR_DialogueNode node)
    {
        if (events == null || !events.ContainsKey(node.tag)) return;

        foreach (var e in events[node.tag])
        {
            if (e.callTime == DialogueEvent.OnDialogueEvent.OPTION_A)
            {
                e.eventToCall.Invoke();
            }
        }
    }

    public void OnNodeOptionBPick(SCR_DialogueNode node)
    {
        if (events == null || !events.ContainsKey(node.tag)) return;

        foreach (var e in events[node.tag])
        {
            if (e.callTime == DialogueEvent.OnDialogueEvent.OPTION_B)
            {
                e.eventToCall.Invoke();
            }
        }
    }

    public void CreateEvent(string tag, DialogueEvent.OnDialogueEvent callTime, Action functionToInvoke)
    {
        if (string.IsNullOrEmpty(tag))
        {
            Debug.LogWarning("Attempted to create a DialogueEvent with an empty or null tag.");
            return;
        }

        // Build a new UnityEvent and attach the function
        UnityEvent unityEvent = new UnityEvent();
        if (functionToInvoke != null)
        {
            unityEvent.AddListener(() => functionToInvoke());
        }

        // Create the DialogueEvent object
        DialogueEvent newEvent = new DialogueEvent
        {
            callTime = callTime,
            eventToCall = unityEvent
        };

        // Register it in the dictionary
        if (events.ContainsKey(tag))
        {
            events[tag].Add(newEvent);
        }
        else
        {
            events.Add(tag, new List<DialogueEvent> { newEvent });
        }
    }
}

