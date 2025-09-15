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


    public void TransitionBackToPlayerCamera()
	{
		if (_activeChangedCamera == null || _activeChangedCamera == playerCamera) return;

		playerCamera.gameObject.SetActive(true);
		_activeChangedCamera.gameObject.SetActive(false);
		_activeChangedCamera = playerCamera;
	}
}
