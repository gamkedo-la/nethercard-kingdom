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
	[System.Serializable]
	public class CardEvent : UnityEvent<CardNew> { }

	public CardSelectionMode SelectionMode { get; set; }
	public string Name { get { return displayName; } }
	public Card LowerLevelVersion { get { return lowerLevelVersion; } }
	public Card HigherLevelVersion { get { return higherLevelVersion; } }
	public CardAudioVisuals Vizuals { get { return vizuals; } }

	[Header("Objects")]
	[SerializeField] private GameObject toSummon = null;
	[SerializeField] private Card lowerLevelVersion = null;
	[SerializeField] private Card higherLevelVersion = null;
	[SerializeField] private CardAudioVisuals vizuals = null;

	[Header("Card Parameters")]
	[SerializeField] private CardType type = CardType.Unit;
	[SerializeField] private CardLevel level = CardLevel.Level1;
	[SerializeField] private int useCost = 2;
	[SerializeField] private string displayName = "Unnamed Card";
	[SerializeField] private string abilityText = "This is just a test description...";
	[SerializeField] private string flavorText = "What a lovely card!";

	[Header("Events")]
	public CardEvent onStartedDrag = null;
	public CardEvent onEndedDrag = null;
	public CardEvent onOverEnter = null;
	public CardEvent onOverExit = null;
	public CardEvent onClicked = null;
	public CardEvent onRelease = null;
	public CardEvent onDrop = null;

	private bool dragging = false;

	void Start( )
	{
		Assert.IsNotNull( toSummon, $"Please assign <b>{nameof( toSummon )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( vizuals, $"Please assign <b>{nameof( vizuals )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		if ( level == CardLevel.Level1 || level == CardLevel.Level2 )
			Assert.IsNotNull( higherLevelVersion, $"Please assign <b>{nameof( higherLevelVersion )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		if ( level == CardLevel.Level2 || level == CardLevel.Level3 )
			Assert.IsNotNull( lowerLevelVersion, $"Please assign <b>{nameof( lowerLevelVersion )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		PopulateCardInfo( );
	}

	void Update( )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		// We were 'click dragging' and pressed our mouse button
		if ( dragging && Input.GetMouseButtonDown( 0 ) )
			OnEndDrag( );
	}

	public void OnOverEnter( )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		//Debug.Log( $"On Enter: {name}" );
		onOverEnter?.Invoke( this );

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
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		//Debug.Log( $"On Exit: {name}" );
		onOverExit?.Invoke( this );

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

	public void OnCliked( )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		//Debug.Log( $"On Clicked: {name}" );
		onClicked?.Invoke( this );
		//OnBeginDrag( );

		/*if ( selected )
		{
			selected = false;
			OnEndDrag( );
		}
		else
		{
			selected = true;
			OnBeginDrag( );
		}*/

		/*if ( selectionMode == CardSelectionMode.InHand )
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
			if ( !selected /*&& !draggedCard*//* )
			{
				selected = true;
				Invoke( nameof( CanBeUnselected ), 0.01f );
				StartDraggingInDeckBuilding( );
			}
		}*/
	}

	public void OnRelease( )
	{
		//Debug.Log( $"On Release: {name}" );
		onRelease?.Invoke( this );
	}

	public void OnDrop( )
	{
		//Debug.Log( $"On Release: {name}" );
		onDrop?.Invoke( this );
	}

	public void OnBeginDrag( )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		// Only execute if the card is NOT being dragged
		if ( dragging )
			return;
		else
			dragging = true;

		//Debug.Log( $"On Begin Drag: {name}" );
		onStartedDrag?.Invoke( this );

		/*if ( selectionMode == CardSelectionMode.InHand )
			StartSummoning( );
		else if ( selectionMode == CardSelectionMode.InCollection || selectionMode == CardSelectionMode.InDeck )
			StartDraggingInDeckBuilding( );*/
	}

	public void OnEndDrag( )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		// Only execute if the card was being dragged
		if ( !dragging )
			return;
		else
			dragging = false;

		//Debug.Log( $"On End Drag: {name}" );
		onEndedDrag?.Invoke( this );

		/*if ( selectionMode == CardSelectionMode.InHand )
			EndSummoning( );
		else if ( selectionMode == CardSelectionMode.InCollection || selectionMode == CardSelectionMode.InDeck )
			EndDraggingInDeckBuilding( );*/
	}

	public void CardSelected( bool isSelected )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		//Debug.Log( $"On Selected: {name}" );

		/*if ( isSelected )
			defaultScale = Vector3.one * 1.07f;
		else
			defaultScale = Vector3.one;

		scaleToLerp = defaultScale;*/
	}

	public void UpdateCardStatsFromEditor( CardType cardType, CardLevel cardLevel, string name, int cost,
		string ability, string flavor, Sprite borderSprite, Sprite fillSprite, GameObject instanceToSummon )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		/*type = cardType;
		level = cardLevel;
		displayName = name;
		useCost = cost;
		abilityText = ability;
		flavorText = flavor;
		//cardImageBorder.sprite = borderSprite;
		//cardImageFill.sprite = fillSprite;
		toSummon = instanceToSummon;*/
	}

	[ContextMenu( "Update Card Info" )]
	public void PopulateCardInfo( )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

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

	private void StartSummoning( )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		//Debug.Log( $"On Start Summoning: {name}" );

		/*if ( !SummoningManager.Instance.EnoughMana( useCost ) )
			return;

		//draggedCard = this;
		OnOverEnter( );

		SummoningManager.Instance.Summoning( Camera.main.ScreenToWorldPoint( Input.mousePosition ), type, true );
		//playSound.Play( );*/
	}

	private void EndSummoning( )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		//Debug.Log( $"On End Summoning: {name}" );

		/*if ( draggedCard != this )
			return;*/

		/*OnOverExit( );

		bool canSummon = SummoningManager.Instance.Summoning( Vector2.zero, type, false );

		if ( canSummon )
		{
			GameObject instance = Instantiate( toSummon, (Vector2)Camera.main.ScreenToWorldPoint( Input.mousePosition ), Quaternion.identity );
			if ( type == CardType.DirectDefensiveSpell || type == CardType.DirectOffensiveSpell || type == CardType.AoeSpell )
				instance.GetComponent<Spell>( ).SetTarget( SummoningManager.Instance.LastTarget );

			SummoningManager.Instance.RemoveMana( useCost );
			Destroy( gameObject );
		}*/
		//else
		//backSound.Play( );
	}

	private void StartDraggingInDeckBuilding( )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		//Debug.Log( $"On Start Summoning: {name}" );

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
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		/*transform.position = previousPosition;
		canvasGroup.alpha = 1.0f;

		frontCanvas.overrideSorting = false;
		frontCanvas.sortingOrder = 100000;

		draggedCard = null;*/
		//DeckBuilder.Instance.MoveSlot( );
	}
}
