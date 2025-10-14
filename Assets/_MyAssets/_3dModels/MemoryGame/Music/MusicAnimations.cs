using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MusicAnimations : CabinetContentsAnimations_Base
{
	[Header("Game Objects")]

	[SerializeField] GameObject noteSingle;
	[SerializeField] GameObject noteDouble;
	[SerializeField] GameObject solKey;

	[SerializeField] GameObject xylophone;
	[SerializeField] GameObject leftBaton;
	[SerializeField] GameObject rightBaton;

	[SerializeField] GameObject recorder;

	//public void Update()
	//{
	//	if (test)
	//	{
	//		PlayWrongAnimation();
	//		test = false;
	//	}
	//}

	public override async UniTask PlayCorrectAnimation()
	{
		await MoveToPositions();
		StartPerforming();
	}

	public override async UniTask PlayWrongAnimation()
	{
		await PlayJitterAnimation();
	}


	public override async UniTask PlayIntroductionAnimation()
	{
		await AwakeObjects();
	}

	#region Awake Animation

	[Header("Awake Animation Settings")]
	float bounceHeight = 0.5f;
	float bounceDuration = 0.3f;
	float scaleDuration = 0.2f;
	float staggerDelay_awake = 0.1f;

	public async UniTask AwakeObjects()
	{
		// Collect everything in an array for easy looping
		GameObject[] allObjects = {
			noteSingle, noteDouble, solKey,
			xylophone, leftBaton, rightBaton, recorder
		};

		// Animate each one with a slight delay between them
		for (int i = 0; i < allObjects.Length; i++)
		{
			var obj = allObjects[i];
			if (obj == null) continue;

			var t = obj.transform;
			Vector3 originalPos = t.localPosition;
			Vector3 startPos = originalPos - Vector3.up * bounceHeight;

			// Start below and small
			t.localPosition = startPos;
			t.localScale = Vector3.zero;

			// Run bounce and scale together
			t.DOScale(Vector3.one, scaleDuration)
				.SetEase(Ease.OutBack, 1.4f)
				.SetDelay(i * staggerDelay_awake);

			t.DOLocalMoveY(originalPos.y, bounceDuration)
				.SetEase(Ease.OutBounce)
				.SetDelay(i * staggerDelay_awake);

			// small async delay so they play in order
			await UniTask.Delay((int)(staggerDelay_awake * 1000f));
		}

		// wait until all animations done
		await UniTask.Delay((int)((bounceDuration + staggerDelay_awake * allObjects.Length) * 1000f));
	}

	#endregion

	#region Jitter
	[Header("Jitter Settings")]
	float jitterAmount = 0.1f;       // max local offset
	float rotationJitter = 5f;       // max rotation degrees
	float jitterDuration = 0.03f;     // duration of each shake
	int jitterLoops = 8;              // number of shakes
	float stagger = 0.02f;            // slight offset per object
	public async UniTask PlayJitterAnimation()
	{
		var _objects = new List<GameObject> {
		noteSingle, noteDouble, solKey,
		xylophone, leftBaton, rightBaton, recorder
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

	#region MoveToPositions
	[Header("Move End Positions")]
	[SerializeField] Transform noteSingleTarget;
	[SerializeField] Transform noteDoubleTarget;
	[SerializeField] Transform solKeyTarget;
	[SerializeField] Transform xylophoneTarget;
	[SerializeField] Transform leftBatonTarget;
	[SerializeField] Transform rightBatonTarget;
	[SerializeField] Transform recorderTarget;

	float moveDuration = 1.2f;
	float rotateDuration = 1.2f;
	float staggerDelay = 0.15f;
	Ease moveEase = Ease.OutBack;
	Ease rotateEase = Ease.OutCubic;

	public async UniTask MoveToPositions()
	{
		(GameObject obj, Transform target)[] pairs = {
			(noteSingle, noteSingleTarget),
			(noteDouble, noteDoubleTarget),
			(solKey, solKeyTarget),
			(xylophone, xylophoneTarget),
			(leftBaton, leftBatonTarget),
			(rightBaton, rightBatonTarget),
			(recorder, recorderTarget)
		};

		for (int i = 0; i < pairs.Length; i++)
		{
			var (obj, target) = pairs[i];
			if (obj == null || target == null) continue;

			var t = obj.transform;

			// Move & rotate with easing
			t.DOLocalMove(target.localPosition, moveDuration)
				.SetEase(moveEase)
				.SetDelay(i * staggerDelay);

			t.DOLocalRotateQuaternion(target.localRotation, rotateDuration)
				.SetEase(rotateEase)
				.SetDelay(i * staggerDelay);

			await UniTask.Delay((int)(staggerDelay * 1000f));
		}

		// Wait for final object
		await UniTask.Delay((int)((moveDuration + staggerDelay * pairs.Length) * 1000f));
	}

	#endregion

	#region Performing
	[SerializeField] float performanceLoopDuration = 1.5f;
	[SerializeField] float jumpHeight = 0.4f;     // how high notes jump

	private Tween recorderTween;
	private Tween[] noteTweens;
	private Tween[] batonTweens;

	bool _isPerforming;
	public void StartPerforming()
	{
		StopPerforming();
		_isPerforming = true;

		RecorderDance();
		BatonsPlaying().Forget();
		NotesDance().Forget();
	}

	public void StopPerforming()
	{
		_isPerforming = false;


		// --- Kill all tweens safely ---
		recorderTween?.Kill();
		if (noteTweens != null)
		{
			foreach (var tween in noteTweens)
				tween?.Kill();
		}
		if (batonTweens != null)
		{
			foreach (var tween in batonTweens)
				tween?.Kill();
		}

		recorderTween = null;
		noteTweens = null;
		batonTweens = null;
	}

	public void RecorderDance()
	{
		// --- Recorder: small side swings ---
		if (recorder != null)
		{
			var t = recorder.transform;

			float scaleRatio = Random.Range(1.1f, 1.4f);

			// Light breathing / pulsing effect
			var pulseTween = t.DOScale(
				Vector3.one * scaleRatio,
				performanceLoopDuration / 2.5f)
				.SetEase(Ease.InOutSine)
				.SetLoops(-1, LoopType.Yoyo);


			// Combine them so they stop together
			recorderTween = DOTween.Sequence()
				.Join(pulseTween);
		}
	}

	#region Baton Animations

	public Transform[] BatonLeftHits;
	public Transform[] BatonRightHits;

	public Transform BatonUpLeft;
	public Transform BatonUpRight;

	public async UniTask BatonsPlaying()
	{
		GameObject[] batons = { leftBaton, rightBaton };
		Transform[][] hitGroups = { BatonLeftHits, BatonRightHits };
		Transform[] upTargets = { BatonUpLeft, BatonUpRight };

		// Kill any existing tweens
		batonTweens = new Tween[batons.Length];

		for (int i = 0; i < batons.Length; i++)
		{
			var baton = batons[i];
			if (baton == null || upTargets[i] == null) continue;

			// Run each baton independently
			AnimateBaton(baton.transform, upTargets[i], hitGroups[i]).Forget();
		}
	}

	private async UniTaskVoid AnimateBaton(Transform baton, Transform upTarget, Transform[] hitTargets)
	{
		if (baton == null || upTarget == null || hitTargets == null || hitTargets.Length == 0) return;

		// Animation tuning parameters
		float upDuration = 0.20f;
		float hitDuration = 0.08f;       // faster swing down
		float impactBounceDuration = 0.2f;
		float restDelayMin = 0.02f;
		float restDelayMax = 0.08f;

		int lastIndex = -1;

		while (_isPerforming)
		{
			// --- Move Up Smoothly ---
			baton.DOLocalMove(upTarget.localPosition, upDuration)
				.SetEase(Ease.OutCubic);
			baton.DOLocalRotateQuaternion(upTarget.localRotation, upDuration)
				.SetEase(Ease.OutCubic);

			await UniTask.Delay((int)(upDuration * 1000));

			// --- Pick a Random Hit (not same as last) ---
			int newIndex;
			do
			{
				newIndex = UnityEngine.Random.Range(0, hitTargets.Length);
			} while (newIndex == lastIndex && hitTargets.Length > 1);

			lastIndex = newIndex;
			var target = hitTargets[newIndex];


			// --- Snappy Downward Hit ---
			Sequence hitSeq = DOTween.Sequence();

			hitSeq.Append(baton.DOLocalMove(target.localPosition, hitDuration)
				.SetEase(Ease.InQuad)); // sharp snap down

			hitSeq.Join(baton.DOLocalRotateQuaternion(target.localRotation, hitDuration)
				.SetEase(Ease.InQuad));

			// --- Small Impact "Punch" Effect ---
			hitSeq.Append(baton.DOPunchRotation(
				new Vector3(0, 0, UnityEngine.Random.Range(-8f, 8f)),
				impactBounceDuration,
				vibrato: 2,
				elasticity: 0.6f)
				.SetEase(Ease.OutQuad));

			// --- Optional: tiny squash on impact (cartoon exaggeration) ---
			hitSeq.Join(baton.DOScale(Vector3.one * 0.97f, impactBounceDuration / 2f)
				.SetLoops(2, LoopType.Yoyo)
				.SetEase(Ease.OutSine));

			hitSeq.Play();

			// Wait until hit + bounce done
			float totalHitTime = hitDuration + impactBounceDuration;
			await UniTask.Delay((int)(totalHitTime * 1000));

			// --- Random rest before next swing ---
			float restDelay = UnityEngine.Random.Range(restDelayMin, restDelayMax);
			await UniTask.Delay((int)(restDelay * 1000));
		}
	}

	#endregion

	#region Notes
	public async UniTask NotesDance()
	{
		GameObject[] notes = { noteSingle, noteDouble, solKey };
		noteTweens = new Tween[notes.Length];

		float bounceDuration = performanceLoopDuration / 2f;
		float staggerDelay = 0.15f; // offset between each note’s bounce start

		for (int i = 0; i < notes.Length; i++)
		{
			var obj = notes[i];
			if (obj == null) continue;
			var t = obj.transform;

			// Individual bounce sequence
			noteTweens[i] = t.DOLocalMoveY(t.localPosition.y + jumpHeight, bounceDuration)
				.SetEase(Ease.InOutSine)
				.SetLoops(-1, LoopType.Yoyo)
				.SetDelay(i * staggerDelay); // slight rhythm offset

			// Run a twirl coroutine per note
			NoteTwirlRoutine(t).Forget();
		}

		await UniTask.CompletedTask;
	}

	private async UniTaskVoid NoteTwirlRoutine(Transform note)
	{
		if (note == null) return;

		while (_isPerforming)
		{
			// Wait a random amount of time between twirls (1–3 seconds)
			await UniTask.Delay(UnityEngine.Random.Range(1000, 3000));

			// Small chance to skip this twirl, to avoid uniform timing
			if (UnityEngine.Random.value < 0.4f) continue;

			// Choose a random direction (left or right)
			int dir = UnityEngine.Random.value > 0.5f ? 1 : -1;

			// Twirl animation: smooth 360° spin around Z
			note.DOLocalRotate(
				new Vector3(note.localEulerAngles.x, note.localEulerAngles.y, note.localEulerAngles.z + 360f * dir),
				0.6f,
				RotateMode.FastBeyond360)
				.SetEase(Ease.OutCubic);

			// Add a little bounce effect on twirl
			note.DOPunchScale(Vector3.one * 0.1f, 0.3f, vibrato: 1, elasticity: 0.6f);
		}
	}
	#endregion 


	#endregion
}
