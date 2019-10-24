﻿/**
 * Description: Main card functionality.
 * Authors: Kornel, Bilal
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
	public CardSelectionMode SelectionMode { get { return selectionMode; } set { selectionMode = value; } }
	public string Name { get { return displayName; } }
	public Card LowerLevelVersion { get { return lowerLevelVersion; } }
	public Card HigherLevelVersion { get { return higherLevelVersion; } }
	public CardLevel Level { get { return level; } }
    public CardType CardType { get { return type; } }
	public GameObject Prefab { get; set; }

	[Header("External Objects")]
	[SerializeField] private GameObject toSummon = null;
	[SerializeField] private Card lowerLevelVersion = null;
	[SerializeField] private Card higherLevelVersion = null;

	[Header("Card Elements")]
	[SerializeField] private Canvas canvas = null;
	[SerializeField] private CanvasGroup canvasGroup = null;
	[SerializeField] private CanvasGroup liveImage = null;
	[SerializeField] private GameObject statisticsPanel = null;
	[SerializeField] private TextMeshProUGUI manaCostLabel = null;
	[SerializeField] private TextMeshProUGUI nameLabel = null;
	[SerializeField] private TextMeshProUGUI attackLabel = null;
	[SerializeField] private TextMeshProUGUI hpLabel = null;
	[SerializeField] private TextMeshProUGUI speedLabel = null;
	[SerializeField] private TextMeshProUGUI abilityLabel = null;
	[SerializeField] private TextMeshProUGUI flavorLabel = null;
	[SerializeField] private Image cardImageFill = null;
	[SerializeField] private Image cardImageBorder = null;
	[SerializeField] private GameObject level2Marks = null;
	[SerializeField] private GameObject level3Marks = null;

	[Header("Card Parameters")]
	[SerializeField] private CardType type = CardType.Unit;
	[SerializeField] private CardLevel level = CardLevel.Level1;
	[SerializeField] private CardSelectionMode selectionMode = CardSelectionMode.InHand;
	[SerializeField] private int useCost = 2;
	[SerializeField] private string displayName = "Unnamed Card";
	[SerializeField] private string abilityText = "This is just a test description...";
	[SerializeField] private string flavorText = "What a lovely card!";

	static public Card hoverCard = null;
	static public Card draggedCard = null;

	[HideInInspector] public float lerpBackTimer = 0f;
	[HideInInspector] public bool lerpBack = false;

	private Vector3 scaleToLerp = Vector3.one;
	private Vector3 defaultScale = Vector3.one;

	private Vector3 previousPosition = Vector3.zero;

	void Start( )
	{
		Assert.IsNotNull( toSummon, $"Please assign <b>{nameof( toSummon )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		Assert.IsNotNull( canvas, $"Please assign <b>{nameof( canvas )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( canvasGroup, $"Please assign <b>{nameof( canvasGroup )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( liveImage, $"Please assign <b>{nameof( liveImage )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		Assert.IsNotNull( statisticsPanel, $"Please assign <b>{nameof( statisticsPanel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( manaCostLabel, $"Please assign <b>{nameof( manaCostLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( nameLabel, $"Please assign <b>{nameof( nameLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( attackLabel, $"Please assign <b>{nameof( attackLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( hpLabel, $"Please assign <b>{nameof( hpLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( speedLabel, $"Please assign <b>{nameof( speedLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( abilityLabel, $"Please assign <b>{nameof( abilityLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( flavorLabel, $"Please assign <b>{nameof( flavorLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( level2Marks, $"Please assign <b>{nameof( level2Marks )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( level3Marks, $"Please assign <b>{nameof( level3Marks )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		/*if ( level == CardLevel.Level1 || level == CardLevel.Level2 )
			Assert.IsNotNull( higherLevelVersion, $"Please assign <b>{nameof( higherLevelVersion )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		if ( level == CardLevel.Level2 || level == CardLevel.Level3 )
			Assert.IsNotNull( lowerLevelVersion, $"Please assign <b>{nameof( lowerLevelVersion )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		*/
		PopulateCardInfo( );
	}

	void Update( )
	{
		if ( draggedCard == this )
		{
			if ( selectionMode == CardSelectionMode.InHand )
			{
				transform.position = Vector2.Lerp( transform.position, Input.mousePosition, 0.25f );
				canvasGroup.alpha = Mathf.Lerp( canvasGroup.alpha, 0.0f, 0.15f );
				liveImage.alpha = Mathf.Lerp( liveImage.alpha, 0.5f, 0.15f );
				transform.localScale = Vector3.one;
			}
			else if ( selectionMode == CardSelectionMode.InCollection || selectionMode == CardSelectionMode.InDeck )
			{
				transform.position = Vector2.Lerp( transform.position, Input.mousePosition + new Vector3(0.0f, -140.0f, 0.0f), 0.25f );
				canvasGroup.alpha = Mathf.Lerp( canvasGroup.alpha, 0.4f, 0.15f );
				if ( lerpBackTimer <= 0f || !lerpBack )
					transform.localScale = Vector3.Lerp( transform.localScale, scaleToLerp, 0.25f );
			}
		}
		else
		{
			if ( selectionMode == CardSelectionMode.InHand )
			{
				if ( SummoningManager.Instance.EnoughMana( useCost ) )
					canvasGroup.alpha = 1f;
				else
					canvasGroup.alpha = 0.9f;

				liveImage.alpha = Mathf.Lerp( liveImage.alpha, 0f, 0.25f );
			}

			if ( lerpBackTimer <= 0f || !lerpBack )
				transform.localScale = Vector3.Lerp( transform.localScale, scaleToLerp, 0.25f );
		}

		lerpBackTimer -= Time.deltaTime;
	}

	void OnValidate( )
	{
		//PopulateCardInfo( );
	}

	public void OnOverEnter( )
	{
		if ( selectionMode == CardSelectionMode.InHand )
		{
			scaleToLerp = Vector3.one * 1.3f;
			lerpBack = false;
			lerpBackTimer = 0.1f;
			canvas.overrideSorting = true;
			canvas.sortingOrder = 1100;

			if ( hoverCard == null )
				hoverCard = this;
		}
		else if ( selectionMode == CardSelectionMode.InCollection )
		{
			lerpBack = false;
			lerpBackTimer = 0.1f;
			scaleToLerp = defaultScale * 1.1f;

			if(DeckBuilder.Instance.IsDeckCardSelected())
			{
				DeckBuilder.Instance.CheckCollectionCardSelection(this);

				scaleToLerp = defaultScale * 1.5f;
			}
		}
		else if ( selectionMode == CardSelectionMode.InDeck )
		{
			lerpBack = false;
			lerpBackTimer = 0.1f;
			scaleToLerp = defaultScale * 1.1f;

			if(DeckBuilder.Instance.IsCollectionCardSelected())
			{
				DeckBuilder.Instance.CheckDeckCardSelection(this);

				scaleToLerp = defaultScale * 1.5f;
			}
		}
	}

	public void OnOverExit( )
	{
		if ( draggedCard == this )
			return;
		
		if ( selectionMode == CardSelectionMode.InHand )
		{
			scaleToLerp = Vector3.one;
			lerpBack = true;

			canvas.overrideSorting = false;
			canvas.sortingOrder = 0;

			if ( hoverCard == this )
				hoverCard = null;
		}
		else if ( selectionMode == CardSelectionMode.InCollection || selectionMode == CardSelectionMode.InDeck )
		{
			scaleToLerp = defaultScale;
			lerpBack = true;

			DeckBuilder.Instance.CompareAndRemoveSelection(this);
		}
	}

	public void CardSelected( bool isSelected )
	{
		if ( isSelected )
			defaultScale = Vector3.one * 1.07f;
		else
			defaultScale = Vector3.one;

		scaleToLerp = defaultScale;
	}

	public void OnCliked( )
	{
		if ( selectionMode == CardSelectionMode.InHand )
		{
			if ( !SummoningManager.Instance.EnoughMana( useCost ) )
				return;

			draggedCard = this;
			OnOverEnter( );

			SummoningManager.Instance.Summoning( Camera.main.ScreenToWorldPoint( Input.mousePosition ), type, true );
		}
		else if ( selectionMode == CardSelectionMode.InCollection || selectionMode == CardSelectionMode.InDeck )
		{
			//DeckBuilder.Instance.CardClicked( this, selectionMode );

			draggedCard = this;

			DeckBuilder.Instance.CheckCollectionCardSelection(this);
			DeckBuilder.Instance.CheckDeckCardSelection(this);

			previousPosition = transform.position;

			scaleToLerp = defaultScale * 1.25f;

			canvas.overrideSorting = true;
			canvas.sortingOrder = 999999;
		}
	}

	public void OnReleased( )
	{
		if ( selectionMode == CardSelectionMode.InHand )
		{
			if ( draggedCard != this )
				return;

			canvasGroup.alpha = 1f;
			draggedCard = null;
			OnOverExit( );

			bool canSummon = SummoningManager.Instance.Summoning( Vector2.zero, type, false );

			if ( canSummon )
			{
				GameObject instance = Instantiate( toSummon, (Vector2)Camera.main.ScreenToWorldPoint( Input.mousePosition ), Quaternion.identity );
				if ( type == CardType.DirectDefensiveSpell || type == CardType.DirectOffensiveSpell || type == CardType.AoeSpell )
					instance.GetComponent<Spell>( ).SetTarget( SummoningManager.Instance.LastTarget );

				SummoningManager.Instance.RemoveMana( useCost );
				Destroy( gameObject );
			}
		}
		else if ( selectionMode == CardSelectionMode.InCollection || selectionMode == CardSelectionMode.InDeck )
		{
			transform.position = previousPosition;
			canvasGroup.alpha = 1.0f;

			canvas.overrideSorting = false;
			canvas.sortingOrder = 100000;

			draggedCard = null;
			DeckBuilder.Instance.SwapCards();
		}
	}

	public void UpdateCardStatsFromEditor(CardType cardType, CardLevel cardLevel, string name, int cost, 
        string ability, string flavor, Sprite borderSprite, Sprite fillSprite, GameObject instanceToSummon )
	{
		type = cardType;
        level = cardLevel;
		displayName = name;
		useCost = cost;
		//attack = attackPower;
		//hp = hitPoints;
		//speed = cardSpeed;
		abilityText = ability;
		flavorText = flavor;
		cardImageBorder.sprite = borderSprite;
		cardImageFill.sprite = fillSprite;
		toSummon = instanceToSummon;
	}

	[ContextMenu( "Update Card Info" )]
	private void PopulateCardInfo( )
	{
		var specificCulture = System.Globalization.CultureInfo.GetCultureInfo( "en-US" );

		if ( type == CardType.Unit )
		{
			statisticsPanel.SetActive( true );

			Unit u = toSummon.GetComponent<Unit>( );
			attackLabel.text = u.DPS.ToString( "0.0", specificCulture );
			hpLabel.text = u.HP.MaxHP.ToString( );
			speedLabel.text = u.MoveSpeed.ToString( "0.0", specificCulture );
		}
		else
		{
			statisticsPanel.SetActive( false );
		}

		manaCostLabel.text = useCost.ToString( );
		nameLabel.text = displayName;
		abilityLabel.text = abilityText;
		flavorLabel.text = flavorText;

		level2Marks.SetActive( false );
		level3Marks.SetActive( false );

		if ( level == CardLevel.Level2 )
			level2Marks.SetActive( true );
		else if ( level == CardLevel.Level3 )
			level3Marks.SetActive( true );
	}
}
