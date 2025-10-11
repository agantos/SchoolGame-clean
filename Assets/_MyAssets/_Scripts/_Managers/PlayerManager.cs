using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] CinemachineBrain cameraBrain;
    [SerializeField] Transform playerBodyTransform;

	public PlayerMovementController PlayerMovementController { private set; get; }

    private CinemachineCamera activeCamera;
    private Quaternion _originalPlayerRotation;
    private Quaternion _originalCameraRotation;

    private void Start()
    {
        PlayerMovementController = FindAnyObjectByType<PlayerMovementController>();
    } 

    private void Update()
    {
    }

	#region Transform Methods
	public void MoveToPosition(Vector3 position)
	{
		PlayerMovementController.MoveImmediately(position);
	}

	public void SetRotation(Vector3 rotation)
	{
		PlayerMovementController.SetRotation(rotation);
	}

	#endregion


	#region Camera Methods

	public void LookImmediately(Transform lookAt)
    {
        Vector3 direction = lookAt.position - playerBodyTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        PlayerMovementController.SetRotation(targetRotation.eulerAngles);
    }

    public async UniTask CameraLookAt(
        Transform target,
        bool playerLooksThereToo = false,
        float minDuration = 0.5f,  // shortest possible rotation time
        float maxDuration = 1f   // longest possible rotation time
    )
    {
        activeCamera = cameraBrain.ActiveVirtualCamera as CinemachineCamera;

        if (activeCamera == null)
            return;

        _originalCameraRotation = activeCamera.transform.rotation;
        _originalPlayerRotation = playerBodyTransform.rotation;

        PlayerMovementController.DisableMovement();

        // --- Calculate target rotations ---
        Vector3 cameraToTarget = target.position - activeCamera.transform.position;
        Quaternion targetCameraRot = Quaternion.LookRotation(cameraToTarget);

        Vector3 playerToTarget = target.position - playerBodyTransform.position;
        playerToTarget.y = 0;
        Quaternion targetPlayerRot = Quaternion.LookRotation(playerToTarget);

        // --- Calculate angle difference ---
        float angle = Quaternion.Angle(_originalCameraRotation, targetCameraRot);

        // --- Map angle to duration ---
        // Scale duration proportionally: 0° = minDuration, 180° = maxDuration
        float t = Mathf.InverseLerp(0, 180, angle);
        float duration = Mathf.Lerp(minDuration, maxDuration, t);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            // --- Make camera follow player ---
            Vector3 cameraOffset = new Vector3(0f, playerBodyTransform.localScale.y / 1.5f, 0f);
            activeCamera.transform.position = playerBodyTransform.position + cameraOffset;

            // --- Smooth rotate camera ---
            activeCamera.transform.rotation = Quaternion.Slerp(
                _originalCameraRotation,
                targetCameraRot,
                progress
            );

            if (playerLooksThereToo)
            {
                playerBodyTransform.rotation = Quaternion.Slerp(
                    _originalPlayerRotation,
                    targetPlayerRot,
                    progress
                );
            }

            await UniTask.Yield();
        }

        // Snap to final rotations
        activeCamera.transform.rotation = targetCameraRot;
        if (playerLooksThereToo)
            playerBodyTransform.rotation = targetPlayerRot;
    }

    public async UniTask ReturnToOriginalPlayerView(
        bool returnMovement = true,
        float minDuration = 0.25f,   // shortest rotation time
        float maxDuration = 0.75f    // longest rotation time
    )
    {
        PlayerMovementController.DisableMovement();

        if (activeCamera == null)
            return;

        // --- Calculate angle differences ---
        float cameraAngle = Quaternion.Angle(activeCamera.transform.rotation, _originalCameraRotation);
        float playerAngle = Quaternion.Angle(playerBodyTransform.rotation, _originalPlayerRotation);

        // Take the larger angle as the basis
        float maxAngle = Mathf.Max(cameraAngle, playerAngle);

        // --- Map angle to duration ---
        float t = Mathf.InverseLerp(0, 180, maxAngle);
        float duration = Mathf.Lerp(minDuration, maxDuration, t);

        float elapsed = 0f;

        // Store starting points
        Quaternion startCameraRot = activeCamera.transform.rotation;
        Quaternion startPlayerRot = playerBodyTransform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            // Smoothly rotate camera & player back to original
            activeCamera.transform.rotation = Quaternion.Slerp(startCameraRot, _originalCameraRotation, progress);
            playerBodyTransform.rotation = Quaternion.Slerp(startPlayerRot, _originalPlayerRotation, progress);

            await UniTask.Yield();
        }

        // Snap to exact originals at the end
        activeCamera.transform.rotation = _originalCameraRotation;
        playerBodyTransform.rotation = _originalPlayerRotation;

        if (returnMovement)
            PlayerMovementController.EnableMovement();
    }

	#endregion

	#region Grab Release Item
	[SerializeField] Transform heldItemPosition;
	[SerializeField] float grabDuration = 0.4f;
	[SerializeField] Ease grabEase = Ease.OutCubic;

    public GameObject grabbedObject;

	public void GrabItem(GameObject obj)
	{
		// Stop any running tweens on this object (safety)
		obj.transform.DOKill();

		// Disable physics for smoother control
		var rb = obj.GetComponent<Rigidbody>();
        if (rb == null) rb = obj.GetComponentInChildren<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = true;
			rb.angularVelocity = Vector3.zero;
		}

		// Parent it to nothing during flight, to keep world motion smooth
		obj.transform.SetParent(null);

		// --- Animate position + rotation + scale ---
		Sequence grabSeq = DOTween.Sequence();

		grabSeq.Append(
			obj.transform.DOMove(heldItemPosition.position, grabDuration)
				.SetEase(grabEase)
		)
		.Join(
			obj.transform.DORotate(heldItemPosition.rotation.eulerAngles, grabDuration)
				.SetEase(grabEase)
		)
		.Join(
			obj.transform.DOScale(1f, grabDuration * 0.8f).SetEase(Ease.OutBack)
		)
		.OnComplete(() =>
		{
			// Snap parent to hand position when done
			obj.transform.SetParent(heldItemPosition);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;

			grabbedObject = obj;
		});	
	}

	public void ReleaseItem(GameObject obj) {
		var rb = obj.GetComponent<Rigidbody>();
		if (rb == null) rb = obj.GetComponentInChildren<Rigidbody>();
		
        obj.transform.SetParent(null);


		if (rb != null)
		{
			rb.isKinematic = false;
			rb.angularVelocity = Vector3.zero;
		}

        grabbedObject = null;
	}

	#endregion

}
