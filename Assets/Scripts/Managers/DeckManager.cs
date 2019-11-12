/**
 * Description: Managed and displays player deck (in deck builder).
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DeckManager : MonoBehaviour
{
	[Header("External Objects")]
	[SerializeField] private PlayerCards playerCards = null;

	[Header("Objects")]
	[SerializeField] private GameObject deckSlot = null;
	[SerializeField] private Transform slotsParent = null;

	private List<CardSlot> slots = new List<CardSlot>();
	private List<PlayerCard> deck = new List<PlayerCard>();
	private int slotNumber;
	private int draggedIndex = int.MinValue;

	void Start( )
	{
		Assert.IsNotNull( playerCards, $"Please assign <b>{nameof( playerCards )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( deckSlot, $"Please assign <b>{nameof( deckSlot )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( slotsParent, $"Please assign <b>{nameof( slotsParent )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		slotNumber = PlayerCards.MaxCardsInDeck;

		GetDeckCards( );
		CreateLayout( );
		DisplayDeck( );
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
		for ( int i = 0; i < deck.Count; i++ ) // Add cards
		{
			if ( deck[i] != null ) // Non-empty slot
				slots[i].Set( deck[i].Card.gameObject, deck[i].Amount, i, CardDragedEvent, CardDroppedEvent );
			else // Empty slot
				slots[i].Set( null, 0, i, CardDragedEvent, CardDroppedEvent );
		}
	}

	private void CardDroppedEvent( int index )
	{
		Debug.Log( $"Deck dropped: {index}" );
	}

	private void CardDragedEvent( int index, bool endOfDrag )
	{
		//string s = endOfDrag ? "stop" : "start";
		//Debug.Log( $"Collection dragged: {s} {index}" );
		draggedIndex = endOfDrag ? int.MinValue : index; // Index od the dragged card or "null"
	}
}
