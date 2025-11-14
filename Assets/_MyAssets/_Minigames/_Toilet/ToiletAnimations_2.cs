using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.XR;
using Random = UnityEngine.Random;

public class ToiletAnimations_2 : MonoBehaviour
{
	[Header("Hand Transforms SINK")]
	public Transform HandFront;
	public Transform HandBack;

	[Header("Hand Transforms SINK")]
	public Transform HandFrontStart;
	public Transform HandFrontEnd;
	public Transform HandBackStart;
	public Transform HandBackEnd;

	[Header("SINK Animation Settings")]
	public float animationDuration = 0.5f;
	public Ease movementEase = Ease.InOutSine;

	[Space]
	[Space]

	[Header("Hand Transforms Towel")]
	public Transform HandFront_towel;
	public Transform HandBack_towel;

	[Header("Hand Transforms Towel")]
	public Transform HandFrontStart_towel;
	public Transform HandFrontEnd_towel;
	public Transform HandBackStart_towel;
	public Transform HandBackEnd_towel;

	[Header("Towel Models")]
	public Transform TowelMove;
	public Transform TowelEnd;
	public Transform UsedTowelEndPosition;

	[Space]
	[Space]

	[Header("Wash Hands")]
	public Transform foamInside;
	public Transform foamOutside;
	public Transform HandFrontEnd_wash;
	public Transform HandBackEnd_wash;
	[Space]
	[Space]

	private Sequence _sequence;

	public bool playTogether;
	public bool playApart;
	public bool playTowel;
	public bool throwTrash;

	private void Start()
	{
		foamInside.gameObject.SetActive(false);
		foamOutside.gameObject.SetActive(false);

		HandFront_towel.gameObject.SetActive(false);
		HandBack_towel.gameObject.SetActive(false);
	}

	private async void Update()
	{
		if (playTogether)
		{
			playTogether = false;
			await HandsTogetherAnimation_Sink();
		}

		if (playApart)
		{
			playApart = false;
			await HandsApartAnimation_Sink();
		}

		if (playTowel)
		{
			playTowel = false;
			await DryHandsAnimation();

		}

		if (throwTrash)
		{
			throwTrash = false;
			await WashHandsAnimations();

		}
	}

	// ----------------------- Public Hand Animations -----------------------

	public async UniTask DryHandsAnimation()
	{

		HandFront_towel.gameObject.SetActive(true);
		HandBack_towel.gameObject.SetActive(true);

		// Get Towel Out
		await AnimateTowel();
		await UniTask.Delay(400);
		await HandsTogetherAnimation_Towel();
		await UniTask.Delay(300);

		//Dry Hands
		WiggleHandAsync(HandFront_towel);
		await UniTask.Delay(100);
		Vector3 towelOriginalScale = TowelMove.localScale;
		Quaternion towelOriginalRotation = TowelMove.rotation;

		await WiggleHandAsync(HandBack_towel, () => { 
			TowelMove.rotation = Quaternion.Euler(towelOriginalRotation.eulerAngles.x + Random.Range(-15, 15), towelOriginalRotation.eulerAngles.y + Random.Range(-15, 15), Random.Range(-45, 45));
			if (Random.Range(0f, 1f) < 0.8f)
				TowelMove.localScale /= 1.08f;
			else
				TowelMove.localScale *= 1.05f;
		});

		// Despawn Hands
		HandFront_towel.gameObject.SetActive(false);
		HandBack_towel.gameObject.SetActive(false);

		// Spawn Used Model
		TowelMove.localScale = towelOriginalScale;
		TowelMove.rotation = towelOriginalRotation;

		TowelMove.GetChild(0).gameObject.SetActive(false);
		TowelMove.GetChild(1).gameObject.SetActive(true);
	}

	public async UniTask WashHandsAnimations()
	{
		HandBack.gameObject.SetActive(true);
		HandFront.gameObject.SetActive(true);

		HandBack.GetComponent<SpriteRenderer>().sortingOrder += 10;
		HandFront.GetComponent<SpriteRenderer>().sortingOrder += 10;
		foamInside.GetComponent<SpriteRenderer>().sortingOrder += 10;
		foamOutside.GetComponent<SpriteRenderer>().sortingOrder += 10;


		durationPerMove = 0.4f;
		steps = 25;
		xWiggleRange = 0.012f;
		yWiggleRange = 0.012f;

		await HandsTogetherAnimation_Wash();

		WiggleHandAsync(HandFront);
		await UniTask.Delay(100);


		int i = 0;
		await WiggleHandAsync(HandBack, () =>
		{
			foamInside.gameObject.SetActive(true);
			if(i == 8)
				foamOutside.gameObject.SetActive(true);
			i++;

			foamInside.localScale *= 1.03f;
			foamOutside.localScale *= 1.05f;

		});
		await UniTask.Delay(700);

		await AnimateTwoHandsAsync(HandFront, HandBack, HandFront, HandFrontEnd, HandBack, HandBackEnd);
		await UniTask.Delay(100);

		durationPerMove = 0.4f;
		steps = 15;

		HandBack.GetComponent<SpriteRenderer>().sortingOrder -= 10;
		HandFront.GetComponent<SpriteRenderer>().sortingOrder -= 10;
		foamInside.GetComponent<SpriteRenderer>().sortingOrder -= 10;
		foamOutside.GetComponent<SpriteRenderer>().sortingOrder -= 10;

		WiggleHandAsync(HandFront);
		await UniTask.Delay(100);
		await WiggleHandAsync(HandBack, () =>
		{
			foamInside.localScale /= 1.2f;
			foamOutside.localScale /= 1.2f;

		});
		
		foamOutside.gameObject.SetActive(false);
		foamInside.gameObject.SetActive(false);
	}

