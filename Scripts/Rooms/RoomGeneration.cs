using System.Collections.Generic;
using System.Linq;
using StackedBeans.Utils;
using UnityEngine;
using EVN.Movement;

namespace EVN.RoomSystem
{
	public class RoomGeneration : MonoBehaviour
	{
		protected RoomManager m_RoomManager;
		public List<SpawnNode> m_SpawnNodesInRoom = new List<SpawnNode>();
		public Transform m_FloorParent;
		public List<Collider> m_AllFloors;
		public Vector3 m_LeftBottomRoomPoint;
		public Vector3 m_RightTopROomPoint;
		public PathfindingManager m_GridManager;
		public GameObject m_PlayerSpawnNode;

		protected  virtual  void Start()
		{
			m_RoomManager = GetComponent<RoomManager>();
			m_AllFloors = m_FloorParent.GetComponentsInChildren<Collider>().ToList();
			m_LeftBottomRoomPoint =
				m_AllFloors[0].GetBoundsWorldPosition(new Vector3(-1,0,-1));
			m_RightTopROomPoint = m_AllFloors[m_AllFloors.Count - 1]
				.GetBoundsWorldPosition(new Vector3(1,0,1));


			m_GridManager.transform.position = m_LeftBottomRoomPoint;

			int xDistance = Mathf.FloorToInt(m_RightTopROomPoint.x - m_LeftBottomRoomPoint.x);
			int zDistance = Mathf.FloorToInt(m_RightTopROomPoint.z - m_LeftBottomRoomPoint.z);
			m_GridManager.m_GridSize = new Vector2(xDistance, zDistance);
			m_GridManager.RecreateGrid();

			DecidePlayerSpawnPos();
		}

		protected void DecidePlayerSpawnPos()
		{
			Vector3 playerSpawnPoint = Vector3.zero;
			playerSpawnPoint.z = m_LeftBottomRoomPoint.z + (m_GridManager.m_CellSize /2);
			float xDiff = m_RightTopROomPoint.x - m_LeftBottomRoomPoint.x;
			playerSpawnPoint.x = Random.Range(xDiff * 0.3f, xDiff * 0.7f) + (m_GridManager.m_CellSize /2);
			playerSpawnPoint.y = m_LeftBottomRoomPoint.y;
			GameObject player = Instantiate(m_PlayerSpawnNode);
			playerSpawnPoint.y += player.GetComponent<Collider>().bounds.extents.y;
			player.transform.position =  m_GridManager.m_Grid.GetCellOnPosition<MovementCell>(playerSpawnPoint).GetCellCenter();
		}

		
	}
}