using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Targetable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool targetable = true;

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        BattleManager.Instance.SetHoveredTarget(this);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        BattleManager.Instance.SetHoveredTarget(null);
    }
}
