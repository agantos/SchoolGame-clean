using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;

public class CinemachineCameraChanger : MonoBehaviour
{
    CinemachineCamera[] _cameras;
	CinemachineCamera[] _camerasOrdered;

	CinemachineCamera _activeCamera;
	private void Start()
	{
		_cameras = FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		_camerasOrdered = _cameras;

		foreach (var cam in _cameras)
		{
			if (cam.Priority >= 0) {
                _camerasOrdered[cam.Priority] = cam;
            }
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
