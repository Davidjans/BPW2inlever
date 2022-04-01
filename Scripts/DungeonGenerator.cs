 using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public enum TileType{Floor,Wall}
public class DungeonGenerator : SerializedMonoBehaviour
{
    public GameObject m_FloorPrefab;
    public GameObject m_WallPrefab;
    
    public int m_GridWith = 100;
    public int m_GridHeight = 100;
    public int m_MinRoomSize = 3;
    public int m_MaxRoomSize = 7;
    public int m_NumRooms = 10;
    public Dictionary<Vector3Int, TileType> m_Dungeon = new Dictionary<Vector3Int, TileType>();
    public List<Room> m_RoomList = new List<Room>();
    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        for (int i = 0; i < m_NumRooms; i++)
        {
            int minX = Random.Range(0, m_GridWith);
            int maxX = minX + Random.Range(m_MinRoomSize, m_MaxRoomSize + 1);
            int minz = Random.Range(0, m_GridHeight);
            int maxz = minz + Random.Range(m_MinRoomSize, m_MaxRoomSize + 1);
  
            Room room = new Room(minX, maxX, minz, maxz);
            if (CanRoomFitInDugeon(room))
            {
                AddRoomToDungeon(room);
            }
            else
            {
                i--;
            }
        }
        for (int i = 0; i < m_RoomList.Count; i++)
        {
            Room currentRoom = m_RoomList[i];
            Room nextRoom = m_RoomList[(i + Random.Range(1, m_RoomList.Count)) % m_RoomList.Count];
            ConnectRooms(currentRoom,nextRoom);
        }

        AllocateWalls();
        
        SpawnDungeon();
    }

    public void AllocateWalls()
    {
        var keys = m_Dungeon.Keys.ToList();
        foreach (var kvp in keys)
        {
            for (int x =-1; x <= 1; x++)
            {
                for (int z = -1; z <=1; z++)
                {
                    if(Mathf.Abs(x) == Mathf.Abs(z)){continue;}
                    Vector3Int newPos = kvp + new Vector3Int(x, 0, z);
                    if(m_Dungeon.ContainsKey(newPos)){continue;}
                        m_Dungeon.Add(newPos,TileType.Wall);

                }
            }
        }
    }
    public void ConnectRooms(Room currentRoom, Room nextRoom)
    {
        Vector3Int posOne = currentRoom.GetCenter();
        Vector3Int posTwo = nextRoom.GetCenter();
        int dirX = posTwo.x > posOne.x ? 1 : -1;
        int x = 0;
        for (x = posOne.x; x != posTwo.x; x+= dirX)
        {
            Vector3Int position = new Vector3Int(x,0,posOne.z);
            if(m_Dungeon.ContainsKey(position)){continue;}
                m_Dungeon.Add(position,TileType.Floor);
        }
        int dirZ = posTwo.z > posOne.z ? 1 : -1;
        for (int z = posOne.z; z != posTwo.z; z+= dirZ)
        {
            Vector3Int position = new Vector3Int(x,0,z);
            if(m_Dungeon.ContainsKey(position)){continue;}
                m_Dungeon.Add(position,TileType.Floor);
        }
    }
    
    public void SpawnDungeon()
    {
        foreach (KeyValuePair<Vector3Int,TileType> kv in m_Dungeon)
        {
            switch (kv.Value)
            {
                case TileType.Floor:
                    Instantiate(m_FloorPrefab, kv.Key, quaternion.identity, transform);
                    break;
                case TileType.Wall:
                    Instantiate(m_WallPrefab, kv.Key, quaternion.identity, transform);
                    break;
            }
        }
    }
    public void AddRoomToDungeon(Room room)
    {
        for (int x = room.m_MinX; x <= room.m_MaxX; x++)
        {
            for (int z = room.m_MinZ; z<= room.m_MaxZ; z++)
            {
                if(!m_Dungeon.ContainsKey(new Vector3Int(x,0,z)))
                  m_Dungeon.Add(new Vector3Int(x,0,z), TileType.Floor);  
            }
        }
        m_RoomList.Add(room);
    }
    public bool CanRoomFitInDugeon(Room room)
    {
        for (int x = room.m_MinX -2; x < room.m_MaxX + 2; x++)
        {
            for (int z = 0 -2 ; room.m_MinZ < room.m_MaxZ +2; z++)
            {
                if (!m_Dungeon.ContainsKey(new Vector3Int(x, 0, z)))
                    return true;
            }
        }

        return false;
    }

   
    [Serializable]
    public class Room
    {
        public int m_MinX, m_MaxX, m_MinZ, m_MaxZ;

        public Room(int minX, int maxX, int minZ, int maxZ)
        {
            m_MinX = minX;
            m_MaxX = maxX;
            m_MinZ = minZ;
            m_MaxZ = maxZ;
        }

        public Vector3Int GetCenter()
        {
            return new Vector3Int(Mathf.RoundToInt(Mathf.Lerp(m_MinX,m_MaxX,0.5f)),0, Mathf.RoundToInt(Mathf.Lerp(m_MinZ,m_MaxZ, 0.5f)));
        }
    }
    
    
}
