using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] float maxSpeed = 5.0f;
    [SerializeField] Transform player;
    [SerializeField] Transform cameraPivot;


    Rigidbody _rb;
    MobileJoystick _joystick;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _joystick = FindAnyObjectByType<MobileJoystick>(FindObjectsInactive.Include);
    }

    private void FixedUpdate()
    {
        Move(_joystick.GetDirection());
    }

    private void Update()
    {

    }

    void Move(Vector3 Direction)
    {
        Vector3 moveDir = transform.TransformDirection(Direction);
        _rb.linearVelocity = moveDir * maxSpeed;
    }
}
