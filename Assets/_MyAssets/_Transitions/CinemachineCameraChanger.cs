using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;

public class CinemachineCameraChanger : MonoBehaviour
{
    [SerializeField] CinemachineCamera[] cameras;
	[SerializeField] CinemachineCamera[] _camerasOrdered;

	CinemachineCamera _activeCamera;
	private void Start()
	{
		_camerasOrdered = cameras;

		foreach (var cam in cameras)
		{
			_camerasOrdered[cam.Priority] = cam;
		}

		_activeCamera = _camerasOrdered[0];
	}

	public void TransitionToCam(CinemachineCamera cam)
	{
		_activeCamera.gameObject.SetActive(false);
		_activeCamera = cam;
		_activeCamera.gameObject.SetActive(true);
	}
}
