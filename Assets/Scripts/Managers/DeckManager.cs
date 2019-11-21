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
using UnityEngine.Events;

public class DeckManager : MonoBehaviour
{
	[Header("External Objects")]
	[SerializeField] private PlayerCards playerCards = null;
	[SerializeField] private CollectionManager collectionManager = null;
	[SerializeField] private TextMeshProUGUI tooltip = null;

	[Header("Objects")]
	[SerializeField] private GameObject deckSlot = null;
	[SerializeField] private Transform slotsParent = null;

	[Header("Events")]
	[SerializeField] private UnityEventBool onCanSaveDeck = null;
	[SerializeField] private UnityEvent onWarning = null;


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
		deck[draggedSlotIndex] = cardFromCollection != null
			? new PlayerCard( )
			// New one to avoid having shared references in both deck and collection
			{
				Card = cardFromCollection.Card,
				Amount = 1
			}
			: null;

		DisplayDeck( );

		if ( cardFromCollection != null )
			slots[draggedSlotIndex].DoMove( position );
	}

	// Count the number of cards of the same type (even if they are of different levels)
	public bool WillWeExceedSameCardLimit( PlayerCard newCard, PlayerCard cardBeingChanged )
	{
		int sameCardsInDeck = 1; // Start with 1 to account for the card that we want to include in deck
		List<CardSlot> sameCards = new List<CardSlot>( );

		for ( int i = 0; i < deck.Count; i++ )
		{
			// Skipp empty slots
			if ( deck[i] == null )
				continue;

			if ( playerCards.AreCardsOfTheSameType( newCard, deck[i] ) )
			{
				sameCardsInDeck++;
				sameCards.Add( slots[i] );
			}
		}

		// We reached same card limit and we are putting in to empty slot
		// or we reached same card limit and the cards being swapped are different (in other words: skip if we swap same type cards)
		if ( ( sameCardsInDeck > PlayerCards.MaxIdenticalCardsInDeck && cardBeingChanged == null ) ||
			( sameCardsInDeck > PlayerCards.MaxIdenticalCardsInDeck && !playerCards.AreCardsOfTheSameType( newCard, cardBeingChanged ) ) )
		{
			tooltip.text = $"Can't have more then {PlayerCards.MaxIdenticalCardsInDeck} of the same card (or upgraded versions) in the deck";

			onWarning?.Invoke( );
			foreach ( var slot in sameCards )
				slot.OnWarning( );

			return true;
		}

		return false; // We are good (can add this card)
	}

	public void Save( ) => playerCards.SetDeck( deck );

	private void GetDeckCards( )
	{
		playerCards.LoadPlayerCardsData( );
		List<PlayerCard> playerDeck = playerCards.GetDeck( );

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
		cardDragged = null;
		cardDraggedFromCollection = null;

		for ( int i = 0; i < deck.Count; i++ )
			slots[i].Set( deck[i], i, CardDragedEvent, CardDroppedEvent, ClickedOnSlotEvent, false );

		// We should be able to save the deck only if we have all the slots in it filled
		int cardsInDeck = deck.Where( card => card != null ).Count( );
		onCanSaveDeck?.Invoke( cardsInDeck == PlayerCards.MaxCardsInDeck );
	}

	private void ClickedOnSlotEvent( int dropSlotIndex )
	{
		//Debug.Log( $"cardDragged == {cardDragged} && cardDraggedFromCollection == {cardDraggedFromCollection}" );
		// Card was clicked-drag (but not just empty slot)
		if ( cardDragged == null && cardDraggedFromCollection == null && slots[dropSlotIndex].Card != null )
		{
			slots[dropSlotIndex].OnCardStartDragging( );
			CardDragedEvent( dropSlotIndex, false );
		}
		else // Card was clicked-drop
		{
			CardDroppedEvent( dropSlotIndex );
			CardDragedEvent( dropSlotIndex, true );
		}
	}

	private void CardDroppedEvent( int dropSlotIndex )
	{
		tooltip.text = "You did something in the deck ;)";
		PlayerCard cardInDestinationSlot = slots[dropSlotIndex].Card;

		// Dragging withing Deck
		if ( cardDragged != null )
		{
			// Same card
			if ( cardDragged == cardInDestinationSlot )
			{
				tooltip.text = "Card placed in the same slot";

				return;
			}

			// Swap cards
			tooltip.text = "Cards in deck swapped";

			deck[dropSlotIndex] = cardDragged;
			deck[draggedSlotIndex] = cardInDestinationSlot;

			DisplayDeck( );

			slots[draggedSlotIndex].DoMove( slots[dropSlotIndex].CardPosition );
			slots[dropSlotIndex].OnInfromation( );

			return;
		}

		// Dragging from Collection
		if ( cardDraggedFromCollection != null )
		{
			PlayerCard cardFromCollection = collectionManager.GetDraggedCard( );

			// To empty slot
			if ( slots[dropSlotIndex].Card == null )
			{
				if ( WillWeExceedSameCardLimit( cardFromCollection, null ) )
					return;

				tooltip.text = "Card from collection put in to empty slot";

				deck[dropSlotIndex] = new PlayerCard( )
				// New one to avoid having shared references in both deck and collection
				{
					Card = cardFromCollection.Card,
					Amount = 1
				};
				collectionManager.DraggedCardAddedToDeck( slots[dropSlotIndex].CardPosition );

				DisplayDeck( );
				slots[dropSlotIndex].OnInfromation( );

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
			PlayerCard cardToSwap = deck[dropSlotIndex];

			if ( WillWeExceedSameCardLimit( cardFromCollection, cardToSwap ) )
				return;

			tooltip.text = "Swapped card from collection -> deck";

			deck[dropSlotIndex] = new PlayerCard( )
			// New one to avoid having shared references in both deck and collection
			{
				Card = cardFromCollection.Card,
				Amount = 1
			};
			collectionManager.DraggedCardAddedToDeck( slots[dropSlotIndex].CardPosition, cardToSwap );

			DisplayDeck( );
			slots[dropSlotIndex].OnInfromation( );
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
			tooltip.text = "Place card in an empty slot or swap with another one";
		}

		//string s = endOfDrag ? "stop" : "start";
		//Debug.Log( $"Collection dragged: {s} {index}" );
		draggedSlotIndex = endOfDrag ? int.MinValue : index; // Index od the dragged card or "null" (int.MinValue)
	}
}
