using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    SCR_DialogueNode currentNode;
    SCR_DialogueNode _nextNode;

    DialogueUI dialogueUI;

    int _currentStep = 0;

    public void StartNode()
    {

    }

    public void OnNext()
    {

    }

    public void ApplyStep(int index)
    {
        if (index >= currentNode.steps.Count) return;

        //Last Step
        if(index == currentNode.steps.Count - 1)
        {
            if (dialogueUI.gameObject.activeSelf == false)
                dialogueUI.gameObject.SetActive(true);

            var step = currentNode.steps[index];

            dialogueUI.SetSpeakerName(step.speakerName);
            dialogueUI.SetImages(step.leftSpeakerImage, step.rightSpeakerImage);
            
            if(currentNode.options.Length > 0)
            {
                dialogueUI.SetText(step.stepText, currentNode.options[0].optionText, currentNode.options[1].optionText);
            }
            else
            {
                dialogueUI.SetText(step.stepText);
            }
        }

        //First Or Middle Step
        else if (index < currentNode.steps.Count)
        {
            if(dialogueUI.gameObject.activeSelf == false)
                dialogueUI.gameObject.SetActive(true);

            var step = currentNode.steps[index];

            dialogueUI.SetText(step.stepText);
            dialogueUI.SetSpeakerName(step.speakerName);
            dialogueUI.SetImages(step.leftSpeakerImage, step.rightSpeakerImage);
        }        
    }
}
