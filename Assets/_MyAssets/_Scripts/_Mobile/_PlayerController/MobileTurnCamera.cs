using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Unity.Cinemachine;

public class MobileTurnCamera : MonoBehaviour
{
	private InputSystem_Actions.PlayerActions _playerActions;

	[SerializeField] private CinemachineCamera virtualCam;
	[SerializeField] private Transform player;

	public float sensitivity = 0.05f;

	private CinemachinePanTilt panTilt;

	private void Start()
	{
		_playerActions = FindAnyObjectByType<InputManager>().playerActions;
		panTilt = virtualCam.GetComponent<CinemachinePanTilt>();
	}

	private void Update()
	{
		Vector2 lookDelta = _playerActions.Look.ReadValue<Vector2>();

		transform.position = player.position + new Vector3(0f, player.position.y/2, 0f);

		if (lookDelta.sqrMagnitude < 0.001f)
			return;

		// Skip if finger is over UI
		if (Touchscreen.current != null)
		{
			foreach (var touch in Touchscreen.current.touches)
			{
				if (touch.press.isPressed)
				{
					int fingerId = touch.touchId.ReadValue();
					if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(fingerId))
						return;
				}
			}
		}

		panTilt.PanAxis.Value += lookDelta.x * sensitivity;

		panTilt.TiltAxis.Value -= lookDelta.y * sensitivity;


		player.rotation = Quaternion.Euler(0f, panTilt.PanAxis.Value, 0f);
	}
}
