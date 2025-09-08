using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] float maxSpeed = 5.0f;
    [SerializeField] Transform player;
    [SerializeField] Transform cameraPivot;


    Rigidbody _rb;
    MobileJoystick _joystick;
    InputManager _inputManager;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _joystick = FindAnyObjectByType<MobileJoystick>(FindObjectsInactive.Include);
        _inputManager = FindAnyObjectByType<InputManager>();
    }

    private void FixedUpdate()
    {        
        Move(_joystick.GetDirection());
    }

    private void Update()
    {

    }

    public void DisableMovement()
    {
        _joystick.gameObject.SetActive(false);
        _inputManager.playerActions.Disable();
    }

    public void EnableMovement() {
        _joystick.gameObject.SetActive(true);
        _inputManager.playerActions.Enable();
    }

    void Move(Vector3 Direction)
    {
        Vector3 moveDir = transform.TransformDirection(Direction);
        _rb.linearVelocity = moveDir * maxSpeed;
    }
}
