using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManagerHoverNotification : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
	public void OnPointerEnter(PointerEventData eventData)
	{
		UIManager.Instance.m_HoveringOverUI = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		UIManager.Instance.m_HoveringOverUI = false;
	}
}
