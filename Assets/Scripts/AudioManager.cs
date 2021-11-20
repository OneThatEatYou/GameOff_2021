using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using MyBox;

public class AudioManager : Singleton<AudioManager>
{
    public AudioMixer mainMixer;
    public static AudioMixerGroup BGMGroup;
    public static AudioMixerGroup SFXGroup;

    private void Awake()
    {
        InitializeSingleton();

        BGMGroup = mainMixer.FindMatchingGroups("BGM")[0];
        SFXGroup = mainMixer.FindMatchingGroups("SFX")[0];

        if (!BGMGroup || !SFXGroup)
        {
            Debug.LogError("Error finding audio mixer group");
        }
    }
}
