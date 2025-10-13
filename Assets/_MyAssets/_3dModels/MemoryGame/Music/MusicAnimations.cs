using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class MusicAnimations : MonoBehaviour
{
	[Header("Game Objects")]

	[SerializeField] GameObject noteSingle;
	[SerializeField] GameObject noteDouble;
	[SerializeField] GameObject solKey;

	[SerializeField] GameObject xylophone;
	[SerializeField] GameObject baton1;
	[SerializeField] GameObject baton2;

	[SerializeField] GameObject recorder;

	public bool playAwake;
	public bool playMove;
	public void Update()
	{
		if (playAwake)
		{
			AwakeObjects();
			playAwake = false;
		}

		if (playMove) {
			playMove = false;
			MoveToPositions();
		}
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
			xylophone, baton1, baton2, recorder
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

	#region MoveToPositions
	[Header("Target Positions")]
	[SerializeField] Transform noteSingleTarget;
	[SerializeField] Transform noteDoubleTarget;
	[SerializeField] Transform solKeyTarget;
	[SerializeField] Transform xylophoneTarget;
	[SerializeField] Transform baton1Target;
	[SerializeField] Transform baton2Target;
	[SerializeField] Transform recorderTarget;

	[SerializeField] float moveDuration = 0.8f;
	[SerializeField] float rotateDuration = 0.8f;
	[SerializeField] float staggerDelay = 0.1f;
	[SerializeField] Ease moveEase = Ease.OutBack;
	[SerializeField] Ease rotateEase = Ease.OutCubic;

	public async UniTask MoveToPositions()
	{
		(GameObject obj, Transform target)[] pairs = {
			(noteSingle, noteSingleTarget),
			(noteDouble, noteDoubleTarget),
			(solKey, solKeyTarget),
			(xylophone, xylophoneTarget),
			(baton1, baton1Target),
			(baton2, baton2Target),
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
}
