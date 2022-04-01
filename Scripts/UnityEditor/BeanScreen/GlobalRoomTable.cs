#if UNITY_EDITOR

using EVN.MapSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GlobalRoomTable
{


    [HideLabel]
    [TableList(IsReadOnly = true, AlwaysExpanded = true), ShowInInspector]
    public GlobalRoomWrapper[] GlobalRoomSettings;


    public GlobalRoomTable()
    {     
        UpdateList();
    }


    private void UpdateList()
    {
        int enumCount = Enum.GetValues(typeof(RoomType)).Length;

        // Clear the array
        GlobalRoomSettings = new GlobalRoomWrapper[enumCount];

        // Get the already existing assets
        string[] assets = AssetDatabase.FindAssets("t:GlobalRoomSettings");
        IEnumerable<GlobalRoomSettings> roomSettings = assets.Select(guid => AssetDatabase.LoadAssetAtPath<GlobalRoomSettings>(AssetDatabase.GUIDToAssetPath(guid))).ToArray();
        List<GlobalRoomWrapper> existingAssets = roomSettings.Select(x => new GlobalRoomWrapper(x)).ToList();


        for (int i = 0; i < enumCount; i++)
        {
            // Add asset if needed
            if (existingAssets.Count <= i)
            {
                GlobalRoomSettings asset = ScriptableObject.CreateInstance<GlobalRoomSettings>();
                AssetDatabase.CreateAsset(asset, $"{BeanScreen.GlobalRoomsPath}/newRoom{i}.asset");               
            }
            else // Or set to existing asses
            {
                GlobalRoomSettings[i] = existingAssets[i];
            }
        }
    }

    public class GlobalRoomWrapper
    {

        [LabelWidth(1000)]
        [LabelText("$m_roomType")]
        public GlobalRoomSettings GlobalRoom;
        private string m_roomType { get { return $"({GlobalRoom.RoomType}) {GlobalRoom.name}"; } }


        public GlobalRoomWrapper(GlobalRoomSettings room)
        {
            GlobalRoom = room;
        }

    }
}

#endif
