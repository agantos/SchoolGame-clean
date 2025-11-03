using Cysharp.Threading.Tasks;
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

    public async UniTask OnNodeStart(SCR_DialogueNode node)
    {
        if (events == null || !events.ContainsKey(node.tag)) return;

        foreach (var e in events[node.tag])
        {
            if (e.callTime == DialogueEvent.OnDialogueEvent.START_NODE)
            {
                await e.eventToCallAsync.Invoke();
            }
        }
    }

	public async UniTask OnNodeEnd(SCR_DialogueNode node)
	{
		if (!events.ContainsKey(node.tag)) return;

		foreach (var e in events[node.tag])
		{
			if (e.callTime == DialogueEvent.OnDialogueEvent.END_NODE && e.eventToCallAsync != null)
			{
				await e.eventToCallAsync.Invoke();
			}
		}
	}

	public async UniTask OnNodeOptionAPick(SCR_DialogueNode node)
	{
		if (!events.ContainsKey(node.tag)) return;

		foreach (var e in events[node.tag])
		{
			if (e.callTime == DialogueEvent.OnDialogueEvent.OPTION_A && e.eventToCallAsync != null)
			{
				await e.eventToCallAsync.Invoke();
			}
		}
	}
	public async UniTask OnNodeOptionBPick(SCR_DialogueNode node)
	{
		if (!events.ContainsKey(node.tag)) return;

		foreach (var e in events[node.tag])
		{
			if (e.callTime == DialogueEvent.OnDialogueEvent.OPTION_B && e.eventToCallAsync != null)
			{
				await e.eventToCallAsync.Invoke();
			}
		}
	}

	public async UniTask OnStep(SCR_DialogueNode node, int stepNum)
	{
		if (!events.ContainsKey(node.tag)) return;

		foreach (var e in events[node.tag])
		{
			if (e.stepToCall == stepNum && e.callTime == DialogueEvent.OnDialogueEvent.STEP && e.eventToCallAsync != null)
			{
				await e.eventToCallAsync.Invoke();
			}
		}
	}

	public void CreateEvent(string tag, DialogueEvent.OnDialogueEvent callTime, Func<UniTask> functionToInvoke, int step = 0)
    {
        if (string.IsNullOrEmpty(tag))
        {
            Debug.LogWarning("Attempted to create a DialogueEvent with an empty or null tag.");
            return;
        }

        DialogueEvent newEvent = new DialogueEvent
        {
            callTime = callTime,
            eventToCallAsync = functionToInvoke,
            NodeTag = tag
        };

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

