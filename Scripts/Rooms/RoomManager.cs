using System;
using System.Collections;
using System.Collections.Generic;
using EVN.MapSystem;
using UnityEngine;
using UnityEngine.Events;

namespace EVN.RoomSystem
{
	public class RoomManager : MonoBehaviour
	{
		public RoomGeneration m_RoomGenerator;
		public RoomSettings m_RoomSettings;
		public UnityEvent m_OnCompleteRoom;
		public AstarPath m_PathFinder;
		private void Start()
		{
			m_RoomGenerator = GetComponent<RoomGeneration>();
			m_PathFinder.Scan();
		}
	}
}
