/**
 * Description: Card slot for use in deck building (deck, collection and upgrade slots).
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class CardSlot : MonoBehaviour
{
	public PlayerCard Card { get; private set; }
	public Vector2 CardPosition { get { return cardHolder.position; } }

	[SerializeField] private GameObject amountDisplay = null;
	[SerializeField] private TextMeshProUGUI amountLabel = null;
	[SerializeField] private Transform cardHolder = null;
	[SerializeField] private CardSelectionMode mode = CardSelectionMode.InCollection;

	private Card cardInSlot = null;
	private bool cardIsDraged = false;
	private bool returning = false;
	private bool appearing = true;
	private bool showAsDisabled = false;
	private int index = int.MinValue;
	private System.Action<int,bool> onDrag;
	private System.Action<int> onDrop;
	private System.Action<int> onClick;
	private Vector3 dragOffset = Vector3.zero;

	void Start ()
	{
		Assert.IsNotNull( amountDisplay, $"Please assign <b>{nameof( amountDisplay )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( amountLabel, $"Please assign <b>{nameof( amountLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( cardHolder, $"Please assign <b>{nameof( cardHolder )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		Invoke( nameof( Appeared ), 0.7f ); // To prevent cards "jumping" when deck builder first appears
	}

	private void Appeared( ) => appearing = false;

	void Update( )
	{
		if ( cardIsDraged && cardInSlot )
		{
			cardInSlot.transform.position = Vector2.Lerp( cardInSlot.transform.position, Input.mousePosition - dragOffset, 0.25f );
		}
		else if ( cardInSlot && !appearing )
		{
			cardInSlot.transform.position = Vector2.Lerp( cardInSlot.transform.position, cardHolder.position, 0.15f );

			if ( returning && Vector2.Distance( cardInSlot.transform.position, cardHolder.position ) < 5 )
			{
				returning = false;
				cardInSlot.Vizuals.NormalCard( );
				cardInSlot.Vizuals.OnInformation( );
			}
		}
	}

	public void Set( PlayerCard playerCard, int index, System.Action<int,bool> onDrag, System.Action<int> onDrop, System.Action<int> onClick, bool upgrading, bool forceDisable = false )
	{
		Clear( );

		Card = playerCard;
		this.index = index;
		this.onDrag = onDrag;
		this.onDrop = onDrop;
		this.onClick = onClick;

		// Empty slot
		if ( playerCard == null )
			return;

		GameObject go = Instantiate( playerCard.Card.gameObject, cardHolder.position, Quaternion.identity, cardHolder );
		go.transform.localPosition = Vector3.zero;

		cardInSlot = go.GetComponent<Card>( );
		cardInSlot.SelectionMode = mode;
		cardInSlot.onStartedDrag.AddListener( card => OnCardStartDragging( ) );
		cardInSlot.onEndedDrag.AddListener( card => OnCardEndDragging( ) );
		cardInSlot.onOverEnter.AddListener( card => OnCardOverEnter( ) );
		cardInSlot.onOverExit.AddListener( card => OnCardOverExit( ) );
		cardInSlot.onDrop.AddListener( card => OnCardDrop( ) );
		cardInSlot.onClicked.AddListener( card => OnClicked( ) );
		cardInSlot.onRelease.AddListener( card => OnCardRelease( ) );
		cardInSlot.SelectionMode = mode;

		cardInSlot.Vizuals.SelectionMode = mode;
		cardInSlot.Vizuals.SetStack( playerCard.Amount );

		amountLabel.text = $"x{playerCard.Amount}";

		if ( mode == CardSelectionMode.InCollection )
			amountDisplay.SetActive( true );

		// Disable cards that are fewer then the min amount for upgrading and that have max level
		if ( (upgrading && ( playerCard.Amount < PlayerCards.MinCardsForUpgrade || !cardInSlot.HigherLevelVersion ) ) || forceDisable )
		{
			showAsDisabled = true;
			cardInSlot.Vizuals.SetDisabled( );
		}
	}

	public void Canceled( )
	{
		cardIsDraged = false;
		returning = true;

		cardInSlot.Vizuals.Back( );
	}

	public void DoMove( Vector2 position )
	{
		cardInSlot.transform.position = position;
		cardInSlot.Vizuals.DraggedCard( true );
		returning = true;
	}

	public void Clear( )
	{
		amountDisplay.SetActive( false );
		cardIsDraged = false;
		showAsDisabled = false;
		Card = null;

		if ( cardInSlot )
		{
			cardInSlot.onStartedDrag.RemoveAllListeners( );
			cardInSlot.onEndedDrag.RemoveAllListeners( );
			cardInSlot.onOverEnter.RemoveAllListeners( );
			cardInSlot.onOverExit.RemoveAllListeners( );
			cardInSlot.onClicked.RemoveAllListeners( );
			cardInSlot.onRelease.RemoveAllListeners( );
			cardInSlot.onDrop.RemoveAllListeners( );

			Destroy( cardInSlot.gameObject );
		}
	}

	public void OnCardDrop( )
	{
		if ( showAsDisabled )
			return;

		onDrop?.Invoke( index );
	}

	public void OnClicked( )
	{
		if ( showAsDisabled )
			return;

		onClick?.Invoke( index );
	}

	public void OnWarning( ) => cardInSlot.Vizuals.OnWarning( );

	public void OnInfromation( ) => cardInSlot.Vizuals.OnInformation( );

	private void OnCardOverEnter( )
	{
		if ( showAsDisabled )
			return;

		if ( !cardIsDraged && !returning )
			cardInSlot.Vizuals.HighlightCardInDeck( );
	}

	private void OnCardOverExit( )
	{
		if ( showAsDisabled )
			return;

		if ( !cardIsDraged && !returning )
			cardInSlot.Vizuals.NormalCard( );
	}

	public void OnCardStartDragging( )
	{
		if ( showAsDisabled )
			return;

		cardIsDraged = true;
		dragOffset = Input.mousePosition - cardInSlot.transform.position;

		cardInSlot.Vizuals.DraggedCard( false );
		onDrag?.Invoke( index, false );
	}

	private void OnCardEndDragging( )
	{
		if ( showAsDisabled )
			return;

		if ( !cardIsDraged )
			return;

		cardIsDraged = false;
		returning = true;

		cardInSlot.Vizuals.Back( );
		onDrag?.Invoke( index, true );
	}

	private void OnCardRelease( )
	{
		if ( showAsDisabled )
			return;

		if ( !cardIsDraged )
			return;
	}
}
