using UnityEngine;
using UnityEngine.InputSystem;

public class AssignInputUI : MonoBehaviour
{
    private void Start()
    {
        InputManager _inputManager = FindAnyObjectByType<InputManager>();
        DialogueManager _dialogueManager = FindAnyObjectByType<DialogueManager>();
    }
}
