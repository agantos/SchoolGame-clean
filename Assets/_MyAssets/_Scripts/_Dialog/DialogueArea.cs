using UnityEngine;

public class DialogueArea : TriggerArea
{
    private RightSideButtonsHandler _rightSideButtonsHandler;
    private DialogueManager _dialogueManager;

    [SerializeField] private SCR_DialogueNode dialog;
	[SerializeField] private DialogueEventPlanner_Base eventPlanner;
    [SerializeField] private bool startOnTriggerEnter;

    private void Awake()
    {
        _rightSideButtonsHandler = FindAnyObjectByType<RightSideButtonsHandler>();
        _dialogueManager = FindAnyObjectByType<DialogueManager>();
	}

    protected override void OnPlayerEnter()
    {
        _dialogueManager.EventPlanner = eventPlanner;
		_dialogueManager.DialogueToStart = dialog;

        if (startOnTriggerEnter) {            
            _dialogueManager.StartDialogue();
            Disable();
        }
        else
        {
            _rightSideButtonsHandler?.ToggleDialogueButton(true);
        }
    }

    protected override void OnPlayerExit()
    {
        _rightSideButtonsHandler?.ToggleDialogueButton(false);
        _dialogueManager.DialogueToStart = null;
    }
}