using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Targetable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool targetable = true;

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        PlayerSelectionHandler.Instance.SetHoveredTarget(this);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        PlayerSelectionHandler.Instance.SetHoveredTarget(null);
    }
}
