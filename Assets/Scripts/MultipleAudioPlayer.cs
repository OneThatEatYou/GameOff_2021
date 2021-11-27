using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleAudioPlayer : MonoBehaviour
{
    public AudioClip audioClip;
    public float delay;
    public float lifetime;

    private void Start()
    {
        StartCoroutine(PlayAudioClips());
    }

    IEnumerator PlayAudioClips()
    {
        float elapsed = 0;

        while (elapsed < lifetime)
        {
            AudioManager.PlayAudioAtPosition(audioClip, transform.position, AudioManager.SFXGroup);
            yield return new WaitForSeconds(delay);
            elapsed += delay;
        }

        Destroy(this);
    }
}
