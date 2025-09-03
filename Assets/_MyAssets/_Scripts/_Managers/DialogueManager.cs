using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public SCR_DialogueNode DialogueToStart;

    SCR_DialogueNode currentNode;
    SCR_DialogueNode _nextNode;

    [SerializeField] DialogueEventPlanner_Base _eventPlanner;
    [SerializeField] DialogueUI dialogueUI;

    InputManager _inputManager;

    int _currentStep = 0;

    private void Start()
    {
        _inputManager = FindAnyObjectByType<InputManager>();
    }

    public void StartDialogue()
    {
        if (DialogueToStart != null) { 
            OpenNode(DialogueToStart);
        }
    }

    void OpenNode(SCR_DialogueNode node)
    {
        // We reset the current vaiables
        dialogueUI.ClearOnStepFinished();
        currentNode = node;
        _currentStep = 0;

        // Set Dialog to change steps automatically
        if(currentNode.autoChangeSteps) dialogueUI.OnStepFinished_NodeMethods += AutomaticNext;

        if (dialogueUI.gameObject.activeSelf == false)
        {
            dialogueUI.gameObject.SetActive(true);
            _inputManager.playerActions.Disable();
        }

        Dialog_AudioVisual_Display();
        _eventPlanner.OnNodeStart(currentNode);
    }

    public void CloseDialogue()
    {
        _eventPlanner.OnNodeEnd(currentNode);
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
        Dialog_AudioVisual_Display();

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
            Dialog_AudioVisual_Display();
        }

        // Last Step
        if (_currentStep == currentNode.steps.Count - 1)
        {
            Dialog_AudioVisual_Display();
        }
    }

    public void OnOptionAPicked()
    {
        _eventPlanner.OnNodeOptionAPick(currentNode);

        var nextNode = currentNode.options[0].nextNode;

        if (nextNode)
        {
            OpenNode(nextNode);
        }
        else
        {
            dialogueUI.gameObject.SetActive(false);
        }
    }

    public void OnOptionBPicked()
    {
        _eventPlanner.OnNodeOptionBPick(currentNode);

        var nextNode = currentNode.options[1].nextNode;

        if (nextNode)
        {
            OpenNode(nextNode);
        }
        else
        {
            dialogueUI.gameObject.SetActive(false);
        }
    }

    public void Dialog_AudioVisual_Display()
    {
        var step = currentNode.steps[_currentStep];

        // Update View
        if (currentNode.options.Length > 0 && _currentStep == currentNode.steps.Count - 1)
        {
            dialogueUI.UpdateTextView(step.stepText, step.speakerName, currentNode.options[0].optionText, currentNode.options[1].optionText, step.clip);
            dialogueUI.PlayAudio(step.clip);
        }
        else
        {
            dialogueUI.UpdateTextView(step.stepText, step.speakerName, null, null, step.clip);
            dialogueUI.PlayAudio(step.clip);
        }
    }
}
