//Thank you Brackeys :)
using UnityEngine;
using UnityEditor;

public class ExampleEditor : EditorWindow
{

    Color color;

    [MenuItem("Window/Card Creator (WIP)")]
    public static void ShowWindow()
    {
        GetWindow<ExampleEditor>("Card Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Create card", EditorStyles.boldLabel);

        

        if(GUILayout.Button("Finish Card"))
        {
            CreateCardVariant();
        }

    }  

    void CreateCardVariant()
    {
        string prefabPath = "Prefabs/Card Template";
        string localPath = "Assets/Resources/Prefabs/Player Cards/New Card.prefab";
        Object source = Resources.Load(prefabPath);
        GameObject objSource = (GameObject)PrefabUtility.InstantiatePrefab(source);
        GameObject obj = PrefabUtility.SaveAsPrefabAsset(objSource, localPath);
        obj.name = "New Card";
    }
}
