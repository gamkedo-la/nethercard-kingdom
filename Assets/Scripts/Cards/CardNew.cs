/**
 * Description: Main card data and functionality.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class CardNew : MonoBehaviour
{
	public CardSelectionMode SelectionMode { get { return selectionMode; } set { selectionMode = value; } }
	public string Name { get { return displayName; } }
	public Card LowerLevelVersion { get { return lowerLevelVersion; } }
	public Card HigherLevelVersion { get { return higherLevelVersion; } }

	[Header("External Objects")]
	[SerializeField] private GameObject toSummon = null;
	[SerializeField] private Card lowerLevelVersion = null;
	[SerializeField] private Card higherLevelVersion = null;

	[Header("Card Parameters")]
	[SerializeField] private CardType type = CardType.Unit;
	[SerializeField] private CardLevel level = CardLevel.Level1;
	[SerializeField] private CardSelectionMode selectionMode = CardSelectionMode.InHand;
	[SerializeField] private int useCost = 2;
	[SerializeField] private string displayName = "Unnamed Card";
	[SerializeField] private string abilityText = "This is just a test description...";
	[SerializeField] private string flavorText = "What a lovely card!";

	[Header("Events")]
	public UnityEvent onStartedDrag = null;
	public UnityEvent onEndedDrag = null;

	private bool selected = false;
	private bool canBeUnselected = false;

	void Start( )
	{
		Assert.IsNotNull( toSummon, $"Please assign <b>{nameof( toSummon )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		if ( level == CardLevel.Level1 || level == CardLevel.Level2 )
			Assert.IsNotNull( higherLevelVersion, $"Please assign <b>{nameof( higherLevelVersion )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		if ( level == CardLevel.Level2 || level == CardLevel.Level3 )
			Assert.IsNotNull( lowerLevelVersion, $"Please assign <b>{nameof( lowerLevelVersion )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		PopulateCardInfo( );
	}

	void Update( )
	{
		/*if ( draggedCard == this && canBeUnselected && Input.GetMouseButtonDown( 0 ) )
		{
			selected = false;
			canBeUnselected = false;
			EndDraggingInDeckBuilding( );
		}*/
	}

	public void OnOverEnter( )
	{
		/*if ( selectionMode == CardSelectionMode.InHand )
		{
			scaleToLerp = Vector3.one * 1.3f;
			lerpBack = false;
			lerpBackTimer = 0.1f;
			frontCanvas.overrideSorting = true;
			frontCanvas.sortingOrder = 1100;

			if ( hoverCard == null )
			{
				overSound.Play( );
				hoverCard = this;
			}
		}
		else if ( selectionMode == CardSelectionMode.InCollection )
		{
			lerpBack = false;
			lerpBackTimer = 0.1f;
			scaleToLerp = defaultScale * 1.1f;

			if ( DeckBuilder.Instance.IsDeckCardSelected( ) )
			{
				DeckBuilder.Instance.CheckCollectionCardSelection( this );

				scaleToLerp = defaultScale * 1.5f;
			}
		}
		else if ( selectionMode == CardSelectionMode.InDeck )
		{
			lerpBack = false;
			lerpBackTimer = 0.1f;
			scaleToLerp = defaultScale * 1.1f;

			if ( DeckBuilder.Instance.IsCollectionCardSelected( ) )
			{
				DeckBuilder.Instance.CheckDeckCardSelection( this );

				scaleToLerp = defaultScale * 1.5f;
			}
		}*/
	}

	public void OnOverExit( )
	{
		/*if ( draggedCard == this )
			return;

		if ( selectionMode == CardSelectionMode.InHand )
		{
			scaleToLerp = Vector3.one;
			lerpBack = true;

			frontCanvas.overrideSorting = false;
			frontCanvas.sortingOrder = 0;

			if ( hoverCard == this )
				hoverCard = null;
		}
		else if ( selectionMode == CardSelectionMode.InCollection || selectionMode == CardSelectionMode.InDeck )
		{
			scaleToLerp = defaultScale;
			lerpBack = true;

			DeckBuilder.Instance.CompareAndRemoveSelection( this );
		}*/
	}

	public void CardSelected( bool isSelected )
	{
		/*if ( isSelected )
			defaultScale = Vector3.one * 1.07f;
		else
			defaultScale = Vector3.one;

		scaleToLerp = defaultScale;*/
	}

	public void OnBeginDrag( )
	{
		onStartedDrag?.Invoke( );

		if ( selectionMode == CardSelectionMode.InHand )
			StartSummoning( );
		else if ( selectionMode == CardSelectionMode.InCollection || selectionMode == CardSelectionMode.InDeck )
			StartDraggingInDeckBuilding( );
	}

	public void OnEndDrag( )
	{
		onEndedDrag?.Invoke( );

		if ( selectionMode == CardSelectionMode.InHand )
			EndSummoning( );
		else if ( selectionMode == CardSelectionMode.InCollection || selectionMode == CardSelectionMode.InDeck )
			EndDraggingInDeckBuilding( );
	}

	public void OnCliked( )
	{
		if ( selectionMode == CardSelectionMode.InHand )
		{
			if ( selected )
			{
				selected = false;
				EndSummoning( );
			}
			else
			{
				selected = true;
				StartSummoning( );
			}
		}
		else if ( selectionMode == CardSelectionMode.InCollection || selectionMode == CardSelectionMode.InDeck )
		{
			if ( !selected /*&& !draggedCard*/ )
			{
				selected = true;
				Invoke( nameof( CanBeUnselected ), 0.01f );
				StartDraggingInDeckBuilding( );
			}
		}
	}

	public void OnReleased( )
	{
		if ( selectionMode == CardSelectionMode.InHand )
		{ }
		else if ( selectionMode == CardSelectionMode.InCollection || selectionMode == CardSelectionMode.InDeck )
		{ }
	}

	public void UpdateCardStatsFromEditor( CardType cardType, CardLevel cardLevel, string name, int cost,
		string ability, string flavor, Sprite borderSprite, Sprite fillSprite, GameObject instanceToSummon )
	{
		type = cardType;
		level = cardLevel;
		displayName = name;
		useCost = cost;
		abilityText = ability;
		flavorText = flavor;
		//cardImageBorder.sprite = borderSprite;
		//cardImageFill.sprite = fillSprite;
		toSummon = instanceToSummon;
	}

	[ContextMenu( "Update Card Info" )]
	public void PopulateCardInfo( )
	{
		/*var specificCulture = System.Globalization.CultureInfo.GetCultureInfo( "en-US" );

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
			level3Marks.SetActive( true );*/
	}

	private void CanBeUnselected( ) => canBeUnselected = true;

	private void StartSummoning( )
	{
		if ( !SummoningManager.Instance.EnoughMana( useCost ) )
			return;

		//draggedCard = this;
		OnOverEnter( );

		SummoningManager.Instance.Summoning( Camera.main.ScreenToWorldPoint( Input.mousePosition ), type, true );
		//playSound.Play( );
	}

	private void EndSummoning( )
	{
		/*if ( draggedCard != this )
			return;*/

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
		//else
			//backSound.Play( );
	}

	private void StartDraggingInDeckBuilding( )
	{
		/*draggedCard = this;

		DeckBuilder.Instance.CheckCollectionCardSelection( this );
		DeckBuilder.Instance.CheckDeckCardSelection( this );

		previousPosition = transform.position;

		scaleToLerp = defaultScale * 1.25f;

		frontCanvas.overrideSorting = true;
		frontCanvas.sortingOrder = 999999;*/
	}

	private void EndDraggingInDeckBuilding( )
	{
		/*transform.position = previousPosition;
		canvasGroup.alpha = 1.0f;

		frontCanvas.overrideSorting = false;
		frontCanvas.sortingOrder = 100000;

		draggedCard = null;*/
		//DeckBuilder.Instance.MoveSlot( );
	}
}
