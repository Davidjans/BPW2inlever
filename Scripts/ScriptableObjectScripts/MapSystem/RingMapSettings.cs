using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EVN.MapSystem
{ 
    [CreateAssetMenu(fileName = "New RingMapSettings", menuName = "StackedBeans/MapSystem/RingMapSettings", order = 1)]
    public class RingMapSettings : ScriptableObject
    {
        public int RowCount = 7;
        public int MaxRowHeight = 3;
        public int MinRowHeight = 1;
        public float MaxNodeConnectDistance = 1.0f;
        public float SmoothnesScale = 125.0f;
        public Vector2 Spacing = new Vector2(35, 10);
        public RoomType FirstRoomType = RoomType.FirstRoom;
        public RoomType lastRoomType = RoomType.Boss;

        public ForcedRoom[] ForcedRooms;


        [Serializable]
        public struct ForcedRoom
        {
            public RoomType RoomType;
            public int PossibleAfterRow;
        }
    }


}
