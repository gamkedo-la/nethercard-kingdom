using UnityEngine;
using UnityEditor;

public class ExampleEditor : EditorWindow
{
    string cardName = "New Card Name";
    Color color;

    [MenuItem("Window/Card Creator (WIP)")]
    public static void ShowWindow()
    {
        GetWindow<ExampleEditor>("Card Creator");
    }

    void OnGUI()
    {

        GUILayout.Label("Create card", EditorStyles.boldLabel);

        cardName = EditorGUILayout.TextField("Card Name", cardName);

        if (GUILayout.Button("Finish Card"))
        {
            CreateCardVariant();
        }

    }

    void CreateCardVariant()
    {
        string prefabPath = "Assets/Prefabs/Card Template.prefab";
        string localPath = "Assets/Prefabs/Player Cards/" + cardName + ".prefab";
        Object cardPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
        GameObject card = PrefabUtility.InstantiatePrefab(cardPrefab) as GameObject;
        GameObject newCard = PrefabUtility.SaveAsPrefabAsset(card, localPath);
        DestroyImmediate(card);

    }
}
