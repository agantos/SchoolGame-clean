using UnityEngine;

public class DialogueArea : MonoBehaviour
{
    private RightSideButtonsHandler _rightSideButtonsHandler;
    private DialogueManager _dialogueManager;


    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private SCR_DialogueNode dialog;


    private void Awake()
    {
        _rightSideButtonsHandler = FindAnyObjectByType<RightSideButtonsHandler>();
        _dialogueManager = FindAnyObjectByType<DialogueManager>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other.gameObject))
        {
            PlayerInsideArea();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other.gameObject))
        {
            OutsidePlayerArea();
        }
    }

    private bool IsPlayer(GameObject obj)
    {
        return ((1 << obj.layer) & playerLayer) != 0;
    }

    void PlayerInsideArea()
    {
        _rightSideButtonsHandler?.ToggleDialogueButton(true);
        _dialogueManager.DialogueToStart = dialog;
    }

    void OutsidePlayerArea()
    {
        _rightSideButtonsHandler?.ToggleDialogueButton(false);
        _dialogueManager.DialogueToStart = null;
    }
}