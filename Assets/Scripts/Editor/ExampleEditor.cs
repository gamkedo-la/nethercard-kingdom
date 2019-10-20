using UnityEngine;
using UnityEditor;

public class ExampleEditor : EditorWindow
{
    string cardName = "New Card Name";
    CardType cardType;
    private Card cardData;
    private int cardCost = 0;
    //private int attack = 0;
    //private int hp = 0;
    //private float speed = 0;
    private string abilityText = "Ability";
    private string flavorText = "Lore?";

    private Sprite cardArtFill = null;
    private Sprite cardArtBorder = null;

    private GameObject toSummon;

    [MenuItem("Card Creation/Card Creator")]
    public static void ShowWindow()
    {
        GetWindow<ExampleEditor>("Card Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Create card", EditorStyles.boldLabel);

        cardName = EditorGUILayout.TextField("Card Name", cardName);
        cardType = (CardType)EditorGUILayout.EnumFlagsField("Card Type", cardType);
        toSummon = (GameObject)EditorGUILayout.ObjectField("Instance to Summon", toSummon, typeof(GameObject), false, GUILayout.ExpandWidth(true));
        cardArtFill = (Sprite)EditorGUILayout.ObjectField("Card Art Fill",cardArtFill, typeof(Sprite), true);
        cardArtBorder = (Sprite)EditorGUILayout.ObjectField("Card Art Border", cardArtBorder, typeof(Sprite), true);
        cardCost = EditorGUILayout.IntField("Card Cost", cardCost);
        abilityText = EditorGUILayout.TextField("Ability Text", abilityText);
        flavorText = EditorGUILayout.TextField("Flavor Text", flavorText);

        if (GUILayout.Button("Finish Card"))
        {
            CreateCardVariant();
        }
    }

    void CreateCardVariant()
    {
        //TODO: Add some error checking.  Currently, if a new card has the same name as an existing card the existing card's data is overwritten
        string prefabPath = "Assets/Prefabs/Card Template.prefab";
        string localPath = "Assets/Prefabs/Player Cards/" + cardName + ".prefab";
        Object cardPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
        GameObject card = PrefabUtility.InstantiatePrefab(cardPrefab) as GameObject;
        GameObject newCard = PrefabUtility.SaveAsPrefabAsset(card, localPath);

        cardData = newCard.GetComponent<Card>();
        cardData.UpdateCardStatsFromEditor(cardType, cardName, cardCost, abilityText, flavorText, cardArtFill, cardArtBorder, toSummon);
        DestroyImmediate(card);
    }
}
