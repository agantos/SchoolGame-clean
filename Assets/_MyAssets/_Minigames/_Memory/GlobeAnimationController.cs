using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class GlobeAnimationController : CabinetContentsAnimations_Base
{

	[Header("Particles")]
	public ParticleEffect ambience;

	[Header("Globe Parts")]
	public GameObject globeSphere;   // The sphere part that spins
	public GameObject globeWhole;    // The entire globe object for scaling

	[Header("Animation Settings")]
	public float scaleDuration = 0.6f;
	public float spinDuration = 0.4f;
	public float spinAngle = 720f; // how far it spins (2 full turns)

	// Call this to start the full animation sequence
	public override async UniTask PlayCorrectAnimation()
	{
		await PlayCorrectEffect();
		ambience.Play();
		await GlobeIntroAnimation();
		await UniTask.Delay(400);
		await RotateSphere();
	}

	public override async UniTask PlayIntroductionAnimation()
	{
		ambience.Play();
		await GlobeIntroAnimation();
	}

	public override async UniTask PlayWrongAnimation()
	{
		ambience.Stop();
		var t = globeWhole.transform;
		if (t == null) return;

		Vector3 originalPos = t.localPosition;
		Quaternion originalRot = t.localRotation;

		// Jitter settings
		float jitterAmount = 0.03f;       
		float rotationJitter = 5f;        
		float jitterDuration = 0.05f;    
		int jitterLoops = 8;              

		Sequence seq = DOTween.Sequence();

		for (int i = 0; i < jitterLoops; i++)
		{
			Vector3 randomOffset = new Vector3(
				Random.Range(-jitterAmount, jitterAmount),
				Random.Range(-jitterAmount, jitterAmount),
				0f
			);

			Vector3 randomRot = new Vector3(
				0f,
				0f,
				Random.Range(-rotationJitter, rotationJitter)
			);

			seq.Append(t.DOLocalMove(originalPos + randomOffset, jitterDuration)
				.SetEase(Ease.InOutSine));
			seq.Join(t.DOLocalRotate(originalRot.eulerAngles + randomRot, jitterDuration)
				.SetEase(Ease.InOutSine));
		}

		// Add a playful "return to normal" bounce
		seq.Append(t.DOLocalMove(originalPos, 0.2f)
			.SetEase(Ease.OutBack));
		seq.Join(t.DOLocalRotateQuaternion(originalRot, 0.2f)
			.SetEase(Ease.OutBack));

		seq.Play();

		await seq.AsyncWaitForCompletion();
	}

	private async UniTask GlobeIntroAnimation()
	{
		if (globeWhole == null) return;
		var t = globeWhole.transform;

		// Start invisible and neutral rotation
		t.localScale = Vector3.zero;
		t.localRotation = Quaternion.identity;

		// Sequence for cartoony entrance
		Sequence seq = DOTween.Sequence();

		seq.Append(t.DOScale(Vector3.one * 1.1f, scaleDuration / 2f)
			.SetEase(Ease.OutBack, 1.3f));

		seq.Append(t.DOScale(new Vector3(0.95f, 1.05f, 0.95f), scaleDuration / 3f)
			.SetEase(Ease.InOutSine));

		seq.Append(t.DOScale(Vector3.one * 1.1f, scaleDuration / 3f)
			.SetEase(Ease.OutBack, 1f));

		seq.Append(t.DOScale(Vector3.one, scaleDuration / 4f)
			.SetEase(Ease.OutSine));


		seq.Play();

		// Wait until all animations finish
		await seq.AsyncWaitForCompletion();
	}

	private async UniTask RotateSphere()
	{
		// Capture the starting rotation
		var startRot = globeSphere.transform.localEulerAngles;
		var endRot = startRot + new Vector3(0, 0, spinAngle);

		await globeSphere.transform
			.DOLocalRotate(endRot, spinDuration, RotateMode.FastBeyond360)
			.SetEase(Ease.OutCubic)
			.AsyncWaitForCompletion();
	}
}
