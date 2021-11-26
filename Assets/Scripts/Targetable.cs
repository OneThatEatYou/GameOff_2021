using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Targetable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool targetable = true;

    public delegate void PointerCallback();
    public PointerCallback onPointerEnter;
    public PointerCallback onPointerExit;
    public PointerCallback onPointerDown;
    public PointerCallback onPointerUp;
    public PointerCallback onSelected;
    public PointerCallback onUnselected;

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

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke();
    }
}
