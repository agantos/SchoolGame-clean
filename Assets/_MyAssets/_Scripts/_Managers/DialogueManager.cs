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
        currentNode = node;
        _currentStep = 0;

        if (dialogueUI.gameObject.activeSelf == false)
        {
            dialogueUI.gameObject.SetActive(true);
            _inputManager.playerActions.Disable();
        }

        UpdateView();
        _eventPlanner.OnNodeStart(currentNode);
    }

    public void CloseDialogue()
    {
        _eventPlanner.OnNodeEnd(currentNode);
        currentNode = null;
        dialogueUI.gameObject.SetActive(false);

        _inputManager.playerActions.Enable();
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
            UpdateView();
        }

        // Last Step
        if (_currentStep == currentNode.steps.Count - 1)
        {
            UpdateView();
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

    public void UpdateView()
    {
        var step = currentNode.steps[_currentStep];

        // Update View
        if (currentNode.options.Length > 0 && _currentStep == currentNode.steps.Count - 1)
        {
            dialogueUI.UpdateTextView(step.stepText, step.speakerName, currentNode.options[0].optionText, currentNode.options[1].optionText);
        }
        else
        {
            dialogueUI.UpdateTextView(step.stepText, step.speakerName, null, null);
        }
    }
}
