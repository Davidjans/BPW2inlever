using Sirenix.OdinInspector;
using UnityEngine;

namespace EVN.MapSystem
{
    [CreateAssetMenu(fileName = "New Global Room Settings", menuName = "StackedBeans/MapSystem/Global Room Settings", order = 1)]
    public class GlobalRoomSettings : ScriptableObjectFileEdit
    {

        [Required]
        [FoldoutGroup("General")]
        public RoomType RoomType;


        [FoldoutGroup("Settings")]
        [Title("Room Icon",TitleAlignment = TitleAlignments.Centered,HorizontalLine = false)]
        [HideLabel]
        [PreviewField(200, ObjectFieldAlignment.Center)]
        public Sprite RoomIcon;

        [Space(20f)]

        [FoldoutGroup("Settings")]
        [HideLabel]
        public EnumToggles ExitRoomOptions;

        private void OnValidate()
        {
            ExitRoomOptions.ValidateEnumToggle(typeof(RoomType), "Room Exits");
        }
    }
}
