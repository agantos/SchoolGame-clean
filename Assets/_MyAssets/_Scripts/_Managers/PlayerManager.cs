using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] CinemachineBrain cameraBrain;
    [SerializeField] CinemachineCamera focusOnObjectCamera;

    [SerializeField] Transform playerBodyTransform;

    PlayerMovementController _playerMovementController;
    private void Start()
    {
        _playerMovementController = FindAnyObjectByType<PlayerMovementController>();
    }

    public async UniTask LookAt(Transform target, bool keepFocus = false)
    {
        var activeCamera = cameraBrain.ActiveVirtualCamera as CinemachineCamera;
        if (activeCamera == null) return;

        // 1. Freeze player movement
        _playerMovementController.DisableMovement();

        // 2. Sync starting position/rotation (optional, for smooth blending)
        focusOnObjectCamera.transform.position = activeCamera.transform.position;
        focusOnObjectCamera.transform.rotation = activeCamera.transform.rotation;

        // 3. Assign target for Hard Look At
        focusOnObjectCamera.LookAt = target;

        // 4. Blend by adjusting priority
        int oldPriority = activeCamera.Priority;
        focusOnObjectCamera.Priority = oldPriority + 1;

        // 5. Wait until blending is done
        await UniTask.WaitUntil(() => !cameraBrain.IsBlending);

        // Keep looking at target if requested
        if (keepFocus)
        {
            var playerCam = activeCamera.GetComponent<MobileTurnCamera>();
            if (playerCam != null)
            {
                playerCam.SetRotation(focusOnObjectCamera.transform.rotation);
            }
        }
    }

    public async UniTask ReturnToPlayer()
    {
        var activeCamera = cameraBrain.ActiveVirtualCamera as CinemachineCamera;
        if (activeCamera == null) return;

        // Lower focus camera priority so player cam becomes active again
        focusOnObjectCamera.Priority = -1;

        await UniTask.WaitUntil(() => !cameraBrain.IsBlending);

        // Restore movement
        _playerMovementController.EnableMovement();
    }
}
