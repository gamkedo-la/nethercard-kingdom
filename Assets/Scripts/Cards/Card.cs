/**
 * Description: Main card data and functionality.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class Card : MonoBehaviour
{
	[System.Serializable]
	public class CardEvent : UnityEvent<Card> { }

	public CardSelectionMode SelectionMode { get; set; }
	public string Name { get { return displayName; } }
	public Card LowerLevelVersion { get { return lowerLevelVersion; } }
	public Card HigherLevelVersion { get { return higherLevelVersion; } }
	public CardAudioVisuals Vizuals { get { return vizuals; } }
	public int UseCost { get { return useCost; } }
	public CardType Type { get { return type; } }
	public GameObject ToSummon { get { return toSummon; } }
	public string Ability { get { return abilityText; } }
	public string Flavor { get { return flavorText; } }
	public  CardLevel Level { get { return level; } }
	public bool CanBePlayed { get; private set; } = true;

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
		CheckIfWeEndedClickDrag( );
	}

	void FixedUpdate( )
	{
		CheckIfCardCanBePlayed( );
	}

	public void OnOverEnter( ) => onOverEnter?.Invoke( this );

	public void OnOverExit( ) => onOverExit?.Invoke( this );

	public void OnCliked( ) => onClicked?.Invoke( this );

	public void OnRelease( ) => onRelease?.Invoke( this );

	public void OnDrop( ) => onDrop?.Invoke( this );

	public void OnBeginDrag( )
	{
		// Only execute if the card is NOT being dragged
		if ( dragging )
			return;
		else
			dragging = true;

		onStartedDrag?.Invoke( this );
	}

	public void OnEndDrag( )
	{
		// Only execute if the card was being dragged
		if ( !dragging )
			return;
		else
			dragging = false;

		onEndedDrag?.Invoke( this );
	}

	public void UpdateCardStatsFromEditor( CardType cardType, CardLevel cardLevel, string name, int cost,
		string ability, string flavor, GameObject instanceToSummon )
	{
		type = cardType;
		level = cardLevel;
		displayName = name;
		useCost = cost;
		abilityText = ability;
		flavorText = flavor;
		toSummon = instanceToSummon;
	}

	[ContextMenu( "Update Card Info" )]
	public void PopulateCardInfo( ) => Vizuals.PopulateCardInfo( type, toSummon, useCost, displayName, abilityText, flavorText, level );

	private void CheckIfWeEndedClickDrag( )
	{
		// We were 'click dragging' and pressed our mouse button
		if ( dragging && Input.GetMouseButtonDown( 0 ) )
			OnEndDrag( );
	}

	private void CheckIfCardCanBePlayed( )
	{
		if ( SelectionMode != CardSelectionMode.InHand )
			return;

		if ( SummoningManager.Instance.EnoughMana( useCost ) && !CanBePlayed )
		{
			CanBePlayed = true;
			vizuals.CanBePlayed( true );
		}
		else if ( !SummoningManager.Instance.EnoughMana( useCost ) && CanBePlayed )
		{
			CanBePlayed = false;
			vizuals.CanBePlayed( false );
		}
	}
}
