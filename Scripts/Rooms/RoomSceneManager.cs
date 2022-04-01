using System;
using System.Collections;
using System.Collections.Generic;
using EVN.MapSystem;
using EVN.RoomSystem;
using UnityEngine;

namespace EVN.RoomSystem
{
	public class RoomSceneManager : MonoBehaviour
	{
		public List<RoomManager> m_Rooms;

		public Dictionary<RoomType, RoomManager> m_RoomsByRoomType = new Dictionary<RoomType, RoomManager>();

		public void Awake()
		{
			foreach (var room in m_Rooms)
			{
				m_RoomsByRoomType.Add(room.m_RoomSettings.RoomType,room);
			}
		}

		public void EnableAndGenerateRoom(RoomType roomType)
		{
			m_RoomsByRoomType[roomType].gameObject.SetActive(true);
		}

		public void ReturnToMap()
		{
			MapManager.Instance.EnableMap();
		}
	}
}

