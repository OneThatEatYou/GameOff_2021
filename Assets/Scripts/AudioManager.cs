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

    public static AudioSource PlayAudioAtPosition(AudioClip audioClip, Vector2 position, AudioMixerGroup mixerGroup, bool autoDestroy = true)
    {
        GameObject obj = new GameObject("OneShotAudio");
        obj.transform.position = position;

        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = audioClip;
        source.spatialBlend = 0;
        source.outputAudioMixerGroup = mixerGroup;
        source.Play();

        if (autoDestroy)
        {
            GameObject.Destroy(obj, audioClip.length);
        }

        return source;
    }
}
