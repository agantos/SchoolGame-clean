using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCR_SoundPool", menuName = "ScriptableObjects/SoundPool")]
public class SCR_SoundPool : ScriptableObject
{
    [Serializable]
    public class SoundData
    {
        public SoundType soundType;
        public string tag;
        public AudioClip Clip;
        public float Volume = 1.0f;
    }

    public SoundData[] sounds;

    // helper method to fetch a clip by enum
    public AudioClip GetClip(SoundType type)
    {
        foreach (var sound in sounds)
        {
            if (sound.soundType == type)
                return sound.Clip;
        }
        return null;
    }

    public SoundData[] GetTypingSounds()
    {
        return Array.FindAll(sounds, sound => sound.tag.StartsWith("Typing", StringComparison.OrdinalIgnoreCase));
    }

    public float GetVolume(SoundType type)
    {
        foreach (var sound in sounds)
        {
            if (sound.soundType == type)
                return sound.Volume;
        }
        return 1.0f;
    }
}

public enum SoundType
{
    MenuMusic,
    BattleTheme,
    Victory,
    Defeat
}