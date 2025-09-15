using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{

    #region VARIABLES
    [SerializeField] float maxSpeed = 5.0f;
    [SerializeField] Transform player;
    [SerializeField] Transform cameraPivot;


    Rigidbody _rb;

    MobileJoystick _joystick;
    bool _joystickDisabled;

    MobileTurnCamera _mobileTurnCamera;
    bool _turnCameraDisabled;

    InputManager _inputManager;
    #endregion

    #region MONOBEHAVIOURS
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _joystick = FindAnyObjectByType<MobileJoystick>(FindObjectsInactive.Include);
        _mobileTurnCamera = FindAnyObjectByType<MobileTurnCamera>(FindObjectsInactive.Include);
        _inputManager = FindAnyObjectByType<InputManager>();
    }

    private void FixedUpdate()
    {
        if (_moveImmediatellyToken)
        {
            _rb.position = _positionToMove;
            _positionToMove = Vector3.zero;
            _moveImmediatellyToken = false;
            return;
        }
        if (!_joystickDisabled) MoveTowards(_joystick.GetDirection());
    }

    private void Update()
    {
        if (!_turnCameraDisabled) _mobileTurnCamera.ProvideRotation();
    }
    #endregion

    #region INSTANT TRANSFORMATIONS
    bool _moveImmediatellyToken;
    Vector3 _positionToMove;

    public void MoveImmediately(Vector3 Direction)
    {
        _moveImmediatellyToken = true;
        _positionToMove = Direction;
    }

    public void MoveTowards(Vector3 Direction)
    {
        Vector3 moveDir = transform.TransformDirection(Direction);
        _rb.linearVelocity = moveDir * maxSpeed;
    }

    public void SetRotation(Vector3 rotation)
    {
        _mobileTurnCamera.SetRotation(rotation);
    }
    #endregion

    #region ENABLE DISABLE MOVEMENT
    public void DisableMovement()
    {
        _joystick.Disable();
        _turnCameraDisabled = true;
        _mobileTurnCamera.Disable();

        _joystickDisabled = true;
        _inputManager.playerActions.Disable();
    }

    public void EnableMovement()
    {

        _enableStateChanged = true;

        _joystickDisabled = false;
        _turnCameraDisabled = false;
        _mobileTurnCamera.Enable();

        _joystick.Enable();
        _inputManager.playerActions.Enable();
    }

    bool _enableStateChanged;

    public void AsyncMovementState_SetUP()
    {
        _enableStateChanged = false;
    }

    // Like Enable movement but if enable state has been changed since it setup it does not change the value
    public void EnableMovement_AsyncFearSafe()
    {
        if (!_enableStateChanged)
        {
            _joystickDisabled = false;
            _turnCameraDisabled = false;
            _mobileTurnCamera.Enable();

            _joystick.Enable();
            _inputManager.playerActions.Enable();
        }
    }

    #endregion




}
