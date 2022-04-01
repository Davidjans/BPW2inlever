using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[Serializable]
public class  Grid{
    public float m_CellSize = 1;
    public Vector2 m_GridSize = new Vector2(5, 5);
    public Dictionary<(int x, int y), GridCell> m_Grid = new Dictionary<(int x, int y), GridCell>();
    public MonoBehaviour m_CreatedBy;
    public string m_CellType;
    public Grid(MonoBehaviour createdBy, float cellSize, Vector2 gridSize,string cellType)
    {
        m_CreatedBy = createdBy;
        m_CellSize = cellSize;
        m_GridSize = gridSize;
        m_CellType = cellType;
        CreateGridInDictionary(cellType);
    }
    [Button]
    public void CreateGridInDictionary(string typeOfCellToCreate = nameof(GridCell))
    {
        ClearGrid();
        for (int x = 0; x < m_GridSize.x; x++)
        {
            for (int y = 0; y < m_GridSize.y; y++)
            {
                if (typeOfCellToCreate == nameof(GridCell))
                {
                    m_Grid[(x, y)] = new GridCell();
                }
                else if (typeOfCellToCreate == nameof(MovementCell))
                {
                    MovementCell cell = new MovementCell();
                    m_Grid[(x, y)] = cell ;
                }
                m_Grid[(x,y)].CellStart(this,m_CreatedBy.transform.position + new Vector3(x,0,y) * m_CellSize ,m_CellSize,
                    x+ ","+y,new Vector2Int(x,y) );
            }
        }
        Vector3 beginPos = m_CreatedBy.transform.position;
        Vector3 endPos =  beginPos +  (new Vector3(m_GridSize.x,0,m_GridSize.y)* m_CellSize);
        Debug.DrawLine(new Vector3(beginPos.x,beginPos.y,endPos.z), endPos, Color.white,100f);
        Debug.DrawLine(new Vector3(endPos.x,beginPos.y,beginPos.z), endPos, Color.white,100f);
        
    }
    
    [Button]
    public Vector2Int GetXYBasedOnWorldPos(Vector3 position)
    {
        position -= m_CreatedBy.transform.position;
        int x, y;
        x = (int)Mathf.Clamp(Mathf.FloorToInt(position.x / m_CellSize),0,m_GridSize.x);
        y = (int)Mathf.Clamp(Mathf.FloorToInt(position.z / m_CellSize),0,m_GridSize.y);
        //Debug.LogError(x + " " + y );
        return new Vector2Int(x, y);
    }

    public GridCell GetCellOnPosition<T>(Vector3 position)
    {
        Vector2 gridCellToGet = GetXYBasedOnWorldPos(position);
        return m_Grid[((int)gridCellToGet.x, (int)gridCellToGet.y)];
    }
    
    [Button]
    public void ClearGrid()
    {
        if (m_Grid.Count > 0)
        {
            for (int x = 0; x < m_GridSize.x; x++)
            {
                for (int y = 0; y < m_GridSize.y; y++)
                {
                    m_Grid[(x, y)].OnDestroy();
                }
            }
        }
        m_Grid.Clear();
    }
}
