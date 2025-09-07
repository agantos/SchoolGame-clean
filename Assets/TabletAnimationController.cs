using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class TabletAnimationController : MonoBehaviour
{
	private Animator animator;
	private bool isOpen = false;
	private bool isAnimating = false;

	// Callbacks
	public event Func<UniTask> onTabletOut;
	public event Func<UniTask> OnDoorClosed;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	public async UniTask SlideTabletOut()
	{
		if (isOpen || isAnimating) return;

		animator.SetTrigger("SlideOut");
		isAnimating = true;
		isOpen = true;

		await WaitForState("IdleTabletOut");

		isAnimating = false;
		onTabletOut?.Invoke();
	}

	//public async UniTask CloseDoor()
	//{
	//	if (!isOpen || isAnimating) return;

	//	animator.SetTrigger("CloseDoor");
	//	isAnimating = true;
	//	isOpen = false;

	//	await WaitForState("CloseDoor");

	//	isAnimating = false;
	//	OnDoorClosed?.Invoke();
	//}

	//public async UniTask ToggleDoor()
	//{
	//	if (isOpen) await CloseDoor();
	//	else await OpenDoor();
	//}

	private async UniTask WaitForState(string stateName)
	{
		while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
			await UniTask.Yield();

		float animLength = animator.GetCurrentAnimatorStateInfo(0).length;

		await UniTask.Delay(TimeSpan.FromSeconds(animLength));
	}
}
