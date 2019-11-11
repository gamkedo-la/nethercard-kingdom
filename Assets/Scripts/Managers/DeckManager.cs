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
	private List<PlayerCard> deck;
	private int slotNumber;

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
		deck = playerCards.GetDeck;
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
		// Clear all slots
		foreach ( var slot in slots )
			slot.SetEmpty( );

		// Add cards
		for ( int i = 0; i < deck.Count; i++ )
		{
			if ( deck[i].Card ) // Non-empty position
				slots[i].Set( deck[i].Card.gameObject, deck[i].Amount );
		}
	}
}
