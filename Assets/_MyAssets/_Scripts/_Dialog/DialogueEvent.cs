using Cysharp.Threading.Tasks;
using System;

public class DialogueEvent
{
	public Func<UniTask> eventToCallAsync;
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