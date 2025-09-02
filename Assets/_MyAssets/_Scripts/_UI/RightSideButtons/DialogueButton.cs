using UnityEngine;
using UnityEngine.UI;

public class DialogueButton : Button
{
    DialogueManager _dialogueManager;

    protected override void Start()
    {
        base.Start();
        _dialogueManager = FindAnyObjectByType<DialogueManager>();
        onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (_dialogueManager != null)
        {
            _dialogueManager.StartDialogue();
        }
    }
}
