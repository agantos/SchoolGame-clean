using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class BooksAnimationController_Memory : CabinetContentsAnimations_Base
{
	public ParticleEffect confetti;

	[Header("Books")]
	public GameObject orange_gameObject;
	public GameObject yellow_gameObject;
	public GameObject red_gameObject;
	public GameObject green_gameObject;

	[Header("Transforms - Starting")]
	public Transform orange_start;
	public Transform yellow_start;
	public Transform red_start;
	public Transform green_start;


	[Header("Transforms - Final")]
	public Transform orange_end;
	public Transform yellow_end;
	public Transform red_end;
	public Transform green_end;

	[Header("Animation Settings")]
	public float riseDuration = 0.9f;
	public float stackFollowDelay = 0.05f;
	public float slideDuration = 1.2f;
	public float postRiseDelay = 0.2f;
	public Ease riseEase = Ease.OutBack;
	public Ease slideEase = Ease.OutQuad;

	public override async UniTask PlayCorrectAnimation()
	{
		PlayCorrectEffect();
		await UniTask.Delay(700);
		await RiseBooks();
		confetti.PlayDisplaced(0.1f, 0.06f, 0f);
		await UniTask.Delay(1000);
		confetti.PlayDisplaced(0.1f, 0.05f, 0f);


	}

	public async UniTask RiseBooks()
	{
		// Cache original scales
		Vector3 yellowOriginalScale = yellow_gameObject.transform.localScale;
		Vector3 greenOriginalScale = green_gameObject.transform.localScale;
		Vector3 redOriginalScale = red_gameObject.transform.localScale;
		Vector3 orangeOriginalScale = orange_gameObject.transform.localScale;

		Sequence seq = DOTween.Sequence();

		float popDuration = 0.12f; // very quick pop
		float popScale = 1.15f;    // scale up multiplier

		// --- STEP 0: All books pop simultaneously ---
		seq.Append(yellow_gameObject.transform.DOScale(yellowOriginalScale * popScale, popDuration).SetEase(Ease.OutQuad));
		seq.Join(green_gameObject.transform.DOScale(greenOriginalScale * popScale, popDuration).SetEase(Ease.OutQuad));
		seq.Join(red_gameObject.transform.DOScale(redOriginalScale * popScale, popDuration).SetEase(Ease.OutQuad));

		seq.Append(yellow_gameObject.transform.DOScale(yellowOriginalScale, popDuration).SetEase(Ease.InQuad));
		seq.Join(green_gameObject.transform.DOScale(greenOriginalScale, popDuration).SetEase(Ease.InQuad));
		seq.Join(red_gameObject.transform.DOScale(redOriginalScale, popDuration).SetEase(Ease.InQuad));

		// --- STEP 1: After all pops, rise all books together ---
		seq.Append(yellow_gameObject.transform.DOMove(yellow_end.position, riseDuration).SetEase(riseEase));
		seq.Join(yellow_gameObject.transform.DORotateQuaternion(yellow_end.rotation, riseDuration).SetEase(riseEase));

		seq.Join(green_gameObject.transform.DOMove(green_end.position, riseDuration).SetEase(riseEase));
		seq.Join(green_gameObject.transform.DORotateQuaternion(green_end.rotation, riseDuration).SetEase(riseEase));

		seq.Join(red_gameObject.transform.DOMove(red_end.position, riseDuration).SetEase(riseEase));
		seq.Join(red_gameObject.transform.DORotateQuaternion(red_end.rotation, riseDuration).SetEase(riseEase));

		// --- STEP 2: Orange slides in after the stack has risen ---
		float totalRiseTime = popDuration * 2 + riseDuration;
		Vector3 slideOvershoot = orange_end.position + new Vector3(0f, 0.02f, 0.04f);
		Quaternion orangeTilt = Quaternion.Euler(orange_end.rotation.eulerAngles + new Vector3(-8f, 2f, 0f));

		seq.Insert(totalRiseTime,
			orange_gameObject.transform.DOMove(slideOvershoot, slideDuration * 0.5f).SetEase(Ease.OutCubic));
		seq.Insert(totalRiseTime + slideDuration * 0.5f,
			orange_gameObject.transform.DOMove(orange_end.position, slideDuration * 0.5f).SetEase(slideEase));

		seq.Insert(totalRiseTime,
			orange_gameObject.transform.DORotateQuaternion(orangeTilt, slideDuration * 0.4f).SetEase(Ease.OutCubic));
		seq.Insert(totalRiseTime + slideDuration * 0.4f,
			orange_gameObject.transform.DORotateQuaternion(orange_end.rotation, slideDuration * 0.6f).SetEase(Ease.OutQuad));

		await seq.AsyncWaitForCompletion();
		Debug.Log("Book animation complete!");
	}


	public override async UniTask PlayWrongAnimation()
	{ 
	
	}

	public override async UniTask PlayIntroductionAnimation()
	{
		ResetPositions();
	}

	private void ResetPositions()
	{
		orange_gameObject.transform.SetPositionAndRotation(orange_start.position, orange_start.rotation);
		yellow_gameObject.transform.SetPositionAndRotation(yellow_start.position, yellow_start.rotation);
		red_gameObject.transform.SetPositionAndRotation(red_start.position, red_start.rotation);
		green_gameObject.transform.SetPositionAndRotation(green_start.position, green_start.rotation);
	}

	public override void Cleanup()
	{
		// Stop particle effects
		if (confetti != null)
		{
			confetti.Stop();
			Destroy(confetti.gameObject);
			confetti = null;
		}

		// Destroy the GameObject itself
		Destroy(this.gameObject);
	}
}
