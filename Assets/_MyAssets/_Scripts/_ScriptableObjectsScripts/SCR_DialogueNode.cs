using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueNode", menuName = "ScriptableObjects/Dialogue/Node")]
public class SCR_DialogueNode : ScriptableObject
{
    [Header("Dialogue Steps")]
    [Tooltip("Sequential dialogue steps for this node.")]
    public List<DialogueStep> steps = new List<DialogueStep>();

    [Header("Player Options")]
    [Tooltip("Choices the player can select after this dialogue.")]
    public DialogueOption[] options;

    [Serializable]
    public class DialogueStep
    {
        [Header("Step Content")]
        [Tooltip("Text spoken in this step.")]
        [TextArea(2, 6)]
        public string stepText;

        [Tooltip("Name of the speaker.")]
        public string speakerName;

        [Space(5)]
        [Tooltip("Left-side portrait of the speaker.")]
        public Sprite leftSpeakerImage;

        [Tooltip("Right-side portrait of the speaker.")]
        public Sprite rightSpeakerImage;
    }

    [Serializable]
    public class DialogueOption
    {
        [Tooltip("Text for the player's choice.")]
        [TextArea(1, 3)]
        public string optionText;

        [Tooltip("Next dialogue node this option leads to.")]
        public SCR_DialogueNode nextNode;
    }
}
