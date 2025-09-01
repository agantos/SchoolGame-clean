using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;


public class SoundManager : MonoBehaviour
{
    [Header("Sound Data")]
    [SerializeField] private SCR_SoundPool SoundPool;

    [Header("Audio Source Pool Settings")]
    [SerializeField] private GameObject _audioSourcePrefab;
    [SerializeField] private int _initialPoolSize = 10;

    private float _defaultVolume = 1.0f;
    private Queue<AudioSource> _audioSourcePool;
    private Coroutine _typingSoundCoroutine;

    public float GetVolume() => _defaultVolume;

    public float SetVolume(float value) => _defaultVolume = value;

    private void Awake()
    {
        _audioSourcePool = new Queue<AudioSource>();

        for (int i = 0; i < _initialPoolSize; i++)
        {
            AddGameObjectToPool();
        }
    }

    public void PlaySound(SoundType type)
    {        
        AudioSource audioSource = GetPooledAudioSource();
        audioSource.gameObject.SetActive(true);
        audioSource.clip = SoundPool.GetClip(type);
        audioSource.volume = SoundPool.GetVolume(type) * _defaultVolume;
        audioSource.enabled = true;
        audioSource.Play();
        ReturnToPoolAfterPlayback(audioSource, SoundPool.GetClip(type).length).Forget();

        return;            
    }

    public AudioSource PlayTypingSound()
    {
        var soundData = SoundPool.GetTypingSounds();
        AudioSource audioSource = GetPooledAudioSource();

        if (soundData.Length > 0)
        {
            audioSource.gameObject.SetActive(true);
            audioSource.enabled = true;
            _typingSoundCoroutine = StartCoroutine(PlayTypingLoop(audioSource, soundData));

            return audioSource;
        }

        return null;
    }

    private IEnumerator PlayTypingLoop(AudioSource source, SCR_SoundPool.SoundData[] soundData)
    {
        while (true)
        {
            int randomIndex = UnityEngine.Random.Range(0, soundData.Length);
            source.volume = soundData[randomIndex].Volume * _defaultVolume;
            source.clip = soundData[randomIndex].Clip;
            source.Play();

            yield return new WaitForSeconds(source.clip.length);
        }
    }

    // Function to stop typing sounds
    public void StopTypingSound(AudioSource source)
    {
        if (source != null)
        {
            source.Stop();
            source.clip = null;
            StopCoroutine(_typingSoundCoroutine);
        }
    }

    private AudioSource GetPooledAudioSource()
    {
        if (_audioSourcePool.Count > 0)
        {
            return _audioSourcePool.Dequeue();
        }

        AddGameObjectToPool();
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        return newSource;
    }

    private void AddGameObjectToPool()
    {
        AudioSource source = Instantiate(_audioSourcePrefab, transform).GetComponent<AudioSource>();
        source.playOnAwake = false;
        _audioSourcePool.Enqueue(source);
    }

    private async UniTaskVoid ReturnToPoolAfterPlayback(AudioSource source, float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        source.Stop();
        source.clip = null;
        _audioSourcePool.Enqueue(source);
    }

    public void ReturnToPool(AudioSource source)
    {
        if (source == null) return;

        source.Stop();
        source.clip = null;
        _audioSourcePool.Enqueue(source);
    }
}
