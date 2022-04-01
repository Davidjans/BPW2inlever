using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
public class VisualInventoryManager : MonoBehaviour
{
    #region Instancing
    public static VisualInventoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<VisualInventoryManager>();
                if (_instance == null)
                {
                    _instance = new GameObject("VisualInventoryManager").AddComponent<VisualInventoryManager>();
                }
            }
            return _instance;
        }
    }

    private static VisualInventoryManager _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        if (_instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }
    #endregion
    public List<InventorySlot> m_InventorySlots = new List<InventorySlot>();
    public InventorySlot m_SelectedSlot;
    public InventorySlot m_PreviouslySelectedSlot;
    public Image m_SelectedItemImage;
    

    public void SetVisualInventory()
    {
        foreach (var slot in m_InventorySlots)
        {
            slot.ClearSlot();
            if (slot.m_ItemInHere == null)
            {
                break;
            }
        }
        //InventoryManager.Instance.LoadInventory();
        for (int i = 0; i < InventoryManager.Instance.m_OwnedGear.Count; i++)
        {
            m_InventorySlots[i].LoadItem(InventoryManager.Instance.m_OwnedGear[i]);
        }
    }
   
    public void SelectSlot(InventorySlot slotSelected)
    {
        if (slotSelected == m_SelectedSlot)
        {
            m_SelectedItemImage.gameObject.SetActive(false);
            m_SelectedSlot = null;
            return;
        }
        else
        {
            m_SelectedItemImage.gameObject.SetActive(true);
        }
        if(m_SelectedSlot != null)
            m_PreviouslySelectedSlot = m_SelectedSlot;
        
        
        m_SelectedSlot = slotSelected;
        //m_SelectedSlot.gameObject.SetActive(true);
        m_SelectedItemImage.transform.SetParent(m_SelectedSlot.transform,true);
        m_SelectedItemImage.transform.position = m_SelectedSlot.transform.position;
        m_SelectedItemImage.transform.parent = transform.root;
    }
    public void SelectLoadoutSlot(InventorySlot slotSelected)
    {
        SelectSlot(slotSelected);
        if (m_SelectedSlot == null)
            return;
        if (m_PreviouslySelectedSlot != null && m_PreviouslySelectedSlot.m_SlotType == EquipmentSlot.Inventory &&
            m_PreviouslySelectedSlot.m_ItemInHere != null && 
            m_SelectedSlot.CheckItemAllowed(m_PreviouslySelectedSlot.m_ItemInHere)
            && m_PreviouslySelectedSlot.m_ItemInHere.m_InLoadout == false)
        {
            m_SelectedSlot.LoadItem(m_PreviouslySelectedSlot.m_ItemInHere);
            m_PreviouslySelectedSlot.m_ItemInHere.m_InLoadout = true;
        }
        EquipmentManager.Instance.m_PlayerEquipment.ChangeSlot(m_SelectedSlot.m_SlotType,m_SelectedSlot.m_ItemInHere);
    }
    public void SelectInventorySlot(InventorySlot slotSelected)
    {
        SelectSlot(slotSelected);
        if (m_PreviouslySelectedSlot != null && m_PreviouslySelectedSlot.m_SlotType != EquipmentSlot.Inventory  &&
            slotSelected.m_ItemInHere == null && m_PreviouslySelectedSlot.m_ItemInHere != null)
        {
            m_PreviouslySelectedSlot.m_ItemInHere.m_InLoadout = false;
            m_PreviouslySelectedSlot.UnLoadItem();
        }
    }
    [Button]
    public void GetAllChildrenSlots()
    {
        m_InventorySlots.Clear();
        m_InventorySlots = GetComponentsInChildren<InventorySlot>().ToList();
    }
    public void UnSelectSlot()
    {
        
    }
}
