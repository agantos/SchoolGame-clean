using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputSystem_Actions _inputActions;
    public InputSystem_Actions.PlayerActions playerActions;
    public InputSystem_Actions.UIActions UIActions;



    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _inputActions.Player.Enable();
        _inputActions.UI.Enable();

        playerActions = _inputActions.Player;
        UIActions = _inputActions.UI;
    }


    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.UI.Disable();

    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
        _inputActions.UI.Disable();
    }

    void Update()
    {
        
    }
}
