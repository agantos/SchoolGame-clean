using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public abstract class CabinetContentsAnimations_Base : MonoBehaviour
{
	[Header("Base Class Variables")]
	[SerializeField] ParticleEffect CorrectEffect;
	public CinemachineCamera viewCamera;
	public GameObject cabinetDoor;
	public Transform transformParent;

	public bool playIntroduction;
	public bool playCorrect;
	public bool playWrong;

	void Start()
	{
		gameObject.SetActive(false);
	}
	private void Update()
	{
		if (playCorrect)
		{
			PlayCorrectAnimation();
			playCorrect = false;
		}

		if (playWrong)
		{
			PlayWrongAnimation();
			playWrong = false;
		}

		if (playIntroduction)
		{
			PlayIntroductionAnimation();
			playIntroduction = false;
		}
	}

	public async UniTask PlayCorrectEffect() { 
		CorrectEffect.Play();
		await UniTask.Delay(1000);
	}

	#region OPEN CLOSE DOOR

	float openAngle_door = -90f;
	float duration_door = 1.2f;

	private bool isOpen_door = false;

	private Tween currentTween_door;

	public async UniTask OpenCabinetDoor()
	{
		if (isOpen_door) return;
		isOpen_door = true;

		currentTween_door?.Kill();

		// Rotate smoothly to openAngle
		currentTween_door = cabinetDoor.transform
			.DOLocalRotate(
				new Vector3(0f, openAngle_door, 0f),
				duration_door,
				RotateMode.Fast)
			.SetEase(Ease.OutBack, 1.5f); // nice cartoony pop

		await currentTween_door.AsyncWaitForCompletion();

		// Example: deactivate door after animation
		cabinetDoor.SetActive(false);
	}

	public async UniTask CloseCabinetDoor()
	{
		cabinetDoor.SetActive(true);
		if (!isOpen_door) return;
		isOpen_door = false;

		currentTween_door?.Kill();

		// Rotate smoothly back to zero (closed)
		currentTween_door = cabinetDoor.transform
			.DOLocalRotate(Vector3.zero, duration_door, RotateMode.Fast)
			.SetEase(Ease.InBack, 1.5f);

		await currentTween_door.AsyncWaitForCompletion();
	}

	#endregion



	public virtual async UniTask PrepareForCorrect()
	{
		gameObject.SetActive(true);
		await UniTask.Yield(); // keeps async consistent
	}

	public virtual async UniTask PrepareForWrong()
	{
		gameObject.SetActive(true);
		await UniTask.Yield();
	}

	public virtual async UniTask PrepareIntroduction()
	{
		gameObject.SetActive(true);
		await UniTask.Yield();
	}


	public abstract UniTask PlayCorrectAnimation();
	public abstract UniTask PlayWrongAnimation();
	public abstract UniTask PlayIntroductionAnimation();
	public abstract void Cleanup();
}
