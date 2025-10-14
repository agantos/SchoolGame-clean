using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class CabinetContentsAnimations_Base : MonoBehaviour
{
	[SerializeField] ParticleEffect CorrectEffect;

	public abstract UniTask PlayCorrectAnimation();
	public abstract UniTask PlayWrongAnimation();
	public abstract UniTask PlayIntroductionAnimation();

}
