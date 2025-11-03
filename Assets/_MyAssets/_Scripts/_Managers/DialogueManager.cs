using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public SCR_DialogueNode DialogueToStart;

    SCR_DialogueNode currentNode;

    public DialogueEventPlanner_Base EventPlanner;
    [SerializeField] DialogueUI dialogueUI;

    PlayerMovementController _playerMovementController;

    int _currentStep = 0;

    private void Start()
    {
        _playerMovementController = FindAnyObjectByType<PlayerMovementController>();
    }

    public async void StartDialogue()
    {
        if (DialogueToStart != null) {
            _playerMovementController.DisableMovement();
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
        }

        Update_Dialog_AudioVisuals();
        if(EventPlanner!=null) await EventPlanner.OnNodeStart(currentNode);
    }

    public async void CloseDialogue()
    {
        var node = currentNode;
        currentNode = null;
        DialogueToStart = null;

        dialogueUI.gameObject.SetActive(false);

        _playerMovementController.EnableMovement();
        if (EventPlanner != null) await EventPlanner.OnNodeEnd(node);

    }

    bool waitingOnAutomatic = false;

    async UniTask AutomaticNext()
    {
        int delay = (int)MathF.Ceiling(currentNode.steps[_currentStep].nextDelay * 1000);

        waitingOnAutomatic = true;
        await UniTask.Delay(delay);
		if (EventPlanner != null) await EventPlanner.OnStep(currentNode, _currentStep);
		waitingOnAutomatic = false;



		// Last Step
		if (_currentStep == currentNode.steps.Count - 1)
        {
			return;
        }

		_currentStep++;

        var step = currentNode.steps[_currentStep];
        Update_Dialog_AudioVisuals();

    }

    public async UniTask OnNextPressed()
    {
        if (waitingOnAutomatic) return;

		if (EventPlanner != null) await EventPlanner.OnStep(currentNode, _currentStep);
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
        var nextNode = currentNode.options[0].nextNode;

        _playerMovementController.AsyncMovementState_SetUP();

        if (EventPlanner != null) await EventPlanner.OnNodeOptionAPick(currentNode);

        if (nextNode)
        {
            await OpenNode(nextNode);
        }
        else
        {
            _playerMovementController.EnableMovement_AsyncFearSafe();
        }
    }

    public async void OnOptionBPicked()
    {
		dialogueUI.gameObject.SetActive(false);
        var nextNode = currentNode.options[1].nextNode;
        _playerMovementController.AsyncMovementState_SetUP();


        if (EventPlanner != null) await EventPlanner.OnNodeOptionBPick(currentNode);

        if (nextNode)
        {
            await OpenNode(nextNode);
        }
        else
        {
            _playerMovementController.EnableMovement_AsyncFearSafe();
        }
    }

    public void Update_Dialog_AudioVisuals()
    {
        var step = currentNode.steps[_currentStep];

        // Update View
        if (_currentStep == currentNode.steps.Count - 1)
        {
            if (currentNode.options.Length == 2)
            {
				dialogueUI.UpdateTextView(step.stepText, step.speakerName, currentNode.options[0].optionText, currentNode.options[1].optionText, step.clip);
                dialogueUI.SetView(step.dialogueView);
			}
			else if(currentNode.options.Length == 1)
            {
				dialogueUI.UpdateTextView(step.stepText, step.speakerName, currentNode.options[0].optionText, null, step.clip);
				dialogueUI.SetView(step.dialogueView);
			}
			else
            {
				dialogueUI.UpdateTextView(step.stepText, step.speakerName, null, null, step.clip);
				dialogueUI.SetView(step.dialogueView);
			}

			dialogueUI.PlayAudio(step.clip);
        }
        else
        {
            dialogueUI.UpdateTextView(step.stepText, step.speakerName, null, null, step.clip);
			dialogueUI.SetView(step.dialogueView);
			dialogueUI.PlayAudio(step.clip);
        }
    }
}
