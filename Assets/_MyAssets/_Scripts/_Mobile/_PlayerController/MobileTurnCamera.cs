using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Unity.Cinemachine;

public class MobileTurnCamera : MonoBehaviour
{
	private InputSystem_Actions.PlayerActions _playerActions;

	[SerializeField] private CinemachineCamera virtualCam;

    [SerializeField] private Transform player;
    [SerializeField] private Transform playerModel;

	public float sensitivity = 0.05f;

	private CinemachinePanTilt panTilt;

	private void Start()
	{
		_playerActions = FindAnyObjectByType<InputManager>().playerActions;
		panTilt = virtualCam.GetComponent<CinemachinePanTilt>();
	}

    public void Enable()
    {
        SetCameraAndPlayerRotation(transform.rotation);

        if (panTilt != null)
            panTilt.enabled = true;
    }

    public void Disable()
    {
        if (panTilt != null)
            panTilt.enabled = false;
    }

	public void ProvideRotation()
	{
		Vector2 lookDelta = _playerActions.Look.ReadValue<Vector2>();

		transform.position = player.position + new Vector3(0f, playerModel.localScale.y / 1.5f, 0f);

		if (lookDelta.sqrMagnitude > 0.001f && !IsPointerOverUI())
		{
			panTilt.PanAxis.Value += lookDelta.x * sensitivity;
			panTilt.TiltAxis.Value -= lookDelta.y * sensitivity;
		}

		player.rotation = Quaternion.Euler(0f, panTilt.PanAxis.Value, 0f);
	}

	private bool IsPointerOverUI()
	{
		if (Touchscreen.current == null || EventSystem.current == null)
			return false;

		foreach (var touch in Touchscreen.current.touches)
		{
			if (touch.press.isPressed)
			{
				int fingerId = touch.touchId.ReadValue();
				if (EventSystem.current.IsPointerOverGameObject(fingerId))
					return true;
			}
		}
		return false;
	}

	public void SetCameraAndPlayerRotation(Quaternion worldRotation)
    {
        if (panTilt == null) panTilt = virtualCam.GetComponent<CinemachinePanTilt>();
        if (panTilt == null) return;

        // Take the forward vector of the given rotation
        Vector3 fwd = worldRotation * Vector3.forward;

        // Pan (yaw) around Y
        float pan = Mathf.Atan2(fwd.x, fwd.z) * Mathf.Rad2Deg;

        // Tilt (pitch)
        float horiz = new Vector2(fwd.x, fwd.z).magnitude;
        float pitchDeg = Mathf.Atan2(fwd.y, horiz) * Mathf.Rad2Deg;

        // Depending on axis setup, you might need to remove the minus
        float tilt = -pitchDeg;

        panTilt.PanAxis.Value = pan;
        panTilt.TiltAxis.Value = tilt;

        // Keep the player facing same direction as pan
        player.rotation = Quaternion.Euler(0f, pan, 0f);
    }

    public void SetRotation(Vector3 eulerAngles) => SetCameraAndPlayerRotation(Quaternion.Euler(eulerAngles));
}
