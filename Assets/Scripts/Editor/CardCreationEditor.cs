using UnityEngine;
using UnityEditor;
using System.IO;


public class CardCreationEditor : EditorWindow
{
    private int toolBarInt = 0;
    private string[] editorTabs = { "Card Creation", "Unit Creation", "Spell Creation" };
    private bool overwriteExistingCard = false;

    //Card Variables
    string cardFileName = "New Card File Name";
    string displayName = "Card Display Name";
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
    private Editor summonedInstancePreview;
    private Card higherLevelCard = null;
    private Card lowerLevelCard = null;

    private GameObject cardObjectToOverwrite = null;
    private Card cardToOverwrite = null;

    private Rect cardDisplay = new Rect(20,20,120,200);

    //Units Instance Variables
    private string unitFileName = "Unit File Name";
    private string unitName = "New Unit Name";
    private Sprite unitArtFill = null;
    private Sprite unitArtBorder = null;
    private Unit unitData;
    private UnitVisuals unitVisualData;

    //Spell Instance Variables
    private string spellFileName = "Spell File Name";
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

            overwriteExistingCard = EditorGUILayout.Toggle("Overwrite Existing Card?", overwriteExistingCard);

            if (overwriteExistingCard)
            {
                cardObjectToOverwrite = (GameObject)EditorGUILayout.ObjectField("Choose Card to Overwrite", cardObjectToOverwrite, typeof(GameObject), false, GUILayout.ExpandWidth(true));
                if(cardObjectToOverwrite != null)
                {
                    cardToOverwrite = cardObjectToOverwrite.GetComponent<Card>();
                    cardFileName = cardToOverwrite.gameObject.name;
                    displayName = cardToOverwrite.Name;
                    cardType = cardToOverwrite.CardType;
                    cardLevel = cardToOverwrite.Level;
                    higherLevelCard = cardToOverwrite.HigherLevelVersion;
                    lowerLevelCard = cardToOverwrite.LowerLevelVersion;
                    toSummon = cardToOverwrite.ToSummon;
                    cardArtFill = cardToOverwrite.CardFill;
                    cardArtBorder = cardToOverwrite.CardBorder;
                    cardCost = cardToOverwrite.Cost;
                    abilityText = cardToOverwrite.Ability;
                    flavorText = cardToOverwrite.Flavor;
                }
            }

            cardFileName = EditorGUILayout.TextField("Card File Name", cardFileName);
            CheckStringLength(cardFileName, 100);
            displayName = EditorGUILayout.TextField("Card Display Name", displayName);
            CheckStringLength(displayName, 20);
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
            CheckInstanceType();
            cardArtFill = (Sprite)EditorGUILayout.ObjectField("Card Art Fill", cardArtFill, typeof(Sprite), true);
            cardArtBorder = (Sprite)EditorGUILayout.ObjectField("Card Art Border", cardArtBorder, typeof(Sprite), true);
            cardCost = EditorGUILayout.IntField("Card Cost", cardCost);
            CheckStringLength(cardCost.ToString(), 2);
            abilityText = EditorGUILayout.TextField("Ability Text", abilityText);
            CheckStringLength(abilityText, 30);
            flavorText = EditorGUILayout.TextField("Flavor Text", flavorText);
            CheckStringLength(flavorText, 30);

            /*
             TODO: Can a Card Preview Window be created?
            summonedInstancePreview = Editor.CreateEditor(toSummon);
            summonedInstancePreview.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(50, 50), EditorStyles.whiteLabel);
            */
            if (GUILayout.Button("Finish Card"))
            {
                CreateCardVariant();
            }
            GUILayout.TextArea("File Path is: Assets/Prefabs/Player Cards/" + cardFileName + ".prefab");
        }

        if (toolBarInt == 1)
        {
            GUILayout.Label("Create Unit", EditorStyles.boldLabel);

            unitFileName = EditorGUILayout.TextField("Unit File Name", unitFileName);
            unitName = EditorGUILayout.TextField("Unit Name", unitName);
            unitArtFill = (Sprite)EditorGUILayout.ObjectField("Unit Art Fill", unitArtFill, typeof(Sprite), true);
            unitArtBorder = (Sprite)EditorGUILayout.ObjectField("Unit Art Border", unitArtBorder, typeof(Sprite), true);


            if (GUILayout.Button("Finish Unit"))
            {
                CreateUnitVariant();
            }
            GUILayout.TextArea("File Path is: Assets/Prefabs/Player Cards/" + unitFileName + ".prefab");
        }

        if (toolBarInt == 2)
        {
            GUILayout.Label("Create Spell", EditorStyles.boldLabel);

            spellFileName = EditorGUILayout.TextField("Spell File Name", spellFileName);
            spellName = EditorGUILayout.TextField("Spell Name", spellName);
            spellType = (CardType)EditorGUILayout.EnumPopup("Spell Type", spellType);
            spellArtFill = (Sprite)EditorGUILayout.ObjectField("Spell Art Fill", spellArtFill, typeof(Sprite), true);
            spellArtBorder = (Sprite)EditorGUILayout.ObjectField("Spell Art Border", spellArtBorder, typeof(Sprite), true);

            if (GUILayout.Button("Finish Spell"))
            {
                CreateSpellInstance();
            }
            GUILayout.TextArea("File Path is: Assets/Prefabs/Player Cards/" + spellName + ".prefab");
        }
    }

    private void CheckStringLength(string textField, int characterMax)
    {
        if (textField.Length > characterMax && textField != null)
        {
            //fileName = "";
            GUILayout.TextField("Please enter " + characterMax.ToString() + " or less characters.");
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
        unitVisualData = newUnit.GetComponent<UnitVisuals>();
        unitVisualData.UpdateVisuals(unitArtBorder, unitArtFill);

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
        string prefabPath = "Assets/Prefabs/Card Template.prefab";
        string localPath = "Assets/Prefabs/Player Cards/" + cardFileName + ".prefab";
        if (CheckIfExists(localPath) && overwriteExistingCard == false)
        {
            return;
        }
        Object cardPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
        GameObject card = PrefabUtility.InstantiatePrefab(cardPrefab) as GameObject;
        GameObject newCard = PrefabUtility.SaveAsPrefabAsset(card, localPath);

        cardData = newCard.GetComponent<Card>();
        cardData.UpdateCardStatsFromEditor(cardType, cardLevel, displayName, cardCost, abilityText, flavorText, cardArtFill, cardArtBorder, toSummon);
        cardData.PopulateCardInfo();        
        Texture2D texture = AssetPreview.GetAssetPreview(newCard);
        GUILayout.Box(texture, GUILayout.Height(100), GUILayout.Width(100));
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

    private void CheckInstanceType()
    {
        if (toSummon != null)
        {
            if (cardType == CardType.Unit)
            {
                if (toSummon.GetComponent<Unit>() == null)
                {
                    GUILayout.TextField("Card Type does not match Instance Type.");
                }
            }
            if (cardType == CardType.DirectDefensiveSpell)
            {
                if (toSummon.GetComponent<DirectHealingSpell>() == null)
                {
                    GUILayout.TextField("Card Type does not match Instance Type.");
                }
            }
            if (cardType == CardType.DirectOffensiveSpell)
            {
                if (toSummon.GetComponent<DirectDamageSpell>() == null)
                {
                    GUILayout.TextField("Card Type does not match Instance Type.");
                }
            }
            if (cardType == CardType.AoeSpell)
            {
                if (toSummon.GetComponent<AoeSpell>() == null)
                {
                    GUILayout.TextField("Card Type does not match Instance Type.");
                }
            }
        }

    }
}
