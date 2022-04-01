
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Linq;

#if UNITY_EDITOR

using UnityEditor;

#endif

//
// This is a scriptable object containing a list of all characters available
// in the Unity project. When a character is added from the RPG editor, the
// list then gets automatically updated via UpdateCharacterOverview.
//
// If you inspect the Character Overview in the inspector, you will also notice, that
// the list is not directly modifiable. Instead, we've customized it so it contains a
// refresh button, that scans the project and automatically populates the list.
//
// CharacterOverview inherits from GlobalConfig which is just a scriptable
// object singleton, used by Odin Inspector for configuration files etc,
// but it could easily just be a simple scriptable object instead.
//

[GlobalConfig("Assets/Game/ScriptableObjects/Gear")]
public partial class GearOverview: GlobalConfig<GearOverview>
{
	[ReadOnly]
	[ListDrawerSettings(Expanded = true)]
	public BaseGear[] AllGear;


}
#if UNITY_EDITOR
public partial class GearOverview 
{


	[Button(ButtonSizes.Medium), PropertyOrder(-1)]
	public void UpdateGearOverview()
	{
		// Finds and assigns all scriptable objects of type Character
		this.AllGear = AssetDatabase.FindAssets("t:BaseGear")
			.Select(guid => AssetDatabase.LoadAssetAtPath<BaseGear>(AssetDatabase.GUIDToAssetPath(guid)))
			.ToArray();
	}


}
#endif