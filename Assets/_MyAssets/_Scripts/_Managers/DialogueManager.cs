using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public SCR_DialogueNode DialogueToStart;

    SCR_DialogueNode currentNode;
    SCR_DialogueNode _nextNode;

    public DialogueEventPlanner_Base EventPlanner;
    [SerializeField] DialogueUI dialogueUI;

    InputManager _inputManager;

    int _currentStep = 0;

    private void Start()
    {
        _inputManager = FindAnyObjectByType<InputManager>();
    }

    public async void StartDialogue()
    {
        if (DialogueToStart != null) { 
            await OpenNode(DialogueToStart);
        }
    }

    async UniTask OpenNode(SCR_DialogueNode node)
    {
        // We reset the current vaiables
        dialogueUI.ClearOnStepFinished();
        dialogueUI.ResetView();

        currentNode = node;
        _currentStep = 0;

        // Set Dialog to change steps automatically
        if(currentNode.autoChangeSteps) dialogueUI.OnStepFinished_NodeMethods += AutomaticNext;

        if (dialogueUI.gameObject.activeSelf == false)
        {
            dialogueUI.gameObject.SetActive(true);
            _inputManager.playerActions.Disable();
        }

        Update_Dialog_AudioVisuals();
        await EventPlanner.OnNodeStart(currentNode);
    }

    public async void CloseDialogue()
    {
        await EventPlanner.OnNodeEnd(currentNode);
        currentNode = null;
        dialogueUI.gameObject.SetActive(false);

        _inputManager.playerActions.Enable();
    }

    void AutomaticNext()
    {
        // Last Step
        if (_currentStep == currentNode.steps.Count - 1)
        {
            return;
        }

        _currentStep++;

        var step = currentNode.steps[_currentStep];
        Update_Dialog_AudioVisuals();
    }

    public void OnNextPressed()
    {
		_currentStep++;

		if (_currentStep == currentNode.steps.Count)
		{
			CloseDialogue();
			return;
		}

		var step = currentNode.steps[_currentStep];

		// middle step
		if (_currentStep < currentNode.steps.Count)
		{
			Update_Dialog_AudioVisuals();
		}

		// Last Step
		if (_currentStep == currentNode.steps.Count - 1)
		{
			Update_Dialog_AudioVisuals();
		}
    }

    public async void OnOptionAPicked()
    {
		dialogueUI.gameObject.SetActive(false);

		await EventPlanner.OnNodeOptionAPick(currentNode);

		var nextNode = currentNode.options[0].nextNode;

        if (nextNode)
        {
            await OpenNode(nextNode);
        }
    }

    public async void OnOptionBPicked()
    {
		dialogueUI.gameObject.SetActive(false);

		await EventPlanner.OnNodeOptionBPick(currentNode);

        var nextNode = currentNode.options[1].nextNode;

        if (nextNode)
        {
            await OpenNode(nextNode);
        }
    }

    public void Update_Dialog_AudioVisuals()
    {
        var step = currentNode.steps[_currentStep];

        // Update View
        if (_currentStep == currentNode.steps.Count - 1)
        {
            if (currentNode.options.Length == 2)
                dialogueUI.UpdateTextView(step.stepText, step.speakerName, currentNode.options[0].optionText, currentNode.options[1].optionText, step.clip);
			else if(currentNode.options.Length == 1)
				dialogueUI.UpdateTextView(step.stepText, step.speakerName, currentNode.options[0].optionText, null, step.clip);
            else
            {
				dialogueUI.UpdateTextView(step.stepText, step.speakerName, null, null, step.clip);
			}

			dialogueUI.PlayAudio(step.clip);
        }
        else
        {
            dialogueUI.UpdateTextView(step.stepText, step.speakerName, null, null, step.clip);
            dialogueUI.PlayAudio(step.clip);
        }
    }
}
