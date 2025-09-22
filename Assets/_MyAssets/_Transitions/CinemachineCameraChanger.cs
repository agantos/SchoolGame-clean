using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class CinemachineCameraChanger : MonoBehaviour
{
	[SerializeField] CinemachineBrain cinemachineBrain;

    [SerializeField] CinemachineCamera playerCamera;
	CinemachineCamera _activeChangedCamera;

    public async UniTask TransitionToCam(CinemachineCamera cam)
    {
        var activeCamera = cinemachineBrain.ActiveVirtualCamera as CinemachineCamera;
        if (activeCamera == null || cam == null || activeCamera == cam)
            return;

        // Disable current
        activeCamera.gameObject.SetActive(false);

        // Queue new active
        _activeChangedCamera = cam;
        cam.gameObject.SetActive(true);

        await UniTask.WaitUntil(() => cinemachineBrain.IsBlending);
        await UniTask.WaitWhile(() => cinemachineBrain.IsBlending);
    }

    public async UniTask TransitionThroughCams(params CinemachineCamera[] cams)
    {
        if (cams == null || cams.Length == 0)
            return;

        foreach (var cam in cams)
        {
            var activeCamera = cinemachineBrain.ActiveVirtualCamera as CinemachineCamera;
            if (activeCamera == null || cam == null || activeCamera == cam)
                continue;

            // Disable current
            activeCamera.gameObject.SetActive(false);

            // Queue new active
            _activeChangedCamera = cam;
            cam.gameObject.SetActive(true);

            // Wait until blending starts
            await UniTask.WaitUntil(() => cinemachineBrain.IsBlending);

            // Wait until blending finishes
            await UniTask.WaitWhile(() => cinemachineBrain.IsBlending);
        }
    }

    public void TransitionBackToPlayerCamera()
	{
		if (_activeChangedCamera == null || _activeChangedCamera == playerCamera) return;

		playerCamera.gameObject.SetActive(true);
		_activeChangedCamera.gameObject.SetActive(false);
		_activeChangedCamera = playerCamera;
	}

    public void PositionPlayerToActiveCamera()
    {
        PlayerManager playerManager = FindAnyObjectByType<PlayerManager>();
        var position = new Vector3(_activeChangedCamera.transform.position.x, 0, _activeChangedCamera.transform.position.z);

        playerManager.MoveToPosition(_activeChangedCamera.transform.position);
        playerManager.SetRotation(_activeChangedCamera.transform.rotation.eulerAngles);
    }
}
