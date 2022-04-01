using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using EVN.Entities;
using UnityEngine.AddressableAssets;
[Serializable]
public class MovementCell : GridCell
{
    public int m_GCost;
    public int m_HCost;
    public int m_FCost;
    public int m_CostToGoToThisOne;
    public MovementCell m_CameFromCell;
    public WorldObject m_AboveWorldObject;
    public Entity m_EntityOnCell;
    public override void CellStart(Grid owner, Vector3 worldPosition, float cellSize,string cellValue,Vector2Int cellPosition)
    {
        base.CellStart(owner,worldPosition,cellSize,cellValue, cellPosition);
        CheckOnWhatWorldObject();
    }
    
    protected override async void CreateCellText()
    {
    }
    
    public void CalculateFCost()
    {
        m_FCost = m_GCost + m_HCost;
    }

    public async void CheckOnWhatWorldObject()
    {
        Vector3 worldPosToCheck = GetCellCenter();
        worldPosToCheck.y += 50;
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        //Debug.DrawRay(worldPosToCheck,Vector3.down * 1000f,Color.red, 1000f);
        if (Physics.Raycast(worldPosToCheck, Vector3.down, out hit, Mathf.Infinity))
        {
           m_AboveWorldObject = hit.transform.gameObject.GetComponent<WorldObject>();
           if (m_AboveWorldObject != null)
           {
               
               //Debug.LogError(m_CellDictionaryPosition.x + " " + m_CellDictionaryPosition.y + "  is on world object" + m_AboveWorldObject.name);
           }
           else
           {
               //Debug.LogError(m_CellDictionaryPosition.x + " " + m_CellDictionaryPosition.y + "  is not on any world object");
           }

           m_WorldPosition.y = hit.point.y;
        }
        else
        {
            //Debug.LogError(m_CellDictionaryPosition.x + " " + m_CellDictionaryPosition.y + "  Did not hit anything");
        }
        m_WorldPosition.y += 0.0001f;
        base.CreateSprite();
        m_WorldPosition.y += 0.001f;
        
        //base.CreateCellText();
        
        if (m_AboveWorldObject != null && !m_AboveWorldObject.m_IsWalkable)
        {
            DestroySpawned();
        }
    }

    private async void DestroySpawned()
    {
        int iteration = 0;
        while (m_CellImage == null || m_CellText == null)
        {
            iteration++;
                await Task.Delay(100);
                if (iteration > 20)
                    break;
        }
        
        GameObject.Destroy(m_CellText.gameObject);
        GameObject.Destroy(m_CellImage.gameObject);
    }

    public async void OnMouseHoverEnter()
    {
        if(m_CellImage != null)
            m_CellImage.GetComponent<MeshRenderer>().material = await Addressables.LoadAssetAsync<Material>("GridIndicatorSelected").Task; 
    }
    public void OnMouseHover()
    {
        
    }
    public async void OnMouseHoverExit()
    {
        if(m_CellImage != null)
            m_CellImage.GetComponent<MeshRenderer>().material = await Addressables.LoadAssetAsync<Material>("GridIndicator").Task; 
    }
}   
