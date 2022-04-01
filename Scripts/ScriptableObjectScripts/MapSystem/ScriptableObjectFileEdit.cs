using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectFileEdit : ScriptableObject
{
#if UNITY_EDITOR

    [FoldoutGroup("File Settings")]
    public string NewName;

    [FoldoutGroup("File Settings")]
    [Button("Rename")]
    private void Rename()
    {
        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), NewName);
    }

    [FoldoutGroup("File Settings")]
    [Button("Delete")]
    private void Delete()
    {
        if (EditorUtility.DisplayDialog("Delete Room?", "Are you sure you want to delete this room?", "Delete", "Return"))
        {
            string path = AssetDatabase.GetAssetPath(this);
            AssetDatabase.DeleteAsset(path);
        }
    }

#endif
}
