using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelEditorMenu))]
public class LevelEditorMenu : Editor
{
    [MenuItem("Custom Tools/Assets/New Level")]
    public static void CreateAsset()
    {
        LevelData asset = ScriptableObject.CreateInstance(typeof(LevelData)) as LevelData;
        AssetDatabase.CreateAsset(asset, "Assets/Resources/Levels/RenameLevel.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

}

