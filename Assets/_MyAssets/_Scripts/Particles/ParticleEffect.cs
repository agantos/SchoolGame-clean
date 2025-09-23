using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
	List<ParticleSystem> _particleSystemsList;

	private void Awake()
	{
		_particleSystemsList = GetComponentsInChildren<ParticleSystem>().ToList<ParticleSystem>();

		foreach (ParticleSystem p in _particleSystemsList)
		{
			p.gameObject.SetActive(false);
		}
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
