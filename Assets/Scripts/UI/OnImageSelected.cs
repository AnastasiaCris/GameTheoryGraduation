using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnImageSelected : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent OnHover;
    public UnityEvent OnHoverExit;


    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverExit.Invoke();
    }
}
