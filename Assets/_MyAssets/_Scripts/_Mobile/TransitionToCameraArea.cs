using Cysharp.Threading.Tasks;
using System;
using Unity.Cinemachine;
using UnityEngine;

public class TransitionToCameraArea : TriggerArea
{
	private PlayerManager _playerManager;
	private CinemachineCameraChanger _cameraChanger;

	[SerializeField] private CinemachineCamera cam;

	public Func<UniTask> callback;

	private void Awake()
	{
		_playerManager = FindAnyObjectByType<PlayerManager>();
		_cameraChanger = FindAnyObjectByType<CinemachineCameraChanger>();
	}

	protected override async void OnObjectEnter(GameObject obj)
	{
		_playerManager.PlayerMovementController.DisableMovement();
		await _cameraChanger.TransitionToCam(cam);
		_playerManager.PlayerMovementController.EnableMovement();

		gameObject.SetActive(false);

		if (callback != null)
		{
			await callback();
		}
	}

	protected override void OnObjectExit(GameObject obj)
	{
		return;
	}

}
