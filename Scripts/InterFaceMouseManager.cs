using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InterFaceMouseManager : MonoBehaviour
{
    #region Instancing
    public static InterFaceMouseManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InterFaceMouseManager>();
                if (_instance == null)
                {
                    _instance = new GameObject("InterFaceMouseManager").AddComponent<InterFaceMouseManager>();
                }
            }
            return _instance;
        }
    }

    private static InterFaceMouseManager _instance;

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

    public InventorySlot m_SlotHolding;
    public Transform m_ChildFollowing;
    public bool m_ShouldMakeChildFollow;
    public BaseGear m_GearHolding;
    public InventorySlot m_CurrentlyOver;
    void Update()
    {
        if (m_ShouldMakeChildFollow & m_ChildFollowing != null)
        {
            m_ChildFollowing.position = Input.mousePosition;
            if (Input.GetMouseButtonUp(0))
            {
                OnMouseUpCustom();
            }
        }
    }

    public void OnMouseUpCustom()
    {
            
        if(m_CurrentlyOver != null && m_CurrentlyOver != m_SlotHolding)
            m_CurrentlyOver.SelectSlot();
        Destroy(m_ChildFollowing.gameObject);
        m_ShouldMakeChildFollow = false;
        m_GearHolding = null;
    }

    public void MakeNewFollowingItem(GameObject objectToChild)
    {
        if (Input.GetMouseButtonUp(0))
            return; 
        m_ChildFollowing = Instantiate(objectToChild, objectToChild.transform.position,objectToChild.transform.rotation,transform).transform;
        m_SlotHolding = objectToChild.GetComponentInChildren<InventorySlot>();
        InventorySlot inventorySlot = m_ChildFollowing.GetComponentInChildren<InventorySlot>();
        
        if (inventorySlot != null)
        {
            m_GearHolding = inventorySlot.m_ItemInHere;
            Destroy(inventorySlot);
            Destroy(inventorySlot.GetComponentInChildren<Collider>());
        }
        Image[] allImages = m_ChildFollowing.GetComponentsInChildren<Image>();
        foreach (var image in allImages)
        {
            image.raycastTarget = false;
        }
        
        m_ShouldMakeChildFollow = true;
    }
}
