using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EVN.Entities;
using Pathfinding;
using UnityEngine;

namespace EVN.Movement
{
	public class EntityMovement : MonoBehaviour
	{
		public Entity m_ParentEntity;
		public AIPath m_AIPath;
		public Seeker m_Seeker;
		public bool m_CurrentlyMoving;
		public Path m_CurrentPath;
		public PathfindingManager m_PathFindingManager;
		public bool m_CompletePathAsFarAsPossible = false;
		private int m_NodesWalked;
		public float m_WalkCostThisTurn = 0;
		public List<MovementCell> m_CellPathRemaining = new List<MovementCell>();
		public List<Vector3> m_VectorPathRemaining = new List<Vector3>();
		private void Start()
		{
			m_PathFindingManager = PathfindingManager.Instance;
			m_ParentEntity = GetComponentInParent<Entity>();
		}

		public void MoveFromPositionList(Path movementPoint)
		{
			m_CurrentPath = movementPoint;
			m_CurrentlyMoving = true;
			//m_ParentEntity.ChangeAPByValue(-CalculateAPCost());
			if (m_CurrentPath.m_ReachableByCurrentCharacter.Count < m_CurrentPath.m_VectorList.Count)
			{
				foreach (var Cell in m_CurrentPath.m_ReachableByCurrentCharacter)
				{
					m_VectorPathRemaining.Add(Cell.GetCellCenter());
				}
			}
			else
			{
				m_VectorPathRemaining = new List<Vector3>(m_CurrentPath.m_VectorList) ;
			}
				
			m_CellPathRemaining = new List<MovementCell>(m_CurrentPath.m_ReachableByCurrentCharacter);
			m_Seeker.StartPath(transform.position,m_VectorPathRemaining[0]);
			m_AIPath.targetReachedDelegate += GoToNextNode;
		}

		public int CalculateAPCost()
		{
			int movementCost = m_CurrentPath.m_CellList.Last().m_FCost;
			int APCost =
				Mathf.CeilToInt(movementCost / m_ParentEntity.m_AttributeDictionary["Movement"].m_CurrentValue);
			Debug.LogError(APCost +" ap cost");
			return APCost;
		}
		
		public virtual void GoToNextNode()
		{
			if (m_VectorPathRemaining.Count > 0)
			{
				m_WalkCostThisTurn += m_CellPathRemaining[0].m_CostToGoToThisOne;
				if (m_WalkCostThisTurn > m_ParentEntity.m_AttributeDictionary["Movement"].m_CurrentValue)
				{
					m_ParentEntity.ChangeAPByValue(-1);
					m_WalkCostThisTurn -= m_ParentEntity.m_AttributeDictionary["Movement"].m_CurrentValue;
				}
				m_VectorPathRemaining.RemoveAt(0);
				m_CellPathRemaining.RemoveAt(0);
			}
				
			if (m_VectorPathRemaining.Count == 1)
			{
				m_AIPath.whenCloseToDestination = CloseToDestinationMode.Stop;
				m_Seeker.StartPath(transform.position, m_VectorPathRemaining[0]);
			}

			if (m_VectorPathRemaining.Count > 0)
			{
				m_Seeker.StartPath(transform.position, m_VectorPathRemaining[0]);
			}
			else
			{
				EndMovement();
			}
		}

		public virtual void EndMovement()
		{
			if(m_WalkCostThisTurn >= 10)
				m_ParentEntity.ChangeAPByValue(-1);
			m_AIPath.whenCloseToDestination = CloseToDestinationMode.Stop;
			m_VectorPathRemaining.Clear();
			m_CellPathRemaining.Clear();
			m_CurrentPath = null;
			m_CurrentlyMoving = false;
			m_ParentEntity.SetOnTile();
			m_AIPath.targetReachedDelegate -= GoToNextNode;
		}
	}
}