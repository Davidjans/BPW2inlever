#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;


public class BeanScreen : OdinMenuEditorWindow
{


    public const string RoomsPath = "Assets/GameContent/ScriptableObjects/MapSystem/Resources/Rooms";
    public const string GlobalRoomsPath = "Assets/GameContent/ScriptableObjects/MapSystem/Resources/GlobalRooms";


    [MenuItem("StackedBeans/BeanScreen")]
    private static void Open()
    {
        var window = GetWindow<BeanScreen>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        // Create the tree
        var tree = new OdinMenuTree(true);
        tree.DefaultMenuStyle.IconSize = 28.00f;
        tree.Config.DrawSearchToolbar = true;


        // Adds the character overview table.
        tree.Add("RoomSettings/Rooms", new RoomTable());

        // Adds the character overview table.
        tree.Add("RoomSettings/GlobalRooms", new GlobalRoomTable());
        

        GlobalSettings globalSettings = new GlobalSettings();
        globalSettings.Start();
        tree.Add("Tools/GlobalSettings", globalSettings);

        return tree;
    }




    private void AddDragHandles(OdinMenuItem menuItem)
    {
        menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
    }



    protected override void OnBeginDrawEditors()
    {
        var selected = this.MenuTree.Selection.FirstOrDefault();
        var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

        // Draws a toolbar with the name of the currently selected menu item.
        SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
        {
            if (selected != null)
            {
                GUILayout.Label(selected.Name);
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }
}
#endif
