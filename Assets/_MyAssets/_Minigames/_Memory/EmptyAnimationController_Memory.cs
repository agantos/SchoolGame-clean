using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class EmptyAnimationController_Memory : CabinetContentsAnimations_Base
{
	public override async UniTask PlayCorrectAnimation()
	{

	}

	public override async UniTask PlayIntroductionAnimation()
	{
		await PlayWrongAnimation();
	}

	public override async UniTask PlayWrongAnimation()
	{
		var t = this.transform;
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

	public override void Cleanup()
	{
		// Destroy the GameObject itself
		Destroy(this.gameObject);
	}
}
