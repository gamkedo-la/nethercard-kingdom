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

	private Card cardInSlot;
	private bool cardIsDraged = false;
	private bool returning = false;
	private int index;
	private System.Action<int,bool> onDrag;
	private System.Action<int> onDrop;

	void Start ()
	{
		Assert.IsNotNull( amount, $"Please assign <b>{nameof( amount )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( cardHolder, $"Please assign <b>{nameof( cardHolder )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( selection, $"Please assign <b>{nameof( selection )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( amountLabel, $"Please assign <b>{nameof( amountLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	void Update( )
	{
		if ( cardIsDraged && cardInSlot )
		{
			cardInSlot.transform.position = Vector2.Lerp( cardInSlot.transform.position, Input.mousePosition + new Vector3( 0.0f, -Screen.height * 0.25f, 0.0f ), 0.25f );
		}
		else if ( cardInSlot )
		{
			cardInSlot.transform.position = Vector2.Lerp( cardInSlot.transform.position, cardHolder.position, 0.15f );

			if ( returning && Vector2.Distance( cardInSlot.transform.position, cardHolder.position ) < 5 )
			{
				returning = false;
				cardInSlot.GetComponent<CardAudioVisuals>( ).NormalCard( );
			}
		}
	}

	public void Set( PlayerCard playerCard, int index, System.Action<int,bool> onDrag, System.Action<int> onDrop )
	{
		Clear( );

		Card = playerCard;
		this.index = index;
		this.onDrag = onDrag;
		this.onDrop = onDrop;

		// Empty slot
		if ( playerCard == null )
			return;

		GameObject go = Instantiate( playerCard.Card.gameObject, cardHolder.position, Quaternion.identity, cardHolder );
		cardInSlot = go.GetComponent<Card>( );
		go.GetComponent<CardNew>( ).SelectionMode = mode;
		go.GetComponent<CardNew>( ).onStartedDrag.AddListener( card => OnCardStartDragging( ) );
		go.GetComponent<CardNew>( ).onEndedDrag.AddListener( card => OnCardEndDragging( ) );
		go.GetComponent<CardNew>( ).onOverEnter.AddListener( card => OnCardOverEnter( ) );
		go.GetComponent<CardNew>( ).onOverExit.AddListener( card => OnCardOverExit( ) );
		go.GetComponent<CardNew>( ).onDrop.AddListener( card => OnCardDrop( ) );
		go.GetComponent<CardNew>( ).onRelease.AddListener( card => OnCardRelease( ) );
		go.GetComponent<CardNew>( ).SelectionMode = mode;
		go.GetComponent<CardAudioVisuals>( ).SelectionMode = mode;
		cardInSlot.SelectionMode = mode;
		cardInSlot.onStartedDrag.AddListener( ( ) => cardIsDraged = true );
		cardInSlot.onEndedDrag.AddListener( ( ) => cardIsDraged = false );

		amountLabel.text = playerCard.Amount.ToString( );

		if ( mode == CardSelectionMode.InCollection )
			amount.SetActive( true );
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

		if ( cardInSlot )
		{
			cardInSlot.onStartedDrag.RemoveAllListeners( );
			cardInSlot.onEndedDrag.RemoveAllListeners( );
			cardInSlot.GetComponent<CardNew>( ).onStartedDrag.RemoveAllListeners( );
			cardInSlot.GetComponent<CardNew>( ).onEndedDrag.RemoveAllListeners( );
			cardInSlot.GetComponent<CardNew>( ).onOverEnter.RemoveAllListeners( );
			cardInSlot.GetComponent<CardNew>( ).onOverExit.RemoveAllListeners( );
			cardInSlot.GetComponent<CardNew>( ).onDrop.RemoveAllListeners( );
			cardInSlot.GetComponent<CardNew>( ).onRelease.RemoveAllListeners( );

			Destroy( cardInSlot.gameObject );
		}
	}

	public void Select( bool selected )
	{
		selection.SetActive( selected );
	}

	public void OnCardDrop( )
	{
		///DeckBuilder.Instance.otherSlot = transform.parent.gameObject;
		///DeckBuilder.Instance.MoveSlot( );
		//Debug.Log( $"Drop: {name} -> {cardInSlot.GetComponent<CardNew>( ).Name}" );
		//Debug.Log( $"Drop: {name}" );
		onDrop.Invoke( index );
	}

	private void OnCardOverEnter( )
	{
		if ( !cardIsDraged )
			cardInSlot.GetComponent<CardAudioVisuals>( ).HighlightCard( );
	}

	private void OnCardOverExit( )
	{
		if ( !cardIsDraged )
			cardInSlot.GetComponent<CardAudioVisuals>( ).NormalCard( );
	}

	private void OnCardStartDragging( )
	{
		cardIsDraged = true;
		cardInSlot.GetComponent<CardAudioVisuals>( ).DraggedCard( );
		onDrag.Invoke( index, false );
	}

	private void OnCardEndDragging( )
	{
		cardIsDraged = false;
		returning = true;
		onDrag.Invoke( index, true );
	}

	private void OnCardRelease( )
	{
		//Debug.Log( $"{name} drop event" );
	}
}
