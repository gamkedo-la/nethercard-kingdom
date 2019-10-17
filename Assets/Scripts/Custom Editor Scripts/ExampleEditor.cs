using UnityEngine;
using UnityEditor;

public class ExampleEditor : EditorWindow
{
    string cardName = "New Card Name";
    CardType cardType;
    private Card cardData;
    private int cardCost = 0;
    private int attack = 0;
    private int hp = 0;
    private float speed = 0;
    private string abilityText = "Ability";
    private string flavorText = "Lore?";

    private Sprite cardArt = null;

    [MenuItem("Window/Card Creator (WIP)")]
    public static void ShowWindow()
    {
        GetWindow<ExampleEditor>("Card Creator");
    }

    void OnGUI()
    {
        
        GUILayout.Label("Create card", EditorStyles.boldLabel);

        cardName = EditorGUILayout.TextField("Card Name", cardName);
        cardType = (CardType)EditorGUILayout.EnumFlagsField("Card Type", cardType);        
        cardCost = EditorGUILayout.IntField("Card Cost", cardCost);
        attack = EditorGUILayout.IntField("Attak Power", attack);
        hp = EditorGUILayout.IntField("Hit Points", hp);
        speed = EditorGUILayout.FloatField("Speed", speed);
        abilityText = EditorGUILayout.TextField("Ability Text", abilityText);
        flavorText = EditorGUILayout.TextField("Flavor Text", flavorText);

        

        /* [SerializeField] private CardType type = CardType.Unit;
      [SerializeField] private int useCost = 2;
      [SerializeField] private int attack = 1;
      [SerializeField] private int hp = 10;
      [SerializeField] private float speed = 2.5f;
      [SerializeField] private string displayName = "Unnamed Card";
      [SerializeField] private string abilityText = "This is just a test description...";
      [SerializeField] private string flavorText = "What a lovely card!";*/

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
        cardData.UpdateCardStatsFromEditor(cardType, cardName, cardCost, abilityText, flavorText);
        DestroyImmediate(card);

    }
}
