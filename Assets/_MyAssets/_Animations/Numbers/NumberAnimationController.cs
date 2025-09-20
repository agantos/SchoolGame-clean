using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NumberAnimationController : MonoBehaviour
{
    [SerializeField] private AnimatedNumber[] animatedNumbers;
    [SerializeField] private RawImage image;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AnimationClip numberAnimation;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup.alpha = 0;
    }

    public async UniTask StartAnimation(int max)
    {
        if (max > animatedNumbers.Length) max = animatedNumbers.Length;

        canvasGroup.alpha = 1;
        for (int i = 0; i < max; i++)
        {
            await PlayNumber(i);
        }

        canvasGroup.alpha = 0;
    }

    private async UniTask PlayNumber(int number)
    {
        if (number < 0 || number >= animatedNumbers.Length) return;

        var numberData = animatedNumbers[number];

        // Set texture
        image.texture = numberData.image;

        // Play audio
        audioSource.Stop();
        audioSource.clip = numberData.clip;
        audioSource.Play();

        // Adjust animation speed to match audio length
        animator.speed = numberAnimation.length / numberData.clip.length;

        animator.Play("NumberAnimation", 0, 0f);
        await UniTask.Delay(Mathf.CeilToInt(numberData.clip.length * 1000));
    }

    [Serializable]
    public class AnimatedNumber
    {
        public Texture2D image;
        public AudioClip clip;
    }
}
