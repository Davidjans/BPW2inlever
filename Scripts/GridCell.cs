using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Mathematics;
using StackedBeans.Utils;

[Serializable]
public class GridCell
{
    public Vector3 m_WorldPosition;
    public float m_CellSize;
    public string m_CellValue = "0";
    public Vector2Int m_CellDictionaryPosition;
    public TextMeshPro m_CellText;
    public Grid m_Owner;
    public GameObject m_CellImage;
    public virtual void CellStart(Grid owner, Vector3 worldPosition, float cellSize,string cellValue,Vector2Int cellPosition)
    {
        m_WorldPosition = worldPosition;
        m_CellSize = cellSize;
        m_CellValue = cellValue;
        m_CellDictionaryPosition = cellPosition;
        m_Owner = owner;
        CellCreation();
    }

    private async void CellCreation()
    {
        CreateCellText();
        // DrawDebugSquare();
    }

    protected virtual async void CreateCellText()
    {
        m_CellText = await StackedBeansUtils.CreateWorldTextMeshPro(null, m_CellValue,GetCellCenter(),Color.white, 8);
    }
    
    protected virtual async void CreateSprite()
    {
        m_CellImage = await StackedBeansUtils.CreateWorldImage(null, "GridIndicator",GetCellCenter(), new Vector3(90,0,0), 1);
    }
    
    public void DrawDebugSquare()
    {
        Debug.DrawLine(m_WorldPosition,new Vector3(m_WorldPosition.x,m_WorldPosition.y ,m_WorldPosition.z+m_CellSize)
            ,Color.white, 100f);
        Debug.DrawLine(m_WorldPosition,new Vector3(m_WorldPosition.x+m_CellSize,m_WorldPosition.y,m_WorldPosition.z)
            ,Color.white, 100f);
    }

    public void OnDestroy()
    {
        GameObject.DestroyImmediate(m_CellText.gameObject);
    }

    public Vector3 GetCellCenter()
    {
        return m_WorldPosition + (new Vector3(1, 0, 1) * m_CellSize) * 0.5f;
    }
}
