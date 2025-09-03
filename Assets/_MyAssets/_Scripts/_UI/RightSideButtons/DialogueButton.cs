using UnityEngine;
using UnityEngine.UI;

public class DialogueButton : Button
{
    private DialogueManager _dialogueManager;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (_dialogueManager == null)
        {
            _dialogueManager = FindAnyObjectByType<DialogueManager>();
        }

        onClick.AddListener(OnClick);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        if (_dialogueManager != null)
        {
            _dialogueManager.StartDialogue();
        }
    }
}
