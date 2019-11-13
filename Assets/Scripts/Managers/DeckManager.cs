/**
 * Description: Managed and displays player deck (in deck builder).
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class DeckManager : MonoBehaviour
{
	[Header("External Objects")]
	[SerializeField] private PlayerCards playerCards = null;
	[SerializeField] private CollectionManager collectionManager = null;
	[SerializeField] private TextMeshProUGUI tooltip = null;

	[Header("Objects")]
	[SerializeField] private GameObject deckSlot = null;
	[SerializeField] private Transform slotsParent = null;

	[Header("Objects")]
	[SerializeField] private UnityEventBool onCanSaveDeck = null;


	private List<CardSlot> slots = new List<CardSlot>();
	private List<PlayerCard> deck = new List<PlayerCard>();
	private int slotNumber;
	private int draggedSlotIndex = int.MinValue;
	private PlayerCard cardDragged;
	private PlayerCard cardDraggedFromCollection;

	void Start( )
	{
		Assert.IsNotNull( playerCards, $"Please assign <b>{nameof( playerCards )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( collectionManager, $"Please assign <b>{nameof( collectionManager )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( deckSlot, $"Please assign <b>{nameof( deckSlot )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( slotsParent, $"Please assign <b>{nameof( slotsParent )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( tooltip, $"Please assign <b>{nameof( tooltip )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		slotNumber = PlayerCards.MaxCardsInDeck;

		GetDeckCards( );
		CreateLayout( );
		DisplayDeck( );
	}

	public void SetDraggedCard( PlayerCard card )
	{
		cardDraggedFromCollection = card;
	}

	public PlayerCard GetDraggedCard( )
	{
		return cardDragged;
	}

	public void DraggedCardAddedToCollection( Vector2 position = default, PlayerCard cardFromCollection = null )
	{
		deck[draggedSlotIndex] = cardFromCollection;

		cardDragged = null;
		cardDraggedFromCollection = null;

		DisplayDeck( );

		if ( cardFromCollection != null )
			slots[draggedSlotIndex].DoMove( position );
	}

	private void GetDeckCards( )
	{
		playerCards.LoadPlayerCardsData( );
		List<PlayerCard> playerDeck = playerCards.GetDeck;

		for ( int i = 0; i < slotNumber; i++ )
		{
			if ( i < playerDeck.Count )
				deck.Add( playerDeck[i] );
			else
				deck.Add( null );
		}
	}

	private void CreateLayout( )
	{
		slots.Clear( );

		for ( int i = 0; i < slotNumber; i++ )
		{
			GameObject slot = Instantiate( deckSlot, slotsParent );
			slots.Add( slot.GetComponent<CardSlot>( ) );
		}
	}

	private void DisplayDeck( )
	{
		for ( int i = 0; i < deck.Count; i++ )
			slots[i].Set( deck[i], i, CardDragedEvent, CardDroppedEvent );

		int cardsInDeck = deck.Where( card => card != null ).Count( );
		onCanSaveDeck?.Invoke( cardsInDeck == PlayerCards.MaxCardsInDeck );
	}

	private void CardDroppedEvent( int dropSlotIndex )
	{
		tooltip.text = "Card dropped in deck";
		PlayerCard cardInDestinationSlot = slots[dropSlotIndex].Card;

		// Dragging withing Deck
		if ( cardDragged != null )
		{
			// Same card
			if ( cardDragged == cardInDestinationSlot )
			{
				tooltip.text = "Same cards in deck swapped";

				return;
			}

			// Swap cards
			tooltip.text = "Cards in deck swapped";

			deck[dropSlotIndex] = cardDragged;
			deck[draggedSlotIndex] = cardInDestinationSlot;

			cardDragged = null;
			cardDraggedFromCollection = null;

			DisplayDeck( );

			slots[draggedSlotIndex].DoMove( slots[dropSlotIndex].CardPosition );

			return;
		}

		// Dragging from Collection
		if ( cardDraggedFromCollection != null )
		{
			PlayerCard cardFromCollection = collectionManager.GetDraggedCard( );

			// To empty slot
			if ( slots[dropSlotIndex].Card == null )
			{
				tooltip.text = "Card from collection put in to empty slot";

				deck[dropSlotIndex] = cardFromCollection;
				collectionManager.DraggedCardAddedToDeck( );

				DisplayDeck( );

				return;
			}

			// Same type (the same card)
			if ( deck[dropSlotIndex].Card.Name == cardFromCollection.Card.Name )
			{
				tooltip.text = "Same card from collection swapped in deck";

				return;
			}

			// Different card types
			// deck[dropSlotIndex].Card.Name != cardFromCollection.Card.Name
			tooltip.text = "Swapped card from collection -> deck";

			PlayerCard cardToSwap = deck[dropSlotIndex];
			cardToSwap.Amount = 1;

			deck[dropSlotIndex] = cardFromCollection;
			collectionManager.DraggedCardAddedToDeck( slots[dropSlotIndex].CardPosition, cardToSwap );

			DisplayDeck( );
		}
	}

	private void CardDragedEvent( int index, bool endOfDrag )
	{
		if ( endOfDrag )
		{
			//Debug.Log( $"End of drag: {index}" );
			cardDragged = null;
			collectionManager.SetDraggedCard( cardDragged );
		}
		else
		{
			cardDragged = slots[index].Card;
			collectionManager.SetDraggedCard( cardDragged );
		}

		//string s = endOfDrag ? "stop" : "start";
		//Debug.Log( $"Collection dragged: {s} {index}" );
		draggedSlotIndex = endOfDrag ? int.MinValue : index; // Index od the dragged card or "null"
	}
}
