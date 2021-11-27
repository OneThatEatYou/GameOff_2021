using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class OverflowAnimator : MonoBehaviour
{
    public float height;
    public float raiseDur;
    public float startFadeoutTime;
    public float fadeoutDur;

    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        Animate();
    }

    private void Animate()
    {
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMoveY(height, raiseDur).SetRelative().SetEase(Ease.OutSine));
        s.Append(transform.DOMoveY(-height, raiseDur).SetRelative().SetEase(Ease.InSine));
        s.Insert(startFadeoutTime, text.DOColor(Color.clear, fadeoutDur).SetEase(Ease.InSine));
        s.AppendCallback(() => Destroy(gameObject));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.up * height, 0.5f);
    }
}
