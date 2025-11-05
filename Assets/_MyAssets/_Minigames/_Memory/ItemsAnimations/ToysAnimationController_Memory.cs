using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ToysAnimationController_Memory : CabinetContentsAnimations_Base
{
	public GameObject train;
	public GameObject spaceship;
	public GameObject planet;
	public GameObject horse;


	public override async UniTask PlayCorrectAnimation()
	{

	}

	public override async UniTask PlayIntroductionAnimation()
	{
		await PlayJitterAnimation();
	}

	public override async UniTask PlayWrongAnimation()
	{
		await PlayJitterAnimation();
	}


	#region Jitter
	[Header("Jitter Settings")]
	float jitterAmount = 15f;       // max local offset
	float rotationJitter = 3f;       // max rotation degrees
	float jitterDuration = 0.03f;     // duration of each shake
	int jitterLoops = 8;              // number of shakes
	float stagger = 0.03f;            // slight offset per object
	public async UniTask PlayJitterAnimation()
	{
		var _objects = new List<GameObject> {
		train, spaceship, planet,
		horse,
	};

		List<UniTask> allSequences = new List<UniTask>();

		for (int i = 0; i < _objects.Count; i++)
		{
			var obj = _objects[i];
			if (obj == null) continue;

			var t = obj.transform;
			Vector3 originalPos = t.localPosition;
			Quaternion originalRot = t.localRotation;

			Sequence seq = DOTween.Sequence();
			for (int j = 0; j < jitterLoops; j++)
			{
				Vector3 randomOffset = new Vector3(
					Random.Range(-jitterAmount, jitterAmount),
					Random.Range(-jitterAmount, jitterAmount),
					Random.Range(-jitterAmount, jitterAmount)
				);

				Vector3 randomRotation = new Vector3(
					Random.Range(-rotationJitter, rotationJitter),
					Random.Range(-rotationJitter, rotationJitter),
					Random.Range(-rotationJitter, rotationJitter)
				);

				seq.Append(t.DOLocalMove(originalPos + randomOffset, jitterDuration).SetEase(Ease.Flash, 1));
				seq.Join(t.DOLocalRotate(originalRot.eulerAngles + randomRotation, jitterDuration).SetEase(Ease.Flash, 1));
			}

			seq.Append(t.DOLocalMove(originalPos, jitterDuration).SetEase(Ease.OutBounce));
			seq.Join(t.DOLocalRotateQuaternion(originalRot, jitterDuration).SetEase(Ease.OutBounce));
			seq.PrependInterval(i * stagger);

			// --- Wrap sequence completion in UniTask ---
			var tcs = new UniTaskCompletionSource();
			seq.OnComplete(() => tcs.TrySetResult());
			seq.Play();
			allSequences.Add(tcs.Task);
		}

		await UniTask.WhenAll(allSequences);
	}
	#endregion


	public override void Cleanup()
	{
		// Finally, destroy this GameObject
		Destroy(this.gameObject);
	}
}
