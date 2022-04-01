using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class InventorySlot : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image m_ItemIcon;
    public BaseGear m_ItemInHere;
    public EquipmentSlot m_SlotType = EquipmentSlot.Inventory;
    
    private bool m_MouseOver = false;
    void Update()
    {
        if (m_MouseOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SelectSlot();
            }
        }
    }
 
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_MouseOver = true;
        InterFaceMouseManager.Instance.m_CurrentlyOver = this;
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        m_MouseOver = false;
        if(InterFaceMouseManager.Instance.m_CurrentlyOver == this)
            InterFaceMouseManager.Instance.m_CurrentlyOver = null;
    }
    public void ClearSlot()
    {
        if(m_ItemInHere != null)
            m_ItemInHere.m_InInventorySlot = null;
        m_ItemInHere = null;
        m_ItemIcon.sprite = null;
        if(m_SlotType != EquipmentSlot.Inventory)
            m_ItemIcon.transform.parent.gameObject.SetActive(false);
        m_ItemIcon.gameObject.SetActive(false);
    }
    public void LoadItem(BaseGear gearInHere)
    {
        m_ItemInHere = gearInHere;
        LoadItemVisual();
        gearInHere.m_InInventorySlot = this;
        if(m_SlotType != EquipmentSlot.Inventory)
            m_ItemIcon.transform.parent.gameObject.SetActive(true);
        m_ItemIcon.gameObject.SetActive(true);
        
    }

    public void LoadItemVisual()
    {
        m_ItemIcon.sprite = m_ItemInHere.m_GearSprite;
    }
    public void UnLoadItem()
    {
        m_ItemInHere = null;
        m_ItemIcon.sprite = null;
        m_ItemIcon.gameObject.SetActive(false);
    }
    [Button]
    public void SelectSlot()
    {
        VisualInventoryManager visualInventoryManager = VisualInventoryManager.Instance;
        if (m_SlotType == EquipmentSlot.Inventory)
        {
            visualInventoryManager.SelectInventorySlot(this);
            InterFaceMouseManager.Instance.MakeNewFollowingItem(this.gameObject);
        }
        else
        {
            visualInventoryManager.SelectLoadoutSlot(this);
        }
    }

    public bool CheckItemAllowed(BaseGear gear)
    {
        foreach (var slotType in gear.m_AllowedEquipmentSlots)
        {
            if (slotType == m_SlotType)
                return true;
        }

        return false;
    }
}
