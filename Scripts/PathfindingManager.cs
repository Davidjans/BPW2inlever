using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EVN.Entities;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using StackedBeans.Utils;

namespace EVN.Movement
{
	public class PathfindingManager : SerializedMonoBehaviour
	{
		#region Instancing
		public static PathfindingManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<PathfindingManager>();
					if (_instance == null)
					{
						_instance = new GameObject("PathfindingManager").AddComponent<PathfindingManager>();
					}
				}
				return _instance;
			}
		}

		private static PathfindingManager _instance;
		#endregion
		public int m_MoveStraightCost = 10;
		public int m_MoveDiagonalCost = 14;

		public float m_CellSize = 1;
		public Vector2 m_GridSize = new Vector2(5, 5);
		private List<MovementCell> m_OpenList;
		private List<MovementCell> m_ClosedList;
		public Grid m_Grid;

		public Dictionary<(int, int), GridCell> m_DebugDictionary;
		public MovementCell m_CurrentlyHoveringOver;
		public EntityMovement m_CurrentMovingCharacter;
		public LineRenderer m_ReachableLineRenderer;
		public LineRenderer m_NonReachableLineRenderer;
		public bool m_DebugOnStart;
		public Path m_CurrentPath;

		public delegate void CellDelegate(MovementCell cell, Vector2Int pos);
		public CellDelegate OnMouseEnter;
		public CellDelegate OnMouseOver;
		public CellDelegate OnMouseExit;
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

			if (m_DebugOnStart)
				RecreateGrid();
		}

		public void Update()
		{
			Vector3 mouseWorldPosition = StackedBeansUtils.GetMouseWorldPositionWithRay();
			Vector2Int xYPos = m_Grid.GetXYBasedOnWorldPos(mouseWorldPosition);
			if (m_Grid.m_Grid.ContainsKey((xYPos.x, xYPos.y)))
			{
				MovementCell cell = (MovementCell)m_Grid.m_Grid[(xYPos.x, xYPos.y)];
				if (m_CurrentlyHoveringOver != cell)
				{
					OnMouseExit?.Invoke(m_CurrentlyHoveringOver,xYPos);
                    m_CurrentlyHoveringOver.OnMouseHoverExit();
				}
				cell.OnMouseHoverEnter();
				OnMouseEnter?.Invoke(cell,xYPos);
				m_CurrentlyHoveringOver = cell;
				OnMouseOver?.Invoke(m_CurrentlyHoveringOver,xYPos);
				cell.OnMouseHover();
			}
		}

		[Button]
		public void RecreateGrid()
		{
			m_Grid = new Grid(this, m_CellSize, m_GridSize, nameof(MovementCell));
			m_DebugDictionary = m_Grid.m_Grid;
		}

		

		public void VisualizeCurrentPath()
		{
			List<Vector3> tempPosList = new List<Vector3>();
			tempPosList.Add(m_CurrentPath.m_StartPos);
			foreach (var pos in m_CurrentPath.m_ReachableByCurrentCharacter)
			{
				tempPosList.Add(pos.GetCellCenter());
			}

			m_ReachableLineRenderer.positionCount = tempPosList.Count;
			m_ReachableLineRenderer.SetPositions(tempPosList.ToArray());
			List<Vector3> nonReachableTempList = new List<Vector3>();
			foreach (var tempPos in m_CurrentPath.m_NonReachableByCurrentCharacter)
			{
				nonReachableTempList.Add(tempPos.GetCellCenter());
			}

			if (nonReachableTempList.Count == 0)
			{
				m_NonReachableLineRenderer.enabled = false;
			}
			else
			{
				nonReachableTempList.Insert(0, tempPosList.Last());
				m_NonReachableLineRenderer.enabled = true;
			}

			m_NonReachableLineRenderer.positionCount = nonReachableTempList.Count;
			m_NonReachableLineRenderer.SetPositions(nonReachableTempList.ToArray());
		}

		public void MoveEntity()
		{
			if (m_CurrentPath != null && (CanReachEndOfPath() || m_CurrentMovingCharacter.m_CompletePathAsFarAsPossible))
			{
				m_CurrentMovingCharacter.MoveFromPositionList(m_CurrentPath);
			}
			else
			{
				m_CurrentPath.m_CellList.Last()?.m_CellImage.transform.QuakeEffect(new Vector3(0.15f, 0, 0.15f),
					new Vector3(0.15f, 0, 0.15f), 0.075f, 0.5f, 0.01f, true);
			}
		}

		private bool CanReachEndOfPath()
		{
			return m_CurrentPath.m_ReachableByCurrentCharacter[0].m_FCost <
			       m_CurrentMovingCharacter.GetComponent<Entity>().m_AttributeDictionary["Action"].m_CurrentValue *
			       m_CurrentMovingCharacter.GetComponent<Entity>().m_AttributeDictionary["Movement"].m_CurrentValue;
		}

		public Path FindPathWorldPos(Vector3 startPosition, Vector3 endPosition)
		{
			Vector2 startXY = m_Grid.GetXYBasedOnWorldPos(startPosition);
			Vector2 endXY = m_Grid.GetXYBasedOnWorldPos(endPosition);
			return FindPath((int) startXY.x, (int) startXY.y, (int) endXY.x, (int) endXY.y);
		}

		public Path FindPath(int startX, int startY, int endX, int endY)
		{
			m_CurrentPath = new Path();
			MovementCell startCell = m_Grid.m_Grid[(startX, startY)] as MovementCell;
			m_CurrentPath.m_StartPos = startCell.GetCellCenter();
			MovementCell endCell = m_Grid.m_Grid[(endX, endY)] as MovementCell;

			if (startCell == null || endCell == null)
			{
				// Invalid Path
				return null;
			}

			m_OpenList = new List<MovementCell> {startCell};
			m_ClosedList = new List<MovementCell>();
			for (int x = 0; x < m_GridSize.x; x++)
			{
				for (int y = 0; y < m_GridSize.y; y++)
				{
					MovementCell movementCell = m_Grid.m_Grid[(x, y)] as MovementCell;
					movementCell.m_GCost = int.MaxValue;
					movementCell.CalculateFCost();
					movementCell.m_CameFromCell = null;
				}
			}

			startCell.m_GCost = 0;
			startCell.m_HCost = CalculateDistanceCost(startCell, endCell);
			startCell.CalculateFCost();

			while (m_OpenList.Count > 0)
			{
				MovementCell currentCell = GetLowestFCostCell(m_OpenList);
				if (currentCell == endCell)
				{
					return CalculatePath(endCell);
				}

				m_OpenList.Remove(currentCell);
				m_ClosedList.Add(currentCell);

				foreach (MovementCell neighbourCell in GetNeighbourList(currentCell))
				{
					if (m_ClosedList.Contains(neighbourCell)) continue;
					if (!neighbourCell.m_AboveWorldObject.m_IsWalkable)
					{
						m_ClosedList.Add(neighbourCell);
						continue;
					}

					int thisCellCost = CalculateDistanceCost(currentCell, neighbourCell);
					int tentativeGCost = currentCell.m_GCost + thisCellCost;
					if (tentativeGCost < neighbourCell.m_GCost)
					{
						neighbourCell.m_CameFromCell = currentCell;
						neighbourCell.m_GCost = tentativeGCost;
						neighbourCell.m_HCost = CalculateDistanceCost(neighbourCell, endCell);
						neighbourCell.m_CostToGoToThisOne = thisCellCost;
						neighbourCell.CalculateFCost();
						if (!m_OpenList.Contains(neighbourCell))
						{
							m_OpenList.Add(neighbourCell);
						}
					}
				}
			}

			Debug.LogError("invalid path");
			m_CurrentPath = null;
			return null;
		}

		private Path CalculatePath(MovementCell endCell)
		{
			float currentPlayerMovementLimit =
				m_CurrentMovingCharacter.GetComponent<Entity>().m_AttributeDictionary["Action"].m_CurrentValue *
				m_CurrentMovingCharacter.GetComponent<Entity>().m_AttributeDictionary["Movement"].m_CurrentValue;

			MovementCell currentCell = endCell;
			while (currentCell.m_CameFromCell != null)
			{
				m_CurrentPath.m_VectorList.Add(currentCell.GetCellCenter());
				m_CurrentPath.m_CellList.Add(currentCell);

				currentCell = currentCell.m_CameFromCell;
			}

			m_CurrentPath.m_CellList.Reverse();
			m_CurrentPath.m_VectorList.Reverse();
			foreach (var cell in m_CurrentPath.m_CellList)
			{
				if (cell.m_GCost < currentPlayerMovementLimit)
				{
					m_CurrentPath.m_ReachableByCurrentCharacter.Add(cell);
				}
				else
				{
					m_CurrentPath.m_NonReachableByCurrentCharacter.Add(cell);
				}
			}
			m_CurrentPath.m_PathFCost = currentCell.m_FCost;
			return m_CurrentPath;
		}

		private int CalculateDistanceCost(MovementCell start, MovementCell end)
		{
			int xDistance = Mathf.Abs((int) start.m_CellDictionaryPosition.x - (int) end.m_CellDictionaryPosition.x);
			int yDistance = Mathf.Abs((int) start.m_CellDictionaryPosition.y - (int) end.m_CellDictionaryPosition.y);
			int remaining = Mathf.Abs(xDistance - yDistance);
			return m_MoveDiagonalCost * Mathf.Min(xDistance, yDistance) + m_MoveStraightCost * remaining;
		}

		private MovementCell GetLowestFCostCell(List<MovementCell> cellList)
		{
			MovementCell lowestFCostCell = cellList[0];
			for (int i = 0; i < cellList.Count; i++)
			{
				if (cellList[i].m_FCost < lowestFCostCell.m_FCost)
				{
					lowestFCostCell = cellList[i];
				}
			}

			return lowestFCostCell;
		}

		private List<MovementCell> GetNeighbourList(MovementCell currentCell)
		{
			List<MovementCell> neighbourList = new List<MovementCell>();

			if (currentCell.m_CellDictionaryPosition.x - 1 >= 0)
			{
				// left
				neighbourList.Add((MovementCell) m_Grid.m_Grid[
					(currentCell.m_CellDictionaryPosition.x - 1, currentCell.m_CellDictionaryPosition.y)]);

				// left down
				if (currentCell.m_CellDictionaryPosition.y - 1 >= 0)
					neighbourList.Add((MovementCell) m_Grid.m_Grid[
						(currentCell.m_CellDictionaryPosition.x - 1, currentCell.m_CellDictionaryPosition.y - 1)]);
				// left up
				if (currentCell.m_CellDictionaryPosition.y + 1 < m_Grid.m_GridSize.y)
					neighbourList.Add((MovementCell) m_Grid.m_Grid[
						(currentCell.m_CellDictionaryPosition.x - 1, currentCell.m_CellDictionaryPosition.y + 1)]);
			}

			if (currentCell.m_CellDictionaryPosition.x + 1 < m_Grid.m_GridSize.x)
			{
				// right
				neighbourList.Add((MovementCell) m_Grid.m_Grid[
					(currentCell.m_CellDictionaryPosition.x + 1, currentCell.m_CellDictionaryPosition.y)]);

				// right down
				if (currentCell.m_CellDictionaryPosition.y - 1 >= 0)
					neighbourList.Add((MovementCell) m_Grid.m_Grid[
						(currentCell.m_CellDictionaryPosition.x + 1, currentCell.m_CellDictionaryPosition.y - 1)]);
				// right up
				if (currentCell.m_CellDictionaryPosition.y + 1 < m_Grid.m_GridSize.y)
					neighbourList.Add((MovementCell) m_Grid.m_Grid[
						(currentCell.m_CellDictionaryPosition.x + 1, currentCell.m_CellDictionaryPosition.y + 1)]);
			}

			// Down
			if (currentCell.m_CellDictionaryPosition.y - 1 >= 0)
				neighbourList.Add((MovementCell) m_Grid.m_Grid[
					(currentCell.m_CellDictionaryPosition.x, currentCell.m_CellDictionaryPosition.y - 1)]);
			// Up
			if (currentCell.m_CellDictionaryPosition.y + 1 < m_Grid.m_GridSize.y)
				neighbourList.Add((MovementCell) m_Grid.m_Grid[
					(currentCell.m_CellDictionaryPosition.x, currentCell.m_CellDictionaryPosition.y + 1)]);

			return neighbourList;
		}
	}

	[Serializable]
	public class Path
	{
		public Vector3 m_StartPos;
		public List<MovementCell> m_CellList = new List<MovementCell>();
		public List<Vector3> m_VectorList = new List<Vector3>();
		public List<MovementCell> m_ReachableByCurrentCharacter = new List<MovementCell>();
		public List<MovementCell> m_NonReachableByCurrentCharacter = new List<MovementCell>();
		public float m_PathFCost = 0;
	}
}