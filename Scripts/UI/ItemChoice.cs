using System.Collections;
using System.Collections.Generic;
using EVN.MapSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemChoice : MonoBehaviour
{
    public BaseGear m_GearStored;
    public TextMeshProUGUI m_ItemNameText;
    public Image m_ItemImage;

    public void ImplementItem(BaseGear generatedGear)
    {
        m_GearStored = generatedGear;
        m_ItemNameText.text = generatedGear.m_BaseGearName;
        m_ItemImage.sprite = generatedGear.m_GearSprite;
        m_ItemNameText.color = m_GearStored.m_Rarity.m_RarityColor;
    }
    
    public void ChooseItem()
    {
        InventoryManager.Instance.AddGearToInventory(m_GearStored);
        transform.root.gameObject.SetActive(false);
        MapManager.Instance.EnableMap();
    }
}
