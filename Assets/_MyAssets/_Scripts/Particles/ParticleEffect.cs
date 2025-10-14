using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
	Vector3 originalPosition;
	List<ParticleSystem> _particleSystemsList;

	private void Awake()
	{
		_particleSystemsList = GetComponentsInChildren<ParticleSystem>().ToList<ParticleSystem>();

		foreach (ParticleSystem p in _particleSystemsList)
		{
			p.gameObject.SetActive(false);
		}
	}

	public async UniTaskVoid PlayDisplaced(
		float maxDisplacementX = 0f, float maxDisplacementY = 0f, float maxDisplacementZ = 0f)
	{
		if (_particleSystemsList == null || _particleSystemsList.Count == 0)
			return;

		originalPosition = transform.localPosition;

		// Random offset
		float x = Random.Range(-maxDisplacementX, maxDisplacementX);
		float y = Random.Range(-maxDisplacementY, maxDisplacementY);
		float z = Random.Range(-maxDisplacementZ, maxDisplacementZ);

		transform.localPosition += new Vector3(x, y, z);

		// Activate and play
		foreach (var p in _particleSystemsList)
		{
			p.gameObject.SetActive(true);

			// restart cleanly
			p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			p.Play(true);
		}

		// Wait one small frame for emission
		await UniTask.Delay(50, DelayType.DeltaTime);

		// Stop emission (let existing particles live)
		foreach (var p in _particleSystemsList)
			p.Stop(false, ParticleSystemStopBehavior.StopEmitting);

		// Move back to original spot
		transform.localPosition = originalPosition;
	}

	public void Play()
	{
		foreach(ParticleSystem p in _particleSystemsList)
		{
			p.gameObject.SetActive(true);
			p.Play();
		}
	}
}
