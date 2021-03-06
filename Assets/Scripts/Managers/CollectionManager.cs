﻿/**
 * Description: Manages and displays player's card collection (in deck builder).
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CollectionManager : MonoBehaviour
{
	public bool AutoSort { get; set; } = true;

	[Header("External Objects")]
	[SerializeField] private PlayerCards playerCards = null;
	[SerializeField] private DeckManager deckManager = null;
	[SerializeField] private TextMeshProUGUI tooltip = null;
	[SerializeField] private Button upgradeButton = null;
	[SerializeField] private GameObject cantUpgradeTip = null;

	[Header("Objects")]
	[SerializeField] private GameObject collectionSlot = null;
	[SerializeField] private Transform slotsParent = null;

	[Header("Upgrading")]
	[SerializeField] private CardSlot upgradSlot1 = null;
	[SerializeField] private CardSlot upgradSlot2 = null;
	[SerializeField] private GameObject upgradeDisplay = null;

	[Header("Parameters")]
	[SerializeField] private int minSlots = 20;

	[Header("Events")]
	[SerializeField] private UnityEventBool onDisableOnDragging = null;

	private List<CardSlot> slots = new List<CardSlot>();
	private List<PlayerCard> collection = new List<PlayerCard>();
	private int draggedSlotIndex = int.MinValue;
	private PlayerCard cardDragged = null;
	private PlayerCard cardDraggedFromDeck = null;
	private PlayerCard upgradingCard = null;
	private bool upgrading = false;

	void Start ()
	{
		Assert.IsNotNull( cantUpgradeTip, $"Please assign <b>{nameof( cantUpgradeTip )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( playerCards, $"Please assign <b>{nameof( playerCards )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( deckManager, $"Please assign <b>{nameof( deckManager )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( collectionSlot, $"Please assign <b>{nameof( collectionSlot )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( slotsParent, $"Please assign <b>{nameof( slotsParent )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( tooltip, $"Please assign <b>{nameof( tooltip )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( upgradeButton, $"Please assign <b>{nameof( upgradeButton )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( upgradSlot1, $"Please assign <b>{nameof( upgradSlot1 )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( upgradSlot2, $"Please assign <b>{nameof( upgradSlot2 )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		GetCollectionCards( );
		CreateLayout( );
		if ( AutoSort ) SortCollection( );
		DisplayCollection( );
	}

	void Update( )
	{
		CheckIfWeForceCanceled( );
	}

	public void SetDraggedCard( PlayerCard card ) => cardDraggedFromDeck = card;

	public PlayerCard GetDraggedCard( ) => cardDragged;

	public void UpgradingWindow( bool opening )
	{
		upgrading = opening;

		// Clean up and close
		if ( !opening )
		{
			CleanUpUpgradeState( true, true );
			return;
		}

		upgradSlot1.Set( null, 0, null, UpgradeSlotDroppedEvent, UpgradeSlotClickedEvent, false );
		upgradSlot2.Set( null, 1, null, UpgradeSlotDroppedEvent, UpgradeSlotClickedEvent, false );

		bool forceDisable = !CanWeFillTheDeck( );
		DisplayCollection( forceDisable );
	}

	public void UpgradeCard( )
	{
		string destinationCardName = upgradingCard.Card.HigherLevelVersion.Name;

		// Try to find higher level version of card we are upgrading in collection
		for ( int i = 0; i < collection.Count; i++ )
		{
			if ( collection[i] != null && collection[i].Card.Name == destinationCardName ) // Found a matching card: add to it and cleanup
			{
				upgradingCard.Amount -= 2; // Since this is reference to the source card let's remove cards from there before adding
				collection[i].Amount += 1;

				CleanUpUpgradeState( true );

				slots[i].DoMove( ( upgradSlot1.CardPosition + upgradSlot2.CardPosition ) / 2 );

				upgradeDisplay.SetActive(true);

				if(upgradeDisplay.transform.GetChild(0).childCount > 0)
					Destroy(upgradeDisplay.transform.GetChild(0).GetChild(0).gameObject);

				GameObject upgradeDisplayCard = Instantiate(slots[i].gameObject, upgradeDisplay.transform.GetChild(0));
				upgradeDisplayCard.transform.position = upgradeDisplayCard.transform.parent.position;

				return;
			}
		}

		// Couldn't find a matching card, find first empty slot and add new one
		for ( int i = 0; i < collection.Count; i++ )
		{
			if ( collection[i] == null ) // Found an empty slot: add to it and cleanup
			{
				upgradingCard.Amount -= 2; // Since this is reference to the source card let's remove cards from there before adding
				collection[i] = new PlayerCard( )
				{
					Card = playerCards.GetPlayerCardByName( destinationCardName ),
					Amount = 1
				};

				CleanUpUpgradeState( true );

				slots[i].DoMove( ( upgradSlot1.CardPosition + upgradSlot2.CardPosition ) / 2 );

				upgradeDisplay.SetActive(true);

				if(upgradeDisplay.transform.GetChild(0).childCount > 0)
					Destroy(upgradeDisplay.transform.GetChild(0).GetChild(0).gameObject);

				GameObject upgradeDisplayCard = Instantiate(slots[i].gameObject, upgradeDisplay.transform.GetChild(0));
				upgradeDisplayCard.transform.position = upgradeDisplayCard.transform.parent.position;

				return;
			}
		}

		Debug.LogError( "Couldn't upgrade the card!" );
	}

	private void UpgradeSlotDroppedEvent( int _ )
	{
		if ( upgradingCard != null )
			CleanUpUpgradeState( false );

		if ( cardDragged == null )
			return;

		cardDragged.Amount -= 2;
		upgradingCard = cardDragged;

		upgradeButton.interactable = true;

		upgradSlot1.Set( upgradingCard, 0, UpgradeSlotDragEvent, UpgradeSlotDroppedEvent, UpgradeSlotClickedEvent, false );
		upgradSlot2.Set( upgradingCard, 1, UpgradeSlotDragEvent, UpgradeSlotDroppedEvent, UpgradeSlotClickedEvent, false );

		upgradSlot1.OnInfromation( );
		upgradSlot2.OnInfromation( );

		DisplayCollection( );
		onDisableOnDragging?.Invoke( true );
	}

	private void UpgradeSlotClickedEvent( int _ )
	{
		if ( upgradSlot1.Card != null )
		{
			int sourceSlotIndex = collection.IndexOf( upgradingCard );
			CleanUpUpgradeState( );

			slots[sourceSlotIndex].DoMove( ( upgradSlot1.CardPosition + upgradSlot2.CardPosition ) / 2 );
		}
		else if ( cardDragged != null )
		{
			UpgradeSlotDroppedEvent( default );
		}
	}

	private void CleanUpUpgradeState( bool updateCollection = true, bool closing = false )
	{
		upgradSlot1.Clear( );
		upgradSlot2.Clear( );

		if ( upgradingCard != null )
		{
			upgradingCard.Amount += 2;

			// This will happen when the upgrade is successful but the source stack had only 2 cards in it
			// in this case we have to remove the stack
			if ( upgradingCard.Amount == 0 )
			{
				int sourceSlotIndex = collection.IndexOf( upgradingCard );
				collection[sourceSlotIndex] = null;
			}
		}
		upgradingCard = null;

		upgradeButton.interactable = false;

		if ( updateCollection )
		{
			bool forceDisable = !CanWeFillTheDeck( ) && !closing;
			DisplayCollection( forceDisable );
		}
	}

	private bool CanWeFillTheDeck( )
	{
		List<PlayerCard> allPlayerCards = collection.Concat( deckManager.GetDeck( ) ).ToList( );
		allPlayerCards = allPlayerCards.Where( c => c != null).ToList( );

		List<int> distinctCardGroups = allPlayerCards.Select( c => c.Card.GroupID ).Distinct( ).ToList( );

		int cardsToFillDeck = 0;
		foreach ( var group in distinctCardGroups )
		{
			int cardsInGroup = 0;
			foreach ( var card in allPlayerCards )
			{
				if ( card.Card.GroupID == group )
					cardsInGroup += card.Amount;
			}

			//Debug.Log( $"Group: {group}, count = {cardsInGroup}" );
			cardsToFillDeck += Mathf.Clamp( cardsInGroup, 0, PlayerCards.MaxIdenticalCardsInDeck );
		}

		//Debug.Log( $"cardsToFillDeck: {cardsToFillDeck}" );
		if ( cardsToFillDeck <= PlayerCards.MaxCardsInDeck )
			cantUpgradeTip.SetActive( true );
		else
			cantUpgradeTip.SetActive( false );

		return cardsToFillDeck > PlayerCards.MaxCardsInDeck;
	}

	private void UpgradeSlotDragEvent( int _, bool endOfDrag )
	{
		if ( endOfDrag )
		{
			int sourceSlotIndex = collection.IndexOf( upgradingCard );
			CleanUpUpgradeState( );

			slots[sourceSlotIndex].DoMove( Input.mousePosition );
		}
	}

	public void DraggedCardAddedToDeck( Vector2 position = default, PlayerCard cardToSwap = null ) // Order of parameters to force the supply of 'position' if someone supply 'cardToSwap'
	{
		int newSlotIndex = draggedSlotIndex;
		collection[draggedSlotIndex].Amount--;

		// We dragged card from the collection that we had only 1 of, so we now have an empty slot
		if ( collection[draggedSlotIndex].Amount <= 0 )
		{
			if ( cardToSwap != null )
				cardToSwap.Amount = 1; // Just to make sure

			collection[draggedSlotIndex] = cardToSwap;
		}
		// We dragged a card from the collection, swapped with the one in deck, but now we need to put the card from the deck in collection
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

		DisplayCollection( );

		if ( position != default )
			slots[newSlotIndex].DoMove( position );
	}

	public void Save( ) => playerCards.SetCollection( collection );

	public void AwardCard( PlayerCard cardToAdd )
	{
		AddCard( cardToAdd, Vector2.zero );
	}

	public void AddCard( PlayerCard cardToAdd, Vector2 position = default )
	{
		// Try to find a match
		for ( int i = 0; i < collection.Count; i++ )
		{
			if ( collection[i] != null && collection[i].Card.Name == cardToAdd.Card.Name ) // Found a match, add card
			{
				collection[i].Amount += 1;
				return;
			}
		}

		// Add to empty slot
		for ( int i = 0; i < collection.Count; i++ )
		{
			if ( collection[i] == null ) // Found a match, add card
			{
				collection[i] = new PlayerCard( )
				{
					Amount = 1,
					Card = cardToAdd.Card
				};

				return;
			}
		}

	}

	public void SortCollection( )
	{
		// "Condense" cards
		for ( int i = 0; i < collection.Count; i++ )
		{
			PlayerCard cardToCondense = collection[i];

			// Skip empty slots
			if ( cardToCondense == null )
				continue;

			// Look at all next cards for the same type
			for ( int j = i+1; j < collection.Count; j++ )
			{
				if ( collection[j] != null && collection[j].Card.Name == cardToCondense.Card.Name )
				{
					// Found a match, condense cards
					cardToCondense.Amount += collection[j].Amount;
					collection[j] = null;
				}
			}
		}

		// Sort cards
		collection = collection.OrderBy( card => { return card != null ? card.Card.CardID : 100000; } ).ToList( );

		// Refresh display
		DisplayCollection( );
	}

	private void CheckIfWeForceCanceled( )
	{
		if ( cardDragged == null || !Input.GetMouseButtonDown( 1 ) )
			return;

		slots[draggedSlotIndex].Canceled( );
		CardDragedEvent( draggedSlotIndex, true );
	}

	private void GetCollectionCards( )
	{
		playerCards.LoadPlayerCardsData( );
		List<PlayerCard> playerCollection = playerCards.GetCollection( );

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

	public void DisplayCollection( bool forceDisable = false )
	{
		cardDragged = null;
		cardDraggedFromDeck = null;

		for ( int i = 0; i < collection.Count; i++ )
			slots[i].Set( collection[i], i, CardDragedEvent, (int value)=> { CardDroppedEvent(value); if (AutoSort) SortCollection(); }, ClickedOnSlotEvent, upgrading, forceDisable );
	}

	private void ClickedOnSlotEvent( int dropSlotIndex )
	{
		// Card was clicked-drag (but not just empty slot)
		if ( cardDragged == null && cardDraggedFromDeck == null && slots[dropSlotIndex].Card != null )
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
		tooltip.text = "";
		PlayerCard cardInDestinationSlot = slots[dropSlotIndex].Card;
		onDisableOnDragging?.Invoke( true );

		// Dragging within Collection
		if ( cardDragged != null )
		{
			// Same card
			if ( cardDragged == cardInDestinationSlot )
			{
				tooltip.text = "Card placed in the same slot";
				DisplayCollection( );
				slots[dropSlotIndex].DoMove( slots[dropSlotIndex].CardPosition );
				CardDragedEvent( dropSlotIndex, true );

				return;
			}

			// Dropped in to empty slot
			if ( cardInDestinationSlot == null )
			{
				tooltip.text = "Card in collection moved";

				collection[dropSlotIndex] = cardDragged;
				collection[draggedSlotIndex] = null;

				DisplayCollection( );
				slots[dropSlotIndex].OnInfromation( );
				CardDragedEvent( dropSlotIndex, true );

				return;
			}

			// Combine piles cards (card of the same name dropped on one another)
			if ( cardDragged.Card.Name == cardInDestinationSlot.Card.Name )
			{
				tooltip.text = "Cards in collection stacked";

				cardInDestinationSlot.Amount += cardDragged.Amount;
				collection[draggedSlotIndex] = null;

				DisplayCollection( );
				slots[dropSlotIndex].OnInfromation( );
				CardDragedEvent( dropSlotIndex, true );

				return;
			}

			// Different types of cards
			tooltip.text = "Cards in collection swapped";

			collection[dropSlotIndex] = cardDragged;
			collection[draggedSlotIndex] = cardInDestinationSlot;

			DisplayCollection( );

			slots[draggedSlotIndex].DoMove( slots[dropSlotIndex].CardPosition );
			slots[dropSlotIndex].OnInfromation( );
			CardDragedEvent( dropSlotIndex, true );

			return;
		}

		// Dragging from Deck
		if ( cardDraggedFromDeck != null )
		{
			PlayerCard cardFromDeck = deckManager.GetDraggedCard( );

			// To empty slot
			if ( slots[dropSlotIndex].Card == null )
			{
				tooltip.text = "Card from deck put in to empty slot";

				collection[dropSlotIndex] = cardFromDeck;
				cardFromDeck.Amount = 1;
				deckManager.DraggedCardAddedToCollection( );

				DisplayCollection( );
				slots[dropSlotIndex].OnInfromation( );
				CardDragedEvent( dropSlotIndex, true );

				return;
			}

			// To same type of card
			if ( collection[dropSlotIndex].Card.Name == cardFromDeck.Card.Name )
			{
				tooltip.text = "Merged card from deck in to collection";

				collection[dropSlotIndex].Amount++;
				deckManager.DraggedCardAddedToCollection( );

				DisplayCollection( );
				slots[dropSlotIndex].OnInfromation( );
				CardDragedEvent( dropSlotIndex, true );

				return;
			}

			// To different type of card
			// collection[dropSlotIndex].Card.Name != cardFromDeck.Card.Name
			PlayerCard cardToSwap = collection[dropSlotIndex];

			if ( deckManager.WillWeExceedSameCardLimit( cardToSwap, cardFromDeck ) )
				return;

			tooltip.text = "Swapped card from deck -> collection";

			cardFromDeck.Amount = 1;
			draggedSlotIndex = dropSlotIndex;

			deckManager.DraggedCardAddedToCollection( slots[dropSlotIndex].CardPosition, cardToSwap );
			DraggedCardAddedToDeck( Vector2.zero, cardFromDeck );

			slots[dropSlotIndex].OnInfromation( );
			CardDragedEvent( dropSlotIndex, true );
		}
	}

	private void CardDragedEvent( int index, bool endOfDrag )
	{
		if ( endOfDrag )
		{
			cardDragged = null;
			deckManager.SetDraggedCard( cardDragged );
		}
		else
		{
			cardDragged = slots[index].Card;
			deckManager.SetDraggedCard( cardDragged );
			tooltip.text = "Place card in an empty slot or swap with another one";
		}

		onDisableOnDragging?.Invoke( endOfDrag );
		draggedSlotIndex = endOfDrag ? int.MinValue : index; // Index od the dragged card or "null"
	}
}
