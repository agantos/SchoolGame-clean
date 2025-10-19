using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Video;

public class ProjectorController : MonoBehaviour
{
	private Animator animator;
	private bool isOpen = false;
	private bool isAnimating = false;

	// Callbacks
	public event Func<UniTask> onProjectorOpened;
	public event Func<UniTask> onProjectorClosed;

	//cameras
	public CinemachineCamera projectorCamera;
	public CinemachineCamera projectorViewCamera;
	CinemachineCameraChanger _cameraChanger;

	//Video variables
	VideoPlayer _videoPlayer;

	PlayerManager _playerManager;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		_videoPlayer = GetComponentInChildren<VideoPlayer>(includeInactive: true);
		_cameraChanger = FindAnyObjectByType<CinemachineCameraChanger>();
		_playerManager = FindAnyObjectByType<PlayerManager>();	
	}

	public void CleanOnCloseEvent()
	{
		onProjectorClosed = null;
	}

	public async void OpenProjectorVideo(VideoClip clip)
	{
		_playerManager.PlayerMovementController.DisableMovement();
		_videoPlayer.gameObject.SetActive(true);
		_videoPlayer.Stop();
		_videoPlayer.clip = clip;
		_videoPlayer.Prepare();
		_videoPlayer.GetComponent<CanvasGroup>().alpha = 0.3f;

		await OpenProjector();
		await UniTask.Delay(500);
		_videoPlayer.GetComponent<CanvasGroup>().DOFade(0.96f, 3.5f).onComplete = 
			() =>
			{
				_videoPlayer.Play();
				_videoPlayer.loopPointReached += OnVideoEnded;
			}
		;
		await _cameraChanger.TransitionToCam(projectorViewCamera);


	}

	// Ends with Camera on projector and Movement Disabled
	async void OnVideoEnded(VideoPlayer vp)
	{
		vp.loopPointReached -= OnVideoEnded;
		_videoPlayer.GetComponent<CanvasGroup>().DOFade(0.3f, 2f);
		await UniTask.Delay(1000);
		await CloseProjector();
		_videoPlayer.gameObject.SetActive(false);
	}

	public async UniTask OpenProjector()
	{
		if (isOpen || isAnimating) return;

		await _cameraChanger.TransitionToCam(projectorCamera);

		animator.SetTrigger("Open");
		isAnimating = true;
		isOpen = true;

		await WaitForState("ProjectorOpen");

		isAnimating = false;
		onProjectorOpened?.Invoke();
	}

	public async UniTask CloseProjector()
	{
		if (!isOpen || isAnimating) return;

		await _cameraChanger.TransitionToCam(projectorCamera);

		animator.SetTrigger("Close");

		isAnimating = true;
		isOpen = false;

		await WaitForState("ProjectorClose");
		isAnimating = false;

		await onProjectorClosed.Invoke();
	}

	private async UniTask WaitForState(string stateName)
	{
		while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
			await UniTask.Yield();

		float animLength = animator.GetCurrentAnimatorStateInfo(0).length;

		await UniTask.Delay(TimeSpan.FromSeconds(animLength));
	}
}
