using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_GoldText;

    private ResourceManager m_ResourceManager;

    private void Start()
    {
        m_ResourceManager = ResourceManager.m_ResourceManager;
        m_ResourceManager.m_GoldAmountChange += UpdateText;
        UpdateText();
    }

    public void UpdateText()
    {
        m_GoldText.text = "" + m_ResourceManager.m_GoldCount;
    }

    private void OnDestroy()
    {
        m_ResourceManager.m_GoldAmountChange -= UpdateText;
    }
}
