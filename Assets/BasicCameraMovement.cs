using UnityEngine;
// Make sure to add this line to use the new Input System!
using UnityEngine.InputSystem;

/// <summary>
/// A basic first-person-style camera controller using the new Input System.
/// Allows camera movement with WASDQE keys and rotation with the mouse.
/// </summary>
public class BasicCameraMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    [Tooltip("The regular speed at which the camera moves.")]
    public float moveSpeed = 5.0f;

    [Tooltip("The faster speed when holding down the 'Left Shift' key.")]
    public float fastMoveSpeed = 15.0f;

    [Header("Mouse Look")]
    [Tooltip("How sensitive the mouse is for looking around.")]
    public float mouseSensitivity = 2.0f;

    [Tooltip("The minimum vertical angle (pitch) the camera can look.")]
    public float verticalPitchMin = -85.0f;

    [Tooltip("The maximum vertical angle (pitch) the camera can look.")]
    public float verticalPitchMax = 85.0f;

    // Internal variables to store rotation values
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    void Start()
    {
        // Lock the cursor to the center of the screen and hide it for a seamless experience.
        // Press 'Esc' to unlock it during gameplay.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize rotation angles from the camera's starting orientation
        Vector3 initialEulerAngles = transform.eulerAngles;
        yaw = initialEulerAngles.y;
        pitch = initialEulerAngles.x;
    }

    void Update()
    {
        // Check if required devices are present
        if (Keyboard.current == null || Mouse.current == null)
        {
            // If no keyboard or mouse is present, do not run the update logic.
            // This prevents errors when the devices are not connected.
            return;
        }

        // --- Camera Rotation (Mouse Look) ---
        // Get mouse delta input from the new Input System.
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        yaw += mouseDelta.x * mouseSensitivity * Time.deltaTime * 50; // Multiplying by a factor to make sensitivity feel similar to old system
        pitch -= mouseDelta.y * mouseSensitivity * Time.deltaTime * 50;

        // Clamp the vertical pitch to prevent the camera from flipping over.
        pitch = Mathf.Clamp(pitch, verticalPitchMin, verticalPitchMax);

        // Apply the rotation to the camera transform.
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);


        // --- Camera Movement (Keyboard) ---
        // Determine the current speed based on whether 'Left Shift' is pressed.
        float currentSpeed = Keyboard.current.leftShiftKey.isPressed ? fastMoveSpeed : moveSpeed;

        // Get keyboard input for movement.
        Vector3 moveInput = Vector3.zero;
        if (Keyboard.current.wKey.isPressed)
        {
            moveInput.z += 1;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            moveInput.z -= 1;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            moveInput.x += 1;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            moveInput.x -= 1;
        }

        // Calculate the forward/backward and left/right movement vectors based on camera's orientation.
        Vector3 forwardMovement = transform.forward * moveInput.z;
        Vector3 rightMovement = transform.right * moveInput.x;

        // Combine movement vectors and normalize to prevent faster diagonal movement.
        Vector3 moveDirection = (forwardMovement + rightMovement).normalized;

        // Apply movement based on the direction and speed.
        transform.position += moveDirection * currentSpeed * Time.deltaTime;

        // Handle Up/Down movement.
        if (Keyboard.current.eKey.isPressed)
        {
            transform.position += Vector3.up * currentSpeed * Time.deltaTime;
        }
        if (Keyboard.current.qKey.isPressed)
        {
            transform.position += Vector3.down * currentSpeed * Time.deltaTime;
        }

        // --- Cursor Lock Management ---
        // Allow the user to unlock the cursor by pressing the Escape key.
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Allow the user to re-lock the cursor by clicking the left mouse button.
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
