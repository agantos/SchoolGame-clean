using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MobileTurnCamera : MonoBehaviour
{
    private Transform _player;
    private Transform _cameraPivot;

    public float sensitivity = 0.2f;
    public float smoothTime = 0.05f;

    private float _pitch = 0f;
    private Vector2 _smoothVelocity;
    private Vector2 _currentLookDelta;

    private InputSystem_Actions.PlayerActions _playerActions;

    private void Start()
    {
        _playerActions = FindAnyObjectByType<InputManager>().playerActions;
    }

    

    private void Update()
    {
    }

    public void Initialize(Transform Player, Transform Camera)
    {
        _player = Player;
        _cameraPivot = Camera;
    }

    public void RotatePlayerAndCamera(float[] pointersToExclude = null ) {
        // Read raw look delta
        Vector2 lookDelta = _playerActions.Look.ReadValue<Vector2>();

        // Skip if no input
        if (lookDelta.sqrMagnitude < 0.001f)
            return;

        // Check all active touches
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed)
                {
                    int fingerId = touch.touchId.ReadValue();
                    Vector2 touchPos = touch.position.ReadValue();

                    if (EventSystem.current.IsPointerOverGameObject(fingerId))
                        return; // Do not rotate camera
                }
            }
        }

        // Smooth input to reduce jitter
        _currentLookDelta = Vector2.SmoothDamp(_currentLookDelta, lookDelta, ref _smoothVelocity, smoothTime);

        _player.Rotate(Vector3.up, _currentLookDelta.x * sensitivity);

        _pitch -= _currentLookDelta.y * sensitivity;
        _pitch = Mathf.Clamp(_pitch, -40f, 60f);

        _cameraPivot.localEulerAngles = new Vector3(_pitch, 0f, 0f);
    }
}
