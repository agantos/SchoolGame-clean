using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class RecycleGameNarrative : MonoBehaviour
{
	private CinemachineCameraChanger _cameraChanger;
	private PlayerManager _playerManager;

	public SCR_DialogueNode dialog;

	[Header("UI Objects")]
	public GameObject gameCanvas;
	public RawImage grabButton;
	public RawImage recycleButton;
	public RawImage wasteButton;

	public TextMeshProUGUI congratulationsText;

	[Header("Cameras")]
    public CinemachineCamera binsCamera;
    public CinemachineCamera objectCamera;
	public CinemachineCamera classRoomCamera;

	[Header("Particles")]
	[SerializeField] ParticleEffect confettiEnd_1;
	[SerializeField] ParticleEffect confettiEnd_2;

	[Header("EndDialogue")]
	[SerializeField] SCR_DialogueNode completeGameDialogue;

	private DialogueManager _dialogueManager;

	private void Awake()
	{
		grabButton.gameObject.SetActive(false);
		recycleButton.gameObject.SetActive(false);
		wasteButton.gameObject.SetActive(false);
	}

	private void Start()
	{
		_playerManager = FindAnyObjectByType<PlayerManager>();
		_cameraChanger = FindAnyObjectByType<CinemachineCameraChanger>();
		_dialogueManager = FindAnyObjectByType<DialogueManager>();

		congratulationsText.alpha = 0f;
		congratulationsText.transform.localScale = Vector3.zero;
	}

	#region Introduction
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

	#endregion

	#region Final Animation

	public async UniTask EndAnimation()
	{
		await UniTask.Delay(500);

		await _cameraChanger.TransitionToCam(classRoomCamera);

		congratulationsText.gameObject.SetActive(true);
		_ = PlayCongratulationTextAnimation();
		await UniTask.Delay(1000);
		confettiEnd_1.PlayDisplaced(0.1f, 0.1f, 0f);

		await UniTask.Delay(1000);
		confettiEnd_2.PlayDisplaced(0.1f, 0.1f, 0f);

		await UniTask.Delay(1200);
		congratulationsText.DOFade(0f, 1f);
		confettiEnd_1.PlayDisplaced(0.2f, 0.15f, 0f);

		await UniTask.Delay(500);
		confettiEnd_2.PlayDisplaced(0.1f, 0.1f, 0f);

		_dialogueManager.DialogueToStart = completeGameDialogue;
		_dialogueManager.StartDialogue();
		await _cameraChanger.TransitionBackToPlayerCamera();
	}

	private float fadeDuration = 0.2f;
	private float scaleDuration = 1f;
	private float overshootScale = 1.3f;
	private float bounceAmount = 50f;
	private float bounceDuration = 0.5f;
	private Ease scaleEase = Ease.OutBack;

	public async UniTask PlayCongratulationTextAnimation()
	{
		Vector3 originalPosition = congratulationsText.transform.localPosition;

		// Reset state
		congratulationsText.DOKill(true);
		congratulationsText.transform.DOKill(true);

		congratulationsText.alpha = 0f;
		congratulationsText.transform.localScale = Vector3.zero;
		congratulationsText.transform.localPosition = originalPosition;

		Sequence seq = DOTween.Sequence();

		// --- Phase 1: Fade + scale in with overshoot ---
		seq.Append(congratulationsText.DOFade(1f, fadeDuration));
		seq.Join(congratulationsText.transform
			.DOScale(Vector3.one * overshootScale, scaleDuration)
			.SetEase(scaleEase));

		seq.Append(congratulationsText.transform
		.DOScale(Vector3.one, scaleDuration/3)
		.SetEase(scaleEase));

		// --- Phase 2: Bounce (move up and down) ---
		seq.Append(congratulationsText.transform
			.DOLocalMoveY(originalPosition.y + bounceAmount, bounceDuration)
			.SetEase(Ease.OutQuad));
		seq.Append(congratulationsText.transform
			.DOLocalMoveY(originalPosition.y, bounceDuration)
			.SetEase(Ease.InQuad));

		await seq.Play().AsyncWaitForCompletion();


	}
	#endregion

}
