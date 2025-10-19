using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class BinAnimation : MonoBehaviour
{
	[Header("Transforms")]

	public Transform bin;
	public Transform trash;
	public Transform returnTrashPosition;

	public CinemachineCamera cameraToTransition;
	CinemachineCameraChanger _cameraChanger;

	private void Awake()
	{
		_cameraChanger = FindAnyObjectByType<CinemachineCameraChanger>();
	}

	private void Update()
	{
		if (playWrong)
		{
			PlayWrong();
			playWrong = false;
		}

		if (playCorrect)
		{
			playCorrect = false;
			PlayCorrect();
		}

		if (throwTrash)
		{
			ThrowTrash();
			throwTrash = false;
		}

		if (returnTrash)
		{
			TrashFliesBack();
			returnTrash = false;
		}
	}

	public bool returnTrash;
	public bool throwTrash;

	public async UniTask PlayCorrectAnimation(GameObject trash)
	{
		this.trash = trash.transform;
		_cameraChanger.TransitionToCam(cameraToTransition);
		await ThrowTrash();
		await UniTask.Delay(1000);
		await PlayCorrect();
		await UniTask.Delay(1000);
		_cameraChanger.TransitionBackToPlayerCamera();
		trash.SetActive(false);
	}

	public async UniTask PlayWrongAnimation(GameObject trash)
	{
		this.trash = trash.transform;
		_cameraChanger.TransitionToCam(cameraToTransition);
		await ThrowTrash();
		await UniTask.Delay(1000);
		await PlayWrong();
		await UniTask.Delay(500);
		_cameraChanger.TransitionBackToPlayerCamera();
		await TrashFliesBack();
	}

	#region Object Animations

	float _binShakeDuration = 2f;
	float _binShakeStrength = 0.05f;
	int _binShakeVibrato = 40;

	public async UniTask ShakeBin()
	{
		await bin.DOShakePosition(_binShakeDuration, _binShakeStrength, _binShakeVibrato, 90, false, true).AsyncWaitForCompletion();
	}

	float jumpPower = 1.2f;
	float duration = 0.9f;

	public async UniTask ThrowTrash()
	{
		// --- Cache original scale & rotation ---
		Vector3 originalScale = bin.localScale;
		Quaternion originalRotation = bin.localRotation;

		// --- Trash flies INTO bin ---
		var trashTween = trash
			.DOJump(bin.position, jumpPower, 1, duration)
			.SetEase(Ease.OutQuad);

		// Wait until the trash almost reaches the bin
		await UniTask.Delay((int)(duration * 700)); // Wait roughly until arrival

		// --- Bin reacts (quick “gulp”) ---
		Sequence gulpSeq = DOTween.Sequence();

		gulpSeq.Append(
			bin.DOScale(new Vector3(
				originalScale.x * 1.2f,
				originalScale.y * 0.8f,
				originalScale.z * 1f
			), 0.1f).SetEase(Ease.OutQuad)
		)
		.Join(
			bin.DOLocalRotate(originalRotation.eulerAngles + new Vector3(-20f, 0f, 0f), 0.1f)
				.SetEase(Ease.OutQuad)
		)
		.Append(
			bin.DOScale(originalScale, 0.25f).SetEase(Ease.OutBack)
		)
		.Join(
			bin.DOLocalRotate(originalRotation.eulerAngles, 0.25f).SetEase(Ease.OutBack)
		);

		await gulpSeq.AsyncWaitForCompletion().AsUniTask();

		// Optional: little shake for satisfaction
		await bin.DOShakeScale(0.2f, 0.1f, 10, 90f, false)
				 .AsyncWaitForCompletion()
				 .AsUniTask();
	}

	public async UniTask TrashFliesBack()
	{
		// --- Bin animation ---
		// exaggerated squash, stretch, tilt and recovery
		Sequence binSeq = DOTween.Sequence();

		binSeq.Append(
			bin.DOScaleY(0.7f, 0.08f)  // quick squash
				.SetEase(Ease.OutQuad)
		)
		.Join(
			bin.DOScaleX(1.3f, 0.08f)  // widen a bit
		)
		.Join(
			bin.DORotate(new Vector3(-15f, 0f, 0f), 0.1f) // tilt forward as it throws
		)
		.Append(
			bin.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack) // recover to normal
		)
		.Join(
			bin.DORotate(Vector3.zero, 0.25f).SetEase(Ease.OutBack)
		);

		// Small anticipation delay before throw
		await UniTask.Delay(100);

		// --- Trash jump animation ---
		var trashTween = trash
			.DOJump(returnTrashPosition.position, jumpPower, 1, duration)
			.SetEase(Ease.OutQuad);

		// Play both together (bin sequence finishes slightly earlier)
		await UniTask.WhenAll(
			binSeq.AsyncWaitForCompletion().AsUniTask(),
			trashTween.AsyncWaitForCompletion().AsUniTask()
		);
	}
	#endregion

	#region Particle Effects - Animations

	[Header("Particles")]
	public ParticleEffect correct;
	public ParticleEffect wrong;
	public ParticleEffect[] sparks;


	public bool playWrong;
	public bool playCorrect;

	public async UniTask PlayWrong()
	{
		await ShakeBin();
		await UniTask.Delay(1000);
		await WaitSparks();
		await UniTask.Delay(300);
		wrong.Play();
	}

	public async UniTask PlayCorrect()
	{
		await ShakeBin();
		await UniTask.Delay(1000);
		await WaitSparks();
		await UniTask.Delay(300);
		correct.Play();
	}

	async UniTask WaitSparks()
	{
		// Shuffle the array so each spark plays once in random order
		var shuffledSparks = sparks.OrderBy(_ => UnityEngine.Random.value).ToArray();

		foreach (var spark in shuffledSparks)
		{
			// Set random scale
			float scale = UnityEngine.Random.Range(0.5f, 1.2f);
			spark.transform.localScale = new Vector3(scale, scale, scale);

			// Set Random Position
			Vector3 initialPosition = spark.transform.localPosition;
			float x = UnityEngine.Random.Range(-0.001f, 0.001f);
			float y = 0;
			float z = UnityEngine.Random.Range(-0.001f, 0.001f);

			spark.transform.localPosition += new Vector3(x,y,z);

			spark.Play();


			float delay = UnityEngine.Random.Range(0.6f, 0.9f);
			await UniTask.Delay(TimeSpan.FromSeconds(delay));
			spark.transform.localPosition = initialPosition;
		}
	}

	#endregion
}
