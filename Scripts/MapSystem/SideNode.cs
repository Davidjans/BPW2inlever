using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SideNode : MonoBehaviour
{
    public Image m_AdditionalInfoIcon;
    private Image m_ThisImage;

    private void Start()
    {
        m_ThisImage = GetComponent<Image>();
    }

    private void Update()
    {
        m_AdditionalInfoIcon.rectTransform.sizeDelta = m_ThisImage.rectTransform.sizeDelta * 0.5f;
    }
}
