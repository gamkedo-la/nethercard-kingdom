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

	[Header("Objects")]
	[SerializeField] private GameObject collectionSlot = null;
	[SerializeField] private Transform slotsParent = null;

	[Header("Parameters")]
	[SerializeField] private int minSlots = 20;

	private List<CardSlot> slots = new List<CardSlot>();
	private List<PlayerCard> collection = new List<PlayerCard>();
	private int draggedIndex = int.MinValue;

	void Start ()
	{
		Assert.IsNotNull( playerCards, $"Please assign <b>{nameof( playerCards )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( collectionSlot, $"Please assign <b>{nameof( collectionSlot )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( slotsParent, $"Please assign <b>{nameof( slotsParent )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		GetCollectionCards( );
		CreateLayout( );
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
		for ( int i = 0; i < collection.Count; i++ ) // Add cards
		{
			if ( collection[i] != null ) // Non-empty slot
				slots[i].Set( collection[i].Card.gameObject, collection[i].Amount, i, CardDragedEvent, CardDroppedEvent );
			else // Empty slot
				slots[i].Set( null, 0, i, CardDragedEvent, CardDroppedEvent );
		}
	}

	private void CardDroppedEvent( int index )
	{
		Debug.Log( $"Collection dropped: {index}" );
	}

	private void CardDragedEvent( int index, bool endOfDrag )
	{
		//string s = endOfDrag ? "stop" : "start";
		//Debug.Log( $"Collection dragged: {s} {index}" );
		draggedIndex = endOfDrag ? int.MinValue : index; // Index od the dragged card or "null"
	}
}
