using UnityEngine;

public class DialogueArea : MonoBehaviour
{
    private RightSideButtonsHandler _rightSideButtonsHandler;
    private DialogueManager _dialogueManager;

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private SCR_DialogueNode dialog;
	[SerializeField] private DialogueEventPlanner_Base eventPlanner;
    [SerializeField] private bool startOnTriggerEnter;

    private void Awake()
    {
        _rightSideButtonsHandler = FindAnyObjectByType<RightSideButtonsHandler>();
        _dialogueManager = FindAnyObjectByType<DialogueManager>();
        
        if(eventPlanner != null) _dialogueManager.EventPlanner = eventPlanner;
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

    void OutsidePlayerArea()
    {
        _rightSideButtonsHandler?.ToggleDialogueButton(false);
        _dialogueManager.DialogueToStart = null;
    }

    void Disable()
    {
        gameObject.GetComponent<SphereCollider>().enabled = false;  
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}