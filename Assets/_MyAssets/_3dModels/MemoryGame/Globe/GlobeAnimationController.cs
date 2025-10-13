using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class GlobeAnimationController : MonoBehaviour
{
	[Header("Globe Parts")]
	public GameObject globeSphere;   // The sphere part that spins
	public GameObject globeWhole;    // The entire globe object for scaling

	[Header("Animation Settings")]
	public float scaleDuration = 0.6f;
	public float spinDuration = 0.4f;
	public float spinAngle = 720f; // how far it spins (2 full turns)

	public bool playAnimation;
	private void Update()
	{
		if (playAnimation)
		{
			PlayGlobeAnimation();
			playAnimation = false;
		}		
	}

	// Call this to start the full animation sequence
	public async UniTask PlayGlobeAnimation()
	{
		await ScaleAnimation();
		await RotateSphere();
	}

	// Playful scale bounce for the whole globe
	private async UniTask ScaleAnimation()
	{
		
		globeWhole.transform.localScale = Vector3.zero;

		// DOTween: scale up with a "bounce" ease
		await globeWhole.transform
			.DOScale(Vector3.one, scaleDuration)
			.SetEase(Ease.OutBack, 1.4f)
			.AsyncWaitForCompletion();
	}

	// Quick spin animation for the globe sphere
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
