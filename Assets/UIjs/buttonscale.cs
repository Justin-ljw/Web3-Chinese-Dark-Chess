using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class buttonscale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * 0.8f;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
    }
    public void OnPointerEnter(PointerEventData eventData)//��������UI��ִ�е��¼�ִ�е�
    {
        transform.localScale = Vector3.one * 1.2f;
    }
    public void OnPointerExit(PointerEventData eventData)//������뿪UI��ִ�е��¼�ִ�е�
    {
        transform.localScale = Vector3.one;
    }

}