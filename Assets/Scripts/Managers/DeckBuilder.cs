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
	[SerializeField] private int maxIdenticalDeckCards = 3;

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
		// Tried to add more then max identical Cards to the Deck
		int sameCardsInDeck = cardsInDeck.Count( card => card.Name == selectedCollectionCard.Name );
		if ( sameCardsInDeck >= maxIdenticalDeckCards &&  selectedCollectionCard.Name != selectedDeckCard.Name )
		{
			tooltip.text = "Can't have more then 3 identical cards in Deck";

			selectedDeckCard.CardSelected( false );
			selectedDeckCard.transform.parent.parent.GetComponent<CardSlot>( ).Select( false );
			selectedDeckCard = null;

			return;
		}

		// -- Swap cards --

		// Remove card selected in collection from the collection (by reducing the amount we have)
		CardInCollection cardInCollectionToSwap = cardsInCollection.First( card => card.Card.Name == selectedCollectionCard.Name );
		cardInCollectionToSwap.Amount--;

		// Remove card selected in the deck from the deck
		Card cardInDeckToSwap = selectedDeckCard;
		int indexInDeckToSwap = System.Array.IndexOf( cardsInDeck, cardInDeckToSwap );
		cardsInDeck[indexInDeckToSwap] = null; // Not strictly necessary

		// Add card selected from the deck to collection
		CardInCollection cardInCollectionToAddTo = cardsInCollection.First( card => card.Card.Name == selectedDeckCard.Name );
		cardInCollectionToAddTo.Amount++;

		// Add (swap) card selected from the collection to deck
		Card cardInDeckToAdd = selectedCollectionCard;
		cardsInDeck[indexInDeckToSwap] = cardInDeckToAdd;

		// -- Cards swapped --

		// Unselect both cards
		selectedCollectionCard.CardSelected( false );
		selectedCollectionCard.transform.parent.parent.GetComponent<CardSlot>( ).Select( false );
		selectedDeckCard.CardSelected( false );
		selectedDeckCard.transform.parent.parent.GetComponent<CardSlot>( ).Select( false );
		selectedCollectionCard = null;
		selectedDeckCard = null;

		tooltip.text = "Cards swapped";

		// Update changes
		UpdateCollection( );
		UpdateDeck( );
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
				cardsInCollection[i].Card = collectionSlots[i].Set( cardsInCollection[i].Card.gameObject, cardsInCollection[i].Amount );
			}
		}
	}

	private void UpdateDeck( )
	{
		cardsInDeck = cardsInDeck.OrderBy( card => card.name ).ToArray( );

		for ( int i = 0; i < cardsInDeck.Length; i++ )
			cardsInDeck[i] = deckSlots[i].Set( cardsInDeck[i].gameObject, 1 );
	}
}
