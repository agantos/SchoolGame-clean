using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class RecycleGameIntroduction : MonoBehaviour
{
	private CinemachineCameraChanger _cameraChanger;

	public SCR_DialogueNode dialog;

	[Header("UI Objects")]
	public GameObject gameCanvas;
	public RawImage grabButton;
	public RawImage recycleButton;
	public RawImage wasteButton;


	[Header("Cameras")]
    public CinemachineCamera binsCamera;
    public CinemachineCamera objectCamera;
	public CinemachineCamera classRoomCamera;

	private DialogueManager _dialogueManager;

	private void Awake()
	{
		grabButton.gameObject.SetActive(false);
		recycleButton.gameObject.SetActive(false);
		wasteButton.gameObject.SetActive(false);
	}

	private async void Start()
	{

		_cameraChanger = FindAnyObjectByType<CinemachineCameraChanger>();
		_dialogueManager = FindAnyObjectByType<DialogueManager>();
	}

	public async UniTask PlayIntroduction()
	{
		await NarrationStep_0();

		await NarrationStep_1();

		await NarrationStep_2();

		await NarrationStep_3();
	}

	async UniTask NarrationStep_0()
	{
		_dialogueManager.DialogueToStart = dialog;
		_cameraChanger.TransitionToCam(classRoomCamera);

		await UniTask.Delay(450);
		_dialogueManager.StartDialogue();
		await UniTask.Delay(5000);

	}

	async UniTask NarrationStep_1()
	{
		_dialogueManager.OnNextPressed();
		await _cameraChanger.TransitionToCam(objectCamera);
		await UniTask.Delay(350);
		// play grab item animation constantly
		PlayButtonPulse(grabButton);
		await UniTask.Delay(4000);
		//stop grab item animation
		StopButtonAnimation(grabButton);
	}

	async UniTask NarrationStep_2()
	{
		_cameraChanger.TransitionToCam(binsCamera);
		await UniTask.Delay(500);
		_dialogueManager.OnNextPressed();
		await UniTask.Delay(4500);
		// play buttons animation constantly
		PlayButtonPulse(wasteButton);
		PlayButtonPulse(recycleButton);

		Debug.Log("Start Buttons animation");

	}

	async UniTask NarrationStep_3()
	{
		_dialogueManager.OnNextPressed();
		await UniTask.Delay(4500);
		_dialogueManager.OnNextPressed();
		await UniTask.Delay(6500);

		_dialogueManager.CloseDialogue();

		await UniTask.Delay(500);

		//stop buttons animation
		StopButtonAnimation(wasteButton);
		StopButtonAnimation(recycleButton);

		await UniTask.Delay(500);

		_cameraChanger.TransitionBackToPlayerCamera();

	}

	void PlayButtonPulse(RawImage button, float scaleMultiplier = 1f, float duration = 1f)
	{
		button.transform.localScale = Vector3.zero;
		button.gameObject.SetActive(true);
		if (button == null) return;

		button.transform
			.DOScale(scaleMultiplier, duration)
			.SetLoops(-1, LoopType.Yoyo)
			.SetEase(Ease.InOutSine);
	}

	void StopButtonAnimation(RawImage button)
	{
		if (button == null) return;

		button.transform.DOKill(); // stop tween
		button.transform.localScale = Vector3.one;
		button.gameObject.SetActive(false);

	}

}
