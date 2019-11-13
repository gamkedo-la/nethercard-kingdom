/**
 * Description: Manages and displays player's card collection (in deck builder).
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections.Generic;
using System.Linq;
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

	/*public void DraggedCardAddedToDeck( )
	{
		collection[draggedSlotIndex].Amount--;

		if ( collection[draggedSlotIndex].Amount == 0 )
			collection[draggedSlotIndex] = null;

		cardDragged = null;
		cardDraggedFromDeck = null;

		DisplayCollection( );
	}*/

	public void DraggedCardAddedToDeck( PlayerCard cardToSwap = null, Vector2 position = default )
	{
		int newSlotIndex = 0;
		collection[draggedSlotIndex].Amount--;

		// We dragged card from the collection that we had only 1 of, so we now have an empty slot
		if ( collection[draggedSlotIndex].Amount <= 0 )
		{
			newSlotIndex = draggedSlotIndex;

			// Either we do not swap cards OR the card we put in is something we do not have in the collection
			if ( cardToSwap == null || ( cardToSwap != null && collection.FirstOrDefault( card => card != null && card.Card.Name == cardToSwap.Card.Name ) == null ) )
			{
				collection[draggedSlotIndex] = cardToSwap;
			}
			// We already have a card of the same name in the collection
			else
			{
				PlayerCard existingCard = collection.First( card => card.Card.Name == cardToSwap.Card.Name );
				existingCard.Amount++;
				newSlotIndex = collection.IndexOf( existingCard );
				collection[draggedSlotIndex] = null;
			}
		}
		// We dragged a card from the collection, swapped with the one in deck, but now we need to put the card from the deck
		// (since the source collection slot is still taken we have to find an empty slot)
		else if ( cardToSwap != null )
		{
			for ( int i = 0; i < collection.Count; i++ )
			{
				// Found an empty slot
				if (collection[i] == null)
				{
					cardToSwap.Amount = 1; // We always add 1 card from the deck
					collection[i] = cardToSwap;
					newSlotIndex = i;

					break; // Nothing more to do here!
				}
			}
		}

		cardDragged = null;
		cardDraggedFromDeck = null;

		DisplayCollection( );

		if ( cardToSwap != null )
			slots[newSlotIndex].DoMove( position );
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
			if ( cardDragged.Card.Name == cardInDestinationSlot.Card.Name )
			{
				cardInDestinationSlot.Amount += cardDragged.Amount;
				collection[draggedSlotIndex] = null;

				DisplayCollection( );

				return;
			}

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
				cardFromDeck.Amount = 1;
				deckManager.DraggedCardAddedToCollection( );

				DisplayCollection( );

				return;
			}

			// To same type of cards
			if ( collection[dropSlotIndex].Card.Name == cardFromDeck.Card.Name )
			{
				Debug.Log( "Same cards, combining" );

				collection[dropSlotIndex].Amount++;
				deckManager.DraggedCardAddedToCollection( );

				DisplayCollection( );

				return;
			}

			// To different type of cards
			if ( collection[dropSlotIndex].Card.Name != cardFromDeck.Card.Name )
			{
				Debug.Log( "Swapping cards: deck -> collection (NOT YET IMPLEMENTED)" );

				PlayerCard cardToSwap = collection[dropSlotIndex];
				//collection[dropSlotIndex] = cardFromDeck;
				//deckManager.DraggedCardAddedToCollection( cardToSwap, slots[dropSlotIndex].CardPosition );

				DisplayCollection( );

				return;
			}
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
