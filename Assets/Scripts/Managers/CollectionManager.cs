/**
 * Description: Manages and displays player's card collection (in deck builder).
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CollectionManager : MonoBehaviour
{
	[Header("External Objects")]
	[SerializeField] private PlayerCards playerCards = null;
	[SerializeField] private DeckManager deckManager = null;

	[Header("Objects")]
	[SerializeField] private GameObject collectionSlot = null;
	[SerializeField] private Transform slotsParent = null;

	[Header("Parameters")]
	[SerializeField] private int minSlots = 20;

	private List<CardSlot> slots = new List<CardSlot>();
	private List<PlayerCard> collection = new List<PlayerCard>();
	private int draggedSlotIndex = int.MinValue;
	private PlayerCard cardDragged;
	private PlayerCard cardDraggedFromDeck;

	void Start ()
	{
		Assert.IsNotNull( playerCards, $"Please assign <b>{nameof( playerCards )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( deckManager, $"Please assign <b>{nameof( deckManager )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( collectionSlot, $"Please assign <b>{nameof( collectionSlot )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( slotsParent, $"Please assign <b>{nameof( slotsParent )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		GetCollectionCards( );
		CreateLayout( );
		DisplayCollection( );
	}

	public void SetDraggedCard( PlayerCard card )
	{
		cardDraggedFromDeck = card;
	}

	public PlayerCard GetDraggedCard( )
	{
		return cardDragged;
	}

	public void DraggedCardAddedToDeck( )
	{
		collection[draggedSlotIndex].Amount--;

		if ( collection[draggedSlotIndex].Amount == 0 )
			collection[draggedSlotIndex] = null;

		cardDragged = null;
		cardDraggedFromDeck = null;

		DisplayCollection( );
	}

	private void GetCollectionCards( )
	{
		playerCards.LoadPlayerCardsData( );
		List<PlayerCard> playerCollection = playerCards.GetCollection;

		for ( int i = 0; i < minSlots; i++ )
		{
			if ( i < playerCollection.Count )
				collection.Add( playerCollection[i] );
			else
				collection.Add( null );
		}
	}

	private void CreateLayout( )
	{
		slots.Clear( );

		for ( int i = 0; i < minSlots; i++ )
		{
			GameObject slot = Instantiate( collectionSlot, slotsParent );
			slots.Add( slot.GetComponent<CardSlot>( ) );
		}
	}

	private void DisplayCollection( )
	{
		for ( int i = 0; i < collection.Count; i++ )
			slots[i].Set( collection[i], i, CardDragedEvent, DroppedOnSlotEvent );
	}

	private void DroppedOnSlotEvent( int dropSlotIndex )
	{
		//Debug.Log( $"Collection dropped: {dropSlotIndex}" );
		PlayerCard cardInDestinationSlot = slots[dropSlotIndex].Card;

		// Dragging within Collection
		if ( cardDragged != null )
		{
			// Same card
			if ( cardDragged == cardInDestinationSlot )
				return;

			// Combine piles cards (card of the same name dropped on one another)

			// Swap cards
			collection[dropSlotIndex] = cardDragged;
			collection[draggedSlotIndex] = cardInDestinationSlot;

			cardDragged = null;
			cardDraggedFromDeck = null;

			DisplayCollection( );

			slots[draggedSlotIndex].DoMove( slots[dropSlotIndex].CardPosition );

			return;
		}

		// Dragging from Deck
		if ( cardDraggedFromDeck != null )
		{
			Debug.Log( "Received drag from Deck" );
			PlayerCard cardFromDeck = deckManager.GetDraggedCard( );

			// To empty slot
			if ( slots[dropSlotIndex].Card == null )
			{
				Debug.Log( "Putting in to an empty slot" );

				collection[dropSlotIndex] = cardFromDeck;
				deckManager.DraggedCardAddedToCollection( );

				DisplayCollection( );

				return;
			}

			// To same type of cards
			if ( collection[dropSlotIndex].Card.Name == cardFromDeck.Card.Name )
			{
				Debug.Log( "Same cards, skipping" );
				return;
			}

			// To different type of cards
			Debug.Log( "Different cards, NO FUNCTIONALITY YET" );
		}
	}

	private void CardDragedEvent( int index, bool endOfDrag )
	{
		if ( endOfDrag )
		{
			//Debug.Log( $"End of drag: {index}" );
			cardDragged = null;
			deckManager.SetDraggedCard( cardDragged );
		}
		else
		{
			cardDragged = slots[index].Card;
			deckManager.SetDraggedCard( cardDragged );
		}

		//string s = endOfDrag ? "stop" : "start";
		//Debug.Log( $"Collection dragged: {s} {index}" );
		draggedSlotIndex = endOfDrag ? int.MinValue : index; // Index od the dragged card or "null"
	}
}
