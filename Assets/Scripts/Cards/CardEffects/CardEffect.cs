using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public abstract class CardEffect : ScriptableObject
{
    [Header("Animation")]
    public bool playAnimation;
    [ConditionalField(nameof(playAnimation)), SerializeField] private AnimationClip animationClip;
    [ConditionalField(nameof(playAnimation)), SerializeField] private Vector2 animationPosOffset = new Vector2(0, 1);
    public bool useParticleEffect;
    [ConditionalField(nameof(useParticleEffect)), SerializeField] private GameObject particlePrefab;
    [ConditionalField(nameof(useParticleEffect)), SerializeField] private Vector2 particleSpawnPosOffset = new Vector2(0, 1);

    [Header("Audio")]
    public bool playAudio;
    [ConditionalField(nameof(playAudio)), SerializeField] private AudioClip audioClip;

    public delegate void CardDelegate();

    public abstract float ApplyEffect(Character character, out CardDelegate callback);

    protected float PlayAnimation(Vector2 pos)
    {
        float animDur = 0;

        if (playAnimation && animationClip)
        {
            Vector2 animSpawnPos = pos + animationPosOffset;
            //Debug.Log("Playing animation at " + animSpawnPos);
            AnimationManager.PlayAnimation(animationClip, animSpawnPos);
            animDur = animationClip.length;
        }

        if (useParticleEffect && particlePrefab)
        {
            Vector2 particleSpawnPos = pos + particleSpawnPosOffset;
            //Debug.Log("Spawning particle effect at " + particleSpawnPos);
            GameObject.Instantiate(particlePrefab, particleSpawnPos, Quaternion.identity);
            float particleSystemDur = particlePrefab.GetComponent<ParticleSystem>().main.duration;

            if (animDur < particleSystemDur) animDur = particleSystemDur;
        }

        if (playAudio && audioClip)
        {
            AudioManager.PlayAudioAtPosition(audioClip, pos, AudioManager.SFXGroup);
        }

        return animDur;
    }
}