	public async UniTask ThrowTrashAnimation()
	{
		_sequence?.Kill();
		_sequence = DOTween.Sequence();

		_sequence.Join(MoveItemAsync(TowelMove.GetChild(1), UsedTowelEndPosition.position, UsedTowelEndPosition.rotation));
		await _sequence.Play().AsyncWaitForCompletion();
	}

	public async UniTask HandsTogetherAnimation_Sink()
	{
		await AnimateTwoHandsAsync(HandFront, HandBack, HandFrontStart, HandFrontEnd, HandBackStart, HandBackEnd);
	}

	public async UniTask HandsApartAnimation_Sink()
	{
		await AnimateTwoHandsAsync(HandFront, HandBack, HandFrontEnd, HandFrontStart, HandBackEnd, HandBackStart);
	}

	public async UniTask HandsTogetherAnimation_Towel()
	{
		await AnimateTwoHandsAsync(HandFront_towel, HandBack_towel, HandFrontStart_towel, HandFrontEnd_towel, HandBackStart_towel, HandBackEnd_towel);
	}

	public async UniTask HandsTogetherAnimation_Wash()
	{
		await AnimateTwoHandsAsync(HandFront, HandBack, HandFrontStart, HandFrontEnd_wash, HandBackStart, HandBackEnd_wash);
	}

	public async UniTask AnimateTowel()
	{
		_sequence?.Kill();
		_sequence = DOTween.Sequence();

		_sequence.Join(MoveItemAsync(TowelMove, TowelEnd.position, TowelEnd.rotation));
		await _sequence.Play().AsyncWaitForCompletion();
	}

	public async UniTask WashHandsAnimation()
	{
		await HandsTogetherAnimation_Sink();

	}

	// ----------------------- Core Animation Logic -----------------------
	private async UniTask AnimateTwoHandsAsync(Transform HandFront, Transform HandBack, Transform frontStart, Transform frontEnd, Transform backStart, Transform backEnd)
	{
		if (frontStart == null || frontEnd == null || backStart == null || backEnd == null)
			return;

		// Kill any previous sequence
		_sequence?.Kill();
		_sequence = DOTween.Sequence();

		// Animate front hand
		_sequence.Join(MoveItemAsync(HandFront, frontEnd.position, frontEnd.rotation));

		// Animate back hand
		_sequence.Join(MoveItemAsync(HandBack, backEnd.position, backEnd.rotation));

		// Wait for all joined tweens to complete
		await _sequence.AsyncWaitForCompletion();
	}

	private Tween MoveItemAsync(Transform item, Vector3 targetPos, Quaternion targetRot)
	{
		// Combine move and rotate into a single DOTween sequence for this item
		var seq = DOTween.Sequence();
		seq.Append(item.DOMove(targetPos, animationDuration).SetEase(movementEase));
		seq.Join(item.DORotateQuaternion(targetRot, animationDuration).SetEase(movementEase));
		return seq;
	}

	[Header("Wiggle Settings")]
	public float durationPerMove = 0.3f;
	public int steps = 5;                     
	public float xWiggleRange = 0.018f;               
	public float yWiggleRange = 0.018f;               
	public Ease movementEase_2 = Ease.OutBack;  

	public async UniTask WiggleHandAsync(Transform Hand, Action callBack = null)
	{
		if (Hand == null) return;

		Vector3 originalPos = Hand.position;

		for (int i = 0; i < steps; i++)
		{
			// Pick a random point near the original position
			Vector3 targetPos = originalPos + new Vector3(
				Random.Range(-xWiggleRange, xWiggleRange),
				Random.Range(-yWiggleRange, yWiggleRange),
				0f // keep Z same
			);

			// Move to target
			await Hand.DOMove(targetPos, durationPerMove)
				.SetEase(movementEase_2)
				.AsyncWaitForCompletion();

			if(callBack != null) callBack();
		}

		// Return to original position at the end
		await Hand.DOMove(originalPos, durationPerMove).SetEase(Ease.OutBack).AsyncWaitForCompletion();
	}
}
