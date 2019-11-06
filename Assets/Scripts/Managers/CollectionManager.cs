/**
 * Description: Manages and displays players card collection.
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
	private List<PlayerCard> collection;

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
		collection = playerCards.GetCollection;
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
		// Clear slots
		foreach ( var slot in slots )
			slot.SetEmpty( );
		Debug.Log( collection.Count );
		// Add cards
		for ( int i = 0; i < collection.Count; i++ )
		{
			if ( collection[i].Card )
				slots[i].Set( collection[i].Card.gameObject, collection[i].Amount );
		}
	}
}
