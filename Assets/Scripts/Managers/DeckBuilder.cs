/**
 * Description: Manages the deck.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class CardInCollection
{
	public Card Card;
	public int Amount;
}

public class DeckBuilder : MonoBehaviour
{
	public static DeckBuilder Instance { get; private set; }

	[Header("Objects")]
	[SerializeField] private GameObject[] toHideOnClose = null;
	[SerializeField] private GameObject[] toShowOnClose = null;
	[SerializeField] private CardSlot[] collectionSlots = null;
	[SerializeField] private CardSlot[] deckSlots = null;
	[SerializeField] private TextMeshProUGUI tooltip = null;

	[Header("Collection")]
	[SerializeField] private CardInCollection[] cardsInCollection = null;

	[Header("Deck")]
	[SerializeField] private Card[] cardsInDeck = null;

	private Card selectedCollectionCard = null;
	private Card selectedDeckCard = null;

	private void Awake( )
	{
		if ( Instance != null && Instance != this )
			Destroy( this );
		else
			Instance = this;
	}

	private void OnDestroy( ) { if ( this == Instance ) { Instance = null; } }

	void Start ()
	{
		Assert.IsNotNull( tooltip, $"Please assign <b>{nameof( tooltip )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		UpdateCollection( );
		UpdateDeck( );
		Open( );
	}

	public void Open( )
	{
		foreach ( var go in toHideOnClose )
			go.SetActive( true );

		foreach ( var go in toShowOnClose )
			go.SetActive( false );
	}

	public void Close( )
	{
		foreach ( var go in toHideOnClose )
			go.SetActive( false );

		foreach ( var go in toShowOnClose )
			go.SetActive( true );
	}

	public void CardClicked( Card card, CardSelectionMode mode )
	{
		// Clicked card in the Collection
		if ( mode == CardSelectionMode.InCollection )
		{
			if ( selectedCollectionCard ) // Unselecting the last card
			{
				selectedCollectionCard.CardSelected( false );
				selectedCollectionCard.transform.parent.parent.GetComponent<CardSlot>( ).Select( false );
			}

			if ( selectedCollectionCard != card ) // Selected another card
			{
				selectedCollectionCard = card;
				selectedCollectionCard.CardSelected( true );
				selectedCollectionCard.transform.parent.parent.GetComponent<CardSlot>( ).Select( true );
				tooltip.text = "Select card from Deck to swap";
			}
			else // Last card was unselected
			{
				selectedCollectionCard = null;
			}
		}
		else if ( mode == CardSelectionMode.InDeck )
		{
			if ( selectedDeckCard ) // Unselecting the last card
			{
				selectedDeckCard.CardSelected( false );
				selectedDeckCard.transform.parent.parent.GetComponent<CardSlot>( ).Select( false );
			}

			if ( selectedDeckCard != card ) // Selected another card
			{
				selectedDeckCard = card;
				selectedDeckCard.CardSelected( true );
				selectedDeckCard.transform.parent.parent.GetComponent<CardSlot>( ).Select( true );
				tooltip.text = "Select card from Collection to swap";
			}
			else // Last card was unselected
			{
				selectedDeckCard = null;
			}
		}

		if ( selectedCollectionCard && selectedDeckCard )
			TrySwapCards( );
	}

	private void TrySwapCards( )
	{
		// Unselect both cards
		selectedCollectionCard.CardSelected( false );
		selectedCollectionCard.transform.parent.parent.GetComponent<CardSlot>( ).Select( false );
		selectedDeckCard.CardSelected( false );
		selectedDeckCard.transform.parent.parent.GetComponent<CardSlot>( ).Select( false );

		// Swap cards
		tooltip.text = "Cards swapped";

		// Cards fully unselected
		selectedCollectionCard = null;
		selectedDeckCard = null;
	}

	private void UpdateCollection( )
	{
		cardsInCollection = cardsInCollection.OrderByDescending( card => card.Amount ).ToArray( );

		foreach ( var slot in collectionSlots )
			slot.SetEmpty( );

		for ( int i = 0; i < cardsInCollection.Length; i++ )
		{
			if ( cardsInCollection[i].Amount > 0 )
			{
				collectionSlots[i].Set( cardsInCollection[i].Card.gameObject, cardsInCollection[i].Amount );
			}
		}
	}

	private void UpdateDeck( )
	{
		cardsInDeck = cardsInDeck.OrderBy( card => card.name ).ToArray( );

		for ( int i = 0; i < cardsInDeck.Length; i++ )
			deckSlots[i].Set( cardsInDeck[i].gameObject, 1 );
	}
}
