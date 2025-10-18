using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SchoolSuppliesAnimationController_Memory : CabinetContentsAnimations_Base
{

	[Header("Non-Physics")]
	public GameObject CabinetView_noPhysics;
	public GameObject pencilHolder_noPhysics;
	public GameObject triangle_noPhysics;
	public GameObject sponge_noPhysics;

	[Header("Inside-Particles")]
	public ParticleEffect confetti;

	private void Awake()
	{
		rigidBodies_pencilHolder = GetComponentsInChildren<Rigidbody>().ToList();
		rigidBody_triangle = triangle_physics.GetComponent<Rigidbody>();
		rigidBody_sponge = sponge_physics.GetComponent<Rigidbody>();
	}

	#region Prepare

	public override async UniTask PrepareForCorrect()
	{
		gameObject.SetActive(true);
		CabinetView_noPhysics.SetActive(false);
		CabinetView_physics.SetActive(true);
	}

	public override async UniTask PrepareForWrong()
	{
		gameObject.SetActive(true);
		CabinetView_noPhysics.SetActive(true);
		CabinetView_physics.SetActive(false);
	}

	public override async UniTask PrepareIntroduction()
	{
		gameObject.SetActive(true);
		CabinetView_noPhysics.SetActive(true);
		CabinetView_physics.SetActive(false);
	}

	#endregion

	#region Introduction

	public async override UniTask PlayIntroductionAnimation()
	{
		var objects = new List<GameObject>
	{
		pencilHolder_noPhysics,
		triangle_noPhysics,
		sponge_noPhysics
	};

		// Animation tuning
		float scaleUpDuration = 0.45f;
		float squashDuration = 0.25f;
		float settleDuration = 0.35f;
		float staggerDelay = 0.12f; // delay between objects

		List<UniTask> allTasks = new List<UniTask>();

		for (int i = 0; i < objects.Count; i++)
		{
			var obj = objects[i];
			if (obj == null) continue;
			var t = obj.transform;

			Vector3 originalScale = t.localScale;
			Quaternion originalRot = t.localRotation;

			// Start hidden and slightly tilted
			t.localScale = Vector3.zero;
			t.localRotation = Quaternion.Euler(
				Random.Range(-10f, 10f),
				Random.Range(-10f, 10f),
				Random.Range(-10f, 10f)
			);

			Sequence seq = DOTween.Sequence();

			seq.Append(t.DOScale(originalScale * 1.15f, scaleUpDuration)
				.SetEase(Ease.OutBack, 1.4f));

			seq.Append(t.DOScale(originalScale * 0.9f, squashDuration / 2f)
				.SetEase(Ease.InOutSine));

			seq.Append(t.DOScale(originalScale * 1.05f, squashDuration / 2f)
				.SetEase(Ease.OutBack, 1.1f));

			seq.Join(t.DOLocalRotateQuaternion(originalRot, settleDuration)
				.SetEase(Ease.OutCubic));

			seq.Append(t.DOScale(originalScale, settleDuration)
				.SetEase(Ease.OutCubic));

			// Add stagger delay between objects
			seq.PrependInterval(i * staggerDelay);

			// Wrap in UniTask
			var tcs = new UniTaskCompletionSource();
			seq.OnComplete(() => tcs.TrySetResult());
			seq.Play();

			allTasks.Add(tcs.Task);
		}

		await UniTask.WhenAll(allTasks);
	}

	#endregion

	#region CorrectAnimation
	[Space]
	[Space]
	[Header("Physics")]
	public GameObject CabinetView_physics;
	public GameObject pencilHolder_physics;
	public GameObject triangle_physics;
	public GameObject sponge_physics;

	List<Rigidbody> rigidBodies_pencilHolder;
	Rigidbody rigidBody_triangle;
	Rigidbody rigidBody_sponge;


	public async override UniTask PlayCorrectAnimation()
	{
		await PlayCorrectEffect();

		rigidBody_triangle.useGravity = true;

		await UniTask.Delay(150);

		rigidBody_sponge.useGravity = true;

		await UniTask.Delay(150);

		int i = 0;

		foreach (var rb in rigidBodies_pencilHolder)
		{
			i += 10;
			await UniTask.Delay(150 - i);

			rb.useGravity = true;
		}
		await UniTask.Delay(500);
		confetti.PlayDisplaced(0f, 0.05f, 0.1f);
		await UniTask.Delay(1200);
		confetti.PlayDisplaced(0f, 0.05f, 0.1f);


	}
	#endregion

	#region Wrong
	public async override UniTask PlayWrongAnimation()
	{
		var objects = new List<GameObject>
	{
		CabinetView_noPhysics,
		pencilHolder_noPhysics,
		triangle_noPhysics,
		sponge_noPhysics
	};

		// Animation parameters (tweak to taste)
		float jitterAmount = 0.008f;       // how far they move
		float rotationJitter = 1f;        // how much they rotate
		float jitterDuration = 0.05f;     // speed of each shake
		int jitterLoops = 7;              // number of wiggles
		float settleDuration = 0.25f;     // smooth settle after shakes

		List<UniTask> allTasks = new List<UniTask>();

		for (int i = 0; i < objects.Count; i++)
		{
			var obj = objects[i];
			if (obj == null) continue;
			var t = obj.transform;

			Vector3 originalPos = t.localPosition;
			Quaternion originalRot = t.localRotation;

			Sequence seq = DOTween.Sequence();

			// Each object starts slightly staggered for more organic feel
			seq.PrependInterval(i * 0.05f);

			for (int j = 0; j < jitterLoops; j++)
			{
				Vector3 randomOffset = new Vector3(
					Random.Range(-jitterAmount, jitterAmount),
					Random.Range(-jitterAmount, jitterAmount),
					Random.Range(-jitterAmount, jitterAmount) * 0.5f
				);

				Vector3 randomRotation = new Vector3(
					Random.Range(-rotationJitter, rotationJitter),
					Random.Range(-rotationJitter, rotationJitter),
					Random.Range(-rotationJitter, rotationJitter)
				);

				seq.Append(t.DOLocalMove(originalPos + randomOffset, jitterDuration)
					.SetEase(Ease.InOutSine));
				seq.Join(t.DOLocalRotate(originalRot.eulerAngles + randomRotation, jitterDuration)
					.SetEase(Ease.InOutSine));
			}

			// Add a fun squash/stretch at the end for cartooniness
			seq.Join(t.DOPunchScale(Vector3.one * 0.1f, settleDuration, 3, 0.6f));

			// Smooth settle back to original
			seq.Append(t.DOLocalMove(originalPos, settleDuration).SetEase(Ease.OutBack));
			seq.Join(t.DOLocalRotateQuaternion(originalRot, settleDuration).SetEase(Ease.OutBack));

			// Wrap into UniTask
			var tcs = new UniTaskCompletionSource();
			seq.OnComplete(() => tcs.TrySetResult());
			seq.Play();

			allTasks.Add(tcs.Task);
		}

		await UniTask.WhenAll(allTasks);
	}
	#endregion

	public override void Cleanup()
	{
		// Kill all DOTween animations
		DOTween.Kill(CabinetView_noPhysics, true);
		DOTween.Kill(CabinetView_physics, true);
		DOTween.Kill(pencilHolder_noPhysics, true);
		DOTween.Kill(pencilHolder_physics, true);
		DOTween.Kill(triangle_noPhysics, true);
		DOTween.Kill(triangle_physics, true);
		DOTween.Kill(sponge_noPhysics, true);
		DOTween.Kill(sponge_physics, true);

		// Stop particle effects
		if (confetti != null)
		{
			confetti.Stop();
			Destroy(confetti.gameObject);
			confetti = null;
		}

		// Clear references
		CabinetView_noPhysics = null;
		CabinetView_physics = null;
		pencilHolder_noPhysics = null;
		pencilHolder_physics = null;
		triangle_noPhysics = null;
		triangle_physics = null;
		sponge_noPhysics = null;
		sponge_physics = null;
		rigidBodies_pencilHolder = null;
		rigidBody_triangle = null;
		rigidBody_sponge = null;

		// Destroy the GameObject itself
		Destroy(this.gameObject);
	}

}
