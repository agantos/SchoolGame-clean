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

	public async UniTask SlideTabletIn()
	{
		if (!isOpen || isAnimating) return;

		animator.SetTrigger("SlideIn");
	
		isAnimating = true;
		isOpen = false;

		await WaitForState("Idle");

		isAnimating = false;
	}

	private async UniTask WaitForState(string stateName)
	{
		while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
			await UniTask.Yield();

		float animLength = animator.GetCurrentAnimatorStateInfo(0).length;

		await UniTask.Delay(TimeSpan.FromSeconds(animLength));
	}
}
