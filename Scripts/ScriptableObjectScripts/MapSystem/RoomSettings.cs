using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace EVN.MapSystem
{
    [CreateAssetMenu(fileName = "New Room Settings", menuName = "StackedBeans/MapSystem/Room Settings", order = 1)]
    public class RoomSettings : ScriptableObjectFileEdit
    {
        [Required]
        [FoldoutGroup("General")]
        public RoomType RoomType;

        [Required]
        [FoldoutGroup("General")]
        public string BaseSceneName;

        [FoldoutGroup("Room")] 
        public List<AdditionalRoomInfo> m_AdditionalRoomInfo = new List<AdditionalRoomInfo>();
        
        [FoldoutGroup("Room")]
        [LabelText("InfoToggles")]
        public EnumToggles SideInfoToggles;
        
        private void OnValidate()
        {
            SideInfoToggles.ValidateEnumToggle(typeof(SideInfoType), "Room Side Info");

        }
    }
}