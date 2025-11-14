using Cysharp.Threading.Tasks;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
	public CinemachineCamera originalCamera;
	public CinemachineCamera waterCamera;
	public CinemachineCamera towelCamera;
	public CinemachineCamera soapCamera;
	public CinemachineCamera binCamera;

	public Button dryButton;
	public Button wetHandsButton;
	public Button waterButton;
	public Button washHandsButton;
	public Button soapButton;

	private ToiletAnimations_1 _toiletAnimations_1;
	private ToiletAnimations_2 _toiletAnimations_2;
	private CinemachineCameraChanger _cameraChanger;

	private void Awake()
	{
		_toiletAnimations_1 = FindAnyObjectByType<ToiletAnimations_1>();
		_toiletAnimations_2 = FindAnyObjectByType<ToiletAnimations_2>();
		_cameraChanger = FindAnyObjectByType<CinemachineCameraChanger>();

		// Register button listeners
		dryButton.onClick.AddListener(() => RunButtonRoutine(PlayDryHandsAnimation));
		wetHandsButton.onClick.AddListener(() => RunButtonRoutine(PlayWetHandsAnimation));
		waterButton.onClick.AddListener(() => RunButtonRoutine(PlayStartWaterAnimation));
		washHandsButton.onClick.AddListener(() => RunButtonRoutine(PlayShampooHandsAnimation));
		soapButton.onClick.AddListener(() => RunButtonRoutine(PlaySoapAnimation));
	}

	private async void RunButtonRoutine(Func<UniTask> animationFunc)
	{
		Button[] buttons = { dryButton, wetHandsButton, waterButton, washHandsButton, soapButton };

		// Disable all buttons
		foreach (var b in buttons)
			b.gameObject.SetActive(false);

		// Play requested animation
		await animationFunc();

		// Enable buttons again
		foreach (var b in buttons)
			b.gameObject.SetActive(true);
	}

	// ------------------ ANIMATIONS ------------------

	public async UniTask PlaySoapAnimation()
	{
		await _cameraChanger.TransitionToCam(soapCamera);
		await _toiletAnimations_1.ShampooAnimation();
		await _cameraChanger.TransitionToCam(originalCamera);
	}

	public async UniTask PlayStartWaterAnimation()
	{
		await _cameraChanger.TransitionToCam(waterCamera);
		await _toiletAnimations_1.StartWaterAnimation();
		await _cameraChanger.TransitionToCam(originalCamera);
	}

	public async UniTask PlayWetHandsAnimation()
	{
		await _cameraChanger.TransitionToCam(originalCamera);
		await UniTask.Delay(300);
		await _toiletAnimations_2.HandsTogetherAnimation_Sink();
		await UniTask.Delay(2000);
		await _toiletAnimations_2.HandsApartAnimation_Sink();
	}

	public async UniTask PlayShampooHandsAnimation()
	{
		await _cameraChanger.TransitionToCam(originalCamera);
		await _toiletAnimations_2.WashHandsAnimations();
		await UniTask.Delay(400);
		await _toiletAnimations_1.StopWaterAnimation();
		await UniTask.Delay(400);
		await _cameraChanger.TransitionToCam(originalCamera);
		await _toiletAnimations_2.HandsApartAnimation_Sink();
	}

	public async UniTask PlayDryHandsAnimation()
	{
		await _cameraChanger.TransitionToCam(towelCamera);
		await _toiletAnimations_2.DryHandsAnimation();
		await UniTask.Delay(700);

		_cameraChanger.TransitionToCam(binCamera);
		await UniTask.Delay(900);

		await _toiletAnimations_2.ThrowTrashAnimation();
		await UniTask.Delay(1500);

		await _cameraChanger.TransitionToCam(originalCamera);
	}
}
