using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Targetable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool targetable = true;

    public delegate void PointerCallback();
    public PointerCallback onPointerEnter;
    public PointerCallback onPointerExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetable)
        {
            PlayerSelectionHandler.Instance.SetHoveredTarget(this);
            onPointerEnter?.Invoke();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerSelectionHandler.Instance.SetHoveredTarget(null);
        onPointerExit?.Invoke();
    }
}
