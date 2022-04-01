#if UNITY_EDITOR

using EVN.MapSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RoomTable
{
    [HorizontalGroup("Group 1",150)]
    [Button("Create New Room",Style = ButtonStyle.FoldoutButton)]
    private void CreateNewRoom()
    {
        if (RoomName == null)      
            return;    

        RoomSettings asset = ScriptableObject.CreateInstance<RoomSettings>();
        asset.RoomType = RoomType;
        AssetDatabase.CreateAsset(asset, $"{BeanScreen.RoomsPath}/{RoomName}.asset");
        AssetDatabase.SaveAssets();
        OrganiseList();
    }

    [HideLabel]
    [HorizontalGroup("Group 1")]
    public RoomType RoomType;

    [HideLabel]
    [HorizontalGroup("Group 1")]
    public string RoomName;

    [HideLabel]
    [TableList(IsReadOnly = true, AlwaysExpanded = true), ShowInInspector]
    public List<RoomWrapper> Rooms = new List<RoomWrapper>();



    public void OrganiseList()
    {
        List<RoomWrapper> organisedRooms = new List<RoomWrapper>();

        foreach (RoomType roomType in Enum.GetValues(typeof(RoomType)))
        {
            for (int i = 0; i < Rooms.Count; i++)
            {
                RoomSettings room = Rooms[i].Room;

                if (room.RoomType == roomType)
                {
                    organisedRooms.Add(Rooms[i]);
                }
            }
        }

        Rooms = organisedRooms;
    }


    public RoomTable()
    {
        string[] assets = AssetDatabase.FindAssets("t:RoomSettings");
        IEnumerable<RoomSettings> roomSettings = assets.Select(guid => AssetDatabase.LoadAssetAtPath<RoomSettings>(AssetDatabase.GUIDToAssetPath(guid))).ToArray();
        Rooms = roomSettings.Select(x => new RoomWrapper(x)).ToList();
        OrganiseList();
    }

    public class RoomWrapper
    {

        [LabelWidth(1000)]
        [LabelText("$m_roomType")]
        public RoomSettings Room;
        private string m_roomType { get { return $"({Room.RoomType}) {Room.name}"; } }


        [Button("Delete")]
        private void DeleteRoom()
        {         
            if (EditorUtility.DisplayDialog("Delete Room?", "Are you sure you want to delete this room?", "Delete","Return"))
            {        
                string path = AssetDatabase.GetAssetPath(Room);
                AssetDatabase.DeleteAsset(path);
            }
        }


        public RoomWrapper(RoomSettings room)
        {
            Room = room;
        }
    }
}

#endif
