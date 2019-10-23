using UnityEngine;
using UnityEditor;
using System.IO;


public class CardCreationEditor : EditorWindow
{
    private int toolBarInt = 0;
    private string[] editorTabs = { "Card Creation", "Unit Creation", "Spell Creation" };

    //Card Variables
    string cardName = "New Card Name";
    CardType cardType;
    CardLevel cardLevel = CardLevel.Level1;
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
    private Card higherLevelCard = null;
    private Card lowerLevelCard = null;

    //Units Instance Variables
    private string unitName = "New Unit Name";
    private Sprite unitArtFill = null;
    private Sprite unitArtBorder = null;
    private Unit unitData;

    //Spell Instance Variables
    private string spellName = "New Spell Name";
    private CardType spellType;

    private Sprite spellArtFill = null;
    private Sprite spellArtBorder = null;

    private SpellInstanceImages spellInstanceImages;


    [MenuItem("Card Creation/Card Creator")]
    public static void ShowWindow()
    {
        GetWindow<CardCreationEditor>("Card Creator");
    }

    void OnGUI()
    {
        toolBarInt = GUILayout.Toolbar(toolBarInt, editorTabs);

        if (toolBarInt == 0)
        {
            GUILayout.Label("Create card", EditorStyles.boldLabel);

            cardName = EditorGUILayout.TextField("Card Name", cardName);
            cardType = (CardType)EditorGUILayout.EnumPopup("Card Type", cardType);
            cardLevel = (CardLevel)EditorGUILayout.EnumPopup("Card Level", cardLevel);

            if (cardLevel == CardLevel.Level1 || cardLevel == CardLevel.Level2)
            {
                higherLevelCard = (Card)EditorGUILayout.ObjectField("Higher Level Card", higherLevelCard, typeof(Card), false, GUILayout.ExpandWidth(true));
            }

            if (cardLevel == CardLevel.Level3 || cardLevel == CardLevel.Level2)
            {
                lowerLevelCard = (Card)EditorGUILayout.ObjectField("Lower Level Card", lowerLevelCard, typeof(Card), false, GUILayout.ExpandWidth(true));
            }

            toSummon = (GameObject)EditorGUILayout.ObjectField("Instance to Summon", toSummon, typeof(GameObject), false, GUILayout.ExpandWidth(true));
            cardArtFill = (Sprite)EditorGUILayout.ObjectField("Card Art Fill", cardArtFill, typeof(Sprite), true);
            cardArtBorder = (Sprite)EditorGUILayout.ObjectField("Card Art Border", cardArtBorder, typeof(Sprite), true);
            cardCost = EditorGUILayout.IntField("Card Cost", cardCost);
            abilityText = EditorGUILayout.TextField("Ability Text", abilityText);
            flavorText = EditorGUILayout.TextField("Flavor Text", flavorText);

            if (GUILayout.Button("Finish Card"))
            {
                CreateCardVariant();
            }
        }

        if (toolBarInt == 1)
        {
            GUILayout.Label("Create Unit", EditorStyles.boldLabel);

            unitName = EditorGUILayout.TextField("Unit Name", unitName);
            unitArtFill = (Sprite)EditorGUILayout.ObjectField("Unit Art Fill", unitArtFill, typeof(Sprite), true);
            unitArtBorder = (Sprite)EditorGUILayout.ObjectField("Unit Art Border", unitArtBorder, typeof(Sprite), true);


            if (GUILayout.Button("Finish Unit"))
            {
                CreateUnitVariant();
            }
        }

        if (toolBarInt == 2)
        {
            GUILayout.Label("Create Spell", EditorStyles.boldLabel);

            spellName = EditorGUILayout.TextField("Spell Name", spellName);
            spellType = (CardType)EditorGUILayout.EnumPopup("Spell Type", spellType);
            spellArtFill = (Sprite)EditorGUILayout.ObjectField("Spell Art Fill", spellArtFill, typeof(Sprite), true);
            spellArtBorder = (Sprite)EditorGUILayout.ObjectField("Spell Art Border", spellArtBorder, typeof(Sprite), true);

            if (GUILayout.Button("Finish Spell"))
            {
                CreateSpellInstance();
            }
        }
    }

    private void CreateUnitVariant()
    {
        string prefabPath = "Assets/Prefabs/Unit Instance Template.prefab";
        string localPath = "Assets/Prefabs/Player Cards/" + unitName + ".prefab";
        if (CheckIfExists(localPath))
        {
            return;
        }
        Object unitPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
        GameObject unit = PrefabUtility.InstantiatePrefab(unitPrefab) as GameObject;
        GameObject newUnit = PrefabUtility.SaveAsPrefabAsset(unit, localPath);

        unitData = newUnit.GetComponent<Unit>();
        unitData.UpdateUnitStatsFromEditor(unitArtBorder, unitArtFill);

        DestroyImmediate(unit);
    }

    private void CreateSpellInstance()
    {
        string prefabPath = "Assets/Prefabs/Spell Instance Template.prefab";
        string localPath = "Assets/Prefabs/Player Cards/" + spellName + ".prefab";
        if (CheckIfExists(localPath))
        {
            return;
        }
        Object spellPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
        GameObject spell = PrefabUtility.InstantiatePrefab(spellPrefab) as GameObject;
        GameObject newSpell = PrefabUtility.SaveAsPrefabAsset(spell, localPath);
        spellInstanceImages = newSpell.GetComponent<SpellInstanceImages>();
        spellInstanceImages.UpdateCardDataFromEditor(spellArtBorder, spellArtFill);

        if (spellType == CardType.AoeSpell)
        {
            newSpell.AddComponent<OffensiveAoeSpell>();
        }
        else if (spellType == CardType.DirectDefensiveSpell)
        {
            newSpell.AddComponent<DirectHealingSpell>();
        }
        else if (spellType == CardType.DirectOffensiveSpell)
        {
            newSpell.AddComponent<DirectDamageSpell>();
        }


        DestroyImmediate(spell);
    }

    void CreateCardVariant()
    {
        //TODO: Add some error checking.  Currently, if a new card has the same name as an existing card the existing card's data is overwritten
        string prefabPath = "Assets/Prefabs/Card Template.prefab";
        string localPath = "Assets/Prefabs/Player Cards/" + cardName + ".prefab";
        if (CheckIfExists(localPath))
        {
            return;
        }
        Object cardPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
        GameObject card = PrefabUtility.InstantiatePrefab(cardPrefab) as GameObject;
        GameObject newCard = PrefabUtility.SaveAsPrefabAsset(card, localPath);

        cardData = newCard.GetComponent<Card>();
        cardData.UpdateCardStatsFromEditor(cardType, cardLevel, cardName, cardCost, abilityText, flavorText, cardArtFill, cardArtBorder, toSummon);
        DestroyImmediate(card);
    }

    bool CheckIfExists(string localPath)
    {
        if (File.Exists(localPath))
        {
            Debug.Log("Card exists already!");
            return true;
        }
        else { return false; }
    }
}
