/**
 * Description: Card slot for use in deck building and card collection.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class CardSlot : MonoBehaviour
{
	public PlayerCard Card { get; private set; }
	public Vector2 CardPosition { get { return cardHolder.position; } }

	[SerializeField] private GameObject amount = null;
	[SerializeField] private Transform cardHolder = null;
	[SerializeField] private GameObject selection = null;
	[SerializeField] private TextMeshProUGUI amountLabel = null;
	[SerializeField] private CardSelectionMode mode = CardSelectionMode.InCollection;

	private Card cardInSlot = null;
	private bool cardIsDraged = false;
	private bool returning = false;
	private int index = int.MinValue;
	private System.Action<int,bool> onDrag;
	private System.Action<int> onDrop;
	private System.Action<int> onClick;
	private bool appearing = true;
	private bool showAsDisabled = false;
	private Vector3 dragOffset = Vector3.zero;

	void Start ()
	{
		Assert.IsNotNull( amount, $"Please assign <b>{nameof( amount )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( cardHolder, $"Please assign <b>{nameof( cardHolder )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( selection, $"Please assign <b>{nameof( selection )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( amountLabel, $"Please assign <b>{nameof( amountLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		Invoke( nameof( Appeared ), 0.7f ); // To prevent cards "jumping" when deck builder first appears
	}

	private void Appeared( ) => appearing = false;

	void Update( )
	{
		if ( cardIsDraged && cardInSlot )
		{
			Vector3 offset = CheatAndDebug.Instance.CardPosOffsetOnDrag ? new Vector3( 0.0f, -Screen.height * 0.25f, 0.0f ) : -dragOffset;
			cardInSlot.transform.position = Vector2.Lerp( cardInSlot.transform.position, Input.mousePosition + offset, 0.25f );
		}
		else if ( cardInSlot && !appearing )
		{
			cardInSlot.transform.position = Vector2.Lerp( cardInSlot.transform.position, cardHolder.position, 0.15f );

			if ( returning && Vector2.Distance( cardInSlot.transform.position, cardHolder.position ) < 5 )
			{
				returning = false;
				cardInSlot.GetComponent<CardAudioVisuals>( ).NormalCard( );
				cardInSlot.GetComponent<CardAudioVisuals>( ).OnInformation( );
			}
		}
	}

	public void Set( PlayerCard playerCard, int index, System.Action<int,bool> onDrag, System.Action<int> onDrop, System.Action<int> onClick, bool upgrading )
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
		//cardInSlot.transform.localPosition = Vector3.zero;
		go.transform.localPosition = Vector3.zero;
		cardInSlot = go.GetComponent<Card>( );
		go.GetComponent<CardNew>( ).SelectionMode = mode;
		go.GetComponent<CardNew>( ).onStartedDrag.AddListener( card => OnCardStartDragging( ) );
		go.GetComponent<CardNew>( ).onEndedDrag.AddListener( card => OnCardEndDragging( ) );
		go.GetComponent<CardNew>( ).onOverEnter.AddListener( card => OnCardOverEnter( ) );
		go.GetComponent<CardNew>( ).onOverExit.AddListener( card => OnCardOverExit( ) );
		go.GetComponent<CardNew>( ).onDrop.AddListener( card => OnCardDrop( ) );
		go.GetComponent<CardNew>( ).onClicked.AddListener( card => OnClicked( ) );
		go.GetComponent<CardNew>( ).onRelease.AddListener( card => OnCardRelease( ) );
		go.GetComponent<CardNew>( ).SelectionMode = mode;
		go.GetComponent<CardAudioVisuals>( ).SelectionMode = mode;
		go.GetComponent<CardAudioVisuals>( ).SetStack( playerCard.Amount );
		cardInSlot.SelectionMode = mode;
		cardInSlot.onStartedDrag.AddListener( ( ) => cardIsDraged = true );
		cardInSlot.onEndedDrag.AddListener( ( ) => cardIsDraged = false );

		amountLabel.text = $"x{playerCard.Amount}";

		if ( mode == CardSelectionMode.InCollection )
			amount.SetActive( true );

		// Hide cards that are fewer then the min amount for upgrading and that have max level
		if ( upgrading && ( playerCard.Amount < PlayerCards.MinCardsForUpgrade || !go.GetComponent<CardNew>( ).HigherLevelVersion ) )
		{
			showAsDisabled = true;
			go.GetComponent<CardAudioVisuals>( ).SetDisabled( );
		}
	}

	public bool IsEmpty( )
	{
		return !cardInSlot;
	}

	public void DoMove( Vector2 position )
	{
		cardInSlot.transform.position = position;
		cardInSlot.GetComponent<CardAudioVisuals>( ).DraggedCard( );
		returning = true;
	}

	public void Clear( )
	{
		amount.SetActive( false );
		cardIsDraged = false;
		Card = null;
		showAsDisabled = false;

		if ( cardInSlot )
		{
			cardInSlot.onStartedDrag.RemoveAllListeners( );
			cardInSlot.onEndedDrag.RemoveAllListeners( );
			cardInSlot.GetComponent<CardNew>( ).onStartedDrag.RemoveAllListeners( );
			cardInSlot.GetComponent<CardNew>( ).onEndedDrag.RemoveAllListeners( );
			cardInSlot.GetComponent<CardNew>( ).onOverEnter.RemoveAllListeners( );
			cardInSlot.GetComponent<CardNew>( ).onOverExit.RemoveAllListeners( );
			cardInSlot.GetComponent<CardNew>( ).onClicked.RemoveAllListeners( );
			cardInSlot.GetComponent<CardNew>( ).onRelease.RemoveAllListeners( );
			cardInSlot.GetComponent<CardNew>( ).onDrop.RemoveAllListeners( );

			Destroy( cardInSlot.gameObject );
		}
	}

	public void Select( bool selected )
	{
		selection.SetActive( selected );
	}

	public void OnCardDrop( )
	{
		if ( showAsDisabled )
			return;

		///DeckBuilder.Instance.otherSlot = transform.parent.gameObject;
		///DeckBuilder.Instance.MoveSlot( );
		//Debug.Log( $"Drop: {name} -> {cardInSlot.GetComponent<CardNew>( ).Name}" );
		//Debug.Log( $"Drop: {name}" );
		onDrop?.Invoke( index );
	}

	public void OnClicked( )
	{
		if ( showAsDisabled )
			return;

		//Debug.Log( $"{name} click event" );
		onClick?.Invoke( index );
	}

	public void OnWarning( )
	{
		cardInSlot.GetComponent<CardAudioVisuals>( ).OnWarning( );
	}

	public void OnInfromation( )
	{
		cardInSlot.GetComponent<CardAudioVisuals>( ).OnInformation( );
	}

	private void OnCardOverEnter( )
	{
		if ( showAsDisabled )
			return;

		if ( !cardIsDraged && !returning )
			cardInSlot.GetComponent<CardAudioVisuals>( ).HighlightCardInDeck( );
	}

	private void OnCardOverExit( )
	{
		if ( showAsDisabled )
			return;

		if ( !cardIsDraged && !returning )
			cardInSlot.GetComponent<CardAudioVisuals>( ).NormalCard( );
	}

	public void OnCardStartDragging( )
	{
		if ( showAsDisabled )
			return;

		cardIsDraged = true;
		dragOffset = Input.mousePosition - cardInSlot.transform.position;
		cardInSlot.GetComponent<CardAudioVisuals>( ).DraggedCard( );
		onDrag?.Invoke( index, false );
	}

	private void OnCardEndDragging( )
	{
		if ( showAsDisabled )
			return;

		cardIsDraged = false;
		returning = true;
		onDrag?.Invoke( index, true );
	}

	private void OnCardRelease( )
	{
		if ( showAsDisabled )
			return;

		//Debug.Log( $"{name} drop event" );
	}
}
