/**
 * Description: Manages the deck.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DeckBuilder : MonoBehaviour
{
	public static DeckBuilder Instance { get; private set; }

	[Header("Objects")]
	[SerializeField] private PlayerCards playerCards = null;
	[SerializeField] private Animator animator = null;
	[SerializeField] private Button combineButton = null;
	[SerializeField] private TextMeshProUGUI tooltip = null;
	[SerializeField] private GameObject[] toHideOnClose = null;
	[SerializeField] private GameObject[] toShowOnClose = null;

	[Header("Collection")]
	[SerializeField] private CardSlot[] collectionSlots = null;

	[Header("Deck")]
	[SerializeField] private CardSlot[] deckSlots = null;
	[SerializeField] private int maxIdenticalDeckCards = 3;

	[Header("Upgrade")]
	[SerializeField] private CardSlot[] upgradeSlots = null;

	public GameObject selectedSlot = null;
	public GameObject otherSlot = null;

	private bool upgrading = false;

	private void Awake( )
	{
		if ( Instance != null && Instance != this )
			Destroy( this );
		else
			Instance = this;
	}

	private void OnDestroy( ) { if ( this == Instance ) { Instance = null; } }

	void Start( )
	{
		Assert.IsNotNull( playerCards, $"Please assign <b>{nameof( playerCards )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( combineButton, $"Please assign <b>{nameof( combineButton )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( tooltip, $"Please assign <b>{nameof( tooltip )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( animator, $"Please assign <b>{nameof( animator )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		Assert.AreNotEqual( toHideOnClose.Length, 0, $"Please assign <b>{nameof( toHideOnClose )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.AreNotEqual( toShowOnClose.Length, 0, $"Please assign <b>{nameof( toShowOnClose )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		Assert.AreNotEqual( collectionSlots.Length, 0, $"Please assign <b>{nameof( collectionSlots )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.AreEqual( deckSlots.Length, PlayerCards.MaxCardsInDeck, $"Please make sure <b>{nameof( deckSlots )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object has {PlayerCards.MaxCardsInDeck} elements (number of cards a Deck should have)" );

		Assert.AreEqual( upgradeSlots.Length, 2, $"Please make sure <b>{nameof( upgradeSlots )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object has 2 elements (number of cards needed for an upgrade)" );

		OpenAndLoad( );
	}

	public void OpenAndLoad( )
	{
		foreach ( var go in toHideOnClose )
			go.SetActive( true );

		foreach ( var go in toShowOnClose )
			go.SetActive( false );

		LoadCollectionAndDeck( );
		UpdateCollection( );
		UpdateDeck( );
	}

	public void Close( )
	{
		animator.SetTrigger( "Close" );

		foreach ( var go in toShowOnClose )
			go.SetActive( true );
	}

	public void OnDoneClose( )
	{
		foreach ( var go in toHideOnClose )
			go.SetActive( false );
	}

	public void CloseAndSave( )
	{
		SaveCollectionAndDeck( );
		Close( );
	}

	public void ShowUpgrade( )
	{
		upgrading = true;
		foreach ( var slot in upgradeSlots )
			slot.Clear( );

		animator.SetTrigger( "ShowUpgrade" );

		combineButton.interactable = false;
		UpdateCollection( );
	}

	public void HideUpgrade( )
	{
		animator.SetTrigger( "HideUpgrade" );

		upgrading = false;
		UpdateCollection( );
	}

	public Card SelectedCard( )
	{
		/*if ( selectedSlot != null )
			return GetCardFromSlot( selectedSlot );
		*/
		return null;
	}

	public Card GetCardFromSlot( GameObject slot )
	{
		/*if ( slot != null && slot.transform.GetChild( 3 ) != null && slot.transform.GetChild( 3 ).GetChild( 0 ) != null && slot.transform.GetChild( 3 ).GetChild( 0 ).GetComponent<Card>( ) != null )
			return slot.transform.GetChild( 3 ).GetChild( 0 ).GetComponent<Card>( );
		*/
		return null;
	}

	public void UpgradeCard( )
	{
		// De-select
		if ( !selectedSlot )
			return;

		Card cardToUpgrade = SelectedCard( ).SelectionMode == CardSelectionMode.InCollection ? SelectedCard( ) : null;
		//CardClicked( selectedCollectionCard, CardSelectionMode.InCollection );

		// Find selected card in master collection
		PlayerCard lowerCardVersion = playerCards.GetCollection.First( card => card.Card.Name == cardToUpgrade.Name );
		// Find it's upgraded version
		PlayerCard higherCardVersion = playerCards.GetCollection.First( card => card.Card.Name == cardToUpgrade.HigherLevelVersion.Name );
		// Take 2 source cards...
		lowerCardVersion.Amount -= 2;
		// ...and turn them in to a better one
		higherCardVersion.Amount += 1;

		UpdateCollection( );
	}

	public Card[] GetPlayerDeck( ) => playerCards.GetDeck.Select( card => card.Card ).ToArray( );

	public void CheckCollectionCardSelection( Card card )
	{
		if ( card.SelectionMode == CardSelectionMode.InCollection )
		{
			if ( !selectedSlot )
				selectedSlot = card.transform.parent.parent.gameObject;
			else if ( !otherSlot )
				otherSlot = card.transform.parent.parent.gameObject;
		}
	}

	public void CheckDeckCardSelection( Card card )
	{
		if ( card.SelectionMode == CardSelectionMode.InDeck )
		{
			if ( !selectedSlot )
				selectedSlot = card.transform.parent.parent.gameObject;
			else if ( !otherSlot )
				otherSlot = card.transform.parent.parent.gameObject;
		}
	}

	public bool IsCollectionCardSelected( )
	{
		/*return ( selectedSlot && SelectedCard( )?.SelectionMode == CardSelectionMode.InCollection )
		|| ( otherSlot && GetCardFromSlot( otherSlot )?.SelectionMode == CardSelectionMode.InCollection );*/
		return false;
	}

	public bool IsDeckCardSelected( )
	{
		/*return ( selectedSlot && SelectedCard( ).SelectionMode == CardSelectionMode.InDeck )
		|| ( otherSlot && GetCardFromSlot( otherSlot ).SelectionMode == CardSelectionMode.InDeck );*/
		return false;
	}

	public void CompareAndRemoveSelection( Card card )
	{
		if ( selectedSlot && SelectedCard( ) == card )
		{
			selectedSlot = null;
			otherSlot = null;
		}
		else if ( otherSlot && GetCardFromSlot( otherSlot ) == card )
			otherSlot = null;
	}

	/*
	  public void CardClicked( Card card, CardSelectionMode mode )
	  {
		  // Clicked card in the Collection
		  if ( mode == CardSelectionMode.InCollection )
		  {
			  if ( selectedCollectionCard ) // Unselecting the last card
			  {
				  selectedCollectionCard.CardSelected( false );
				  selectedCollectionCard.transform.parent.parent.GetComponent<CardSlot>( ).Select( false );

				  if ( upgrading )
				  {
					  foreach ( var slot in upgradeSlots )
						  slot.SetEmpty( );

					  combineButton.interactable = false;
				  }
			  }

			  if ( selectedCollectionCard != card ) // Selected another card
			  {
				  selectedCollectionCard = card;
				  selectedCollectionCard.CardSelected( true );
				  selectedCollectionCard.transform.parent.parent.GetComponent<CardSlot>( ).Select( true );
				  tooltip.text = "Select card from Deck to swap";

				  if ( upgrading )
				  {
					  foreach ( var slot in upgradeSlots )
						  slot.Set( selectedCollectionCard.gameObject, 1 );

					  combineButton.interactable = true;
				  }
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
	  */

	public void MoveSlot( )
	{
		if ( !selectedSlot || !otherSlot )
			return;

		Card selectedCollectionCard = null;
		Card selectedDeckCard = null;
		Card selectedUpgradeCard = null;

		if ( SelectedCard( ).SelectionMode == CardSelectionMode.InCollection )
			selectedCollectionCard = SelectedCard( );
		else if ( SelectedCard( ).SelectionMode == CardSelectionMode.InDeck )
			selectedDeckCard = SelectedCard( );
		else if ( SelectedCard( ).SelectionMode == CardSelectionMode.InUpgrade )
			selectedUpgradeCard = SelectedCard( );

		if ( otherSlot.transform.GetChild( 3 ).childCount >= 1 )
		{
			if ( GetCardFromSlot( otherSlot ).SelectionMode == CardSelectionMode.InCollection )
				selectedCollectionCard = GetCardFromSlot( otherSlot );
			else if ( GetCardFromSlot( otherSlot ).SelectionMode == CardSelectionMode.InDeck )
				selectedDeckCard = GetCardFromSlot( otherSlot );
			else if ( GetCardFromSlot( otherSlot ).SelectionMode == CardSelectionMode.InUpgrade )
				selectedUpgradeCard = GetCardFromSlot( otherSlot );
		}

		if ( selectedCollectionCard != null && selectedDeckCard != null )
		{
			// Count the number of the same cards (even if they are of different levels)
			int sameCardsInDeck = 0;
			foreach ( var card in playerCards.GetDeck )
			{
				// Same level cards
				if ( card.Card.Name == selectedCollectionCard.Name )
					sameCardsInDeck++;

				// Level 2 or Level 3 card vs. lower level
				if ( card.Card.LowerLevelVersion && card.Card.LowerLevelVersion.Name == selectedCollectionCard.Name )
					sameCardsInDeck++;

				// Level 3 card vs. Level 1
				if ( card.Card.LowerLevelVersion && card.Card.LowerLevelVersion.LowerLevelVersion && card.Card.LowerLevelVersion.LowerLevelVersion.Name == selectedCollectionCard.Name )
					sameCardsInDeck++;

				// Level 1 and Level 2 card vs. higher level
				if ( card.Card.HigherLevelVersion && card.Card.HigherLevelVersion.Name == selectedCollectionCard.Name )
					sameCardsInDeck++;

				// Level 1 card vs. Level 3
				if ( card.Card.HigherLevelVersion && card.Card.HigherLevelVersion.HigherLevelVersion && card.Card.HigherLevelVersion.HigherLevelVersion.Name == selectedCollectionCard.Name )
					sameCardsInDeck++;
			}

			// Tried to add more then max identical Cards to the Deck
			if ( sameCardsInDeck >= maxIdenticalDeckCards && selectedCollectionCard.Name != selectedDeckCard.Name )
			{
				tooltip.text = "Can't have more then 3 identical cards in Deck";

				selectedDeckCard.CardSelected( false );
				selectedDeckCard.transform.parent.parent.GetComponent<CardSlot>( ).Select( false );
				selectedDeckCard = null;

				return;
			}

			// -- Swap cards --

			// Remove card selected in collection from the master collection (by reducing the amount we have)
			PlayerCard cardInCollectionToSwap = playerCards.GetCollection.First( card => card.Card.Name == selectedCollectionCard.Name );
			cardInCollectionToSwap.Amount--;

			// Remove card selected in the deck from the master deck
			PlayerCard cardInDeckToSwap = playerCards.GetDeck.First( card => card.Card.Name == selectedDeckCard.Name );
			cardInDeckToSwap.Amount--;

			// Add card selected from the collection to master deck
			cardInCollectionToSwap.Amount++;

			// Add card selected from the deck to master collection
			cardInDeckToSwap.Amount++;

			tooltip.text = "Cards swapped";

			// -- Cards swapped --

			selectedCollectionCard.CardSelected( false );
			selectedDeckCard.CardSelected( false );
		}
		else if ( selectedCollectionCard != null && selectedUpgradeCard != null )
		{
			// Tried to add more then max identical Cards to the Deck
			int sameCardsInDeck = playerCards.GetDeck.Count( card => card.Card.Name == selectedCollectionCard.Name );
			if ( sameCardsInDeck >= maxIdenticalDeckCards && selectedCollectionCard.Name != selectedUpgradeCard.Name )
			{
				tooltip.text = "Can't have more then 3 identical cards in Deck";

				selectedUpgradeCard.CardSelected( false );
				selectedUpgradeCard.transform.parent.parent.GetComponent<CardSlot>( ).Select( false );
				selectedUpgradeCard = null;

				return;
			}

			// -- Set Display for Upgrade Card Slot --

			// Remove card selected in collection from the master collection (by reducing the amount we have)
			PlayerCard cardInCollectionToSwap = playerCards.GetCollection.First( card => card.Card.Name == selectedCollectionCard.Name );

			//cardInCollectionToSwap.AmountPlayerOwns--; //Only SHOW in upgrade

			// Remove card selected in the upgrade
			PlayerCard cardInUpgradeToSwap = playerCards.GetCollection.First( card => card.Card.Name == selectedUpgradeCard.Name );
			//cardInUpgradeToSwap.AmountInDeck--; // MISSING How can I remove this card from Upgrade?

			//selectedUpgradeCard.transform.parent.parent.GetComponent<CardSlot>().SetEmpty(); //Only SHOW in upgrade

			// Add card selected from the collection to upgrade
			//cardInCollectionToSwap.AmountInDeck++; // MISSING How can I add this card to Upgrade?
			if ( otherSlot.GetComponent<CardSlot>( ).IsEmpty( ) )
				otherSlot.GetComponent<CardSlot>( ).Set( selectedCollectionCard.gameObject, 1, 0, ( x, y ) => { }, x => { } );

			// Add card selected from the upgrade to master collection
			//cardInUpgradeToSwap.AmountPlayerOwns++; // Only SHOW in upgrade

			tooltip.text = "Cards swapped";

			// -- Cards swapped --

			selectedCollectionCard.CardSelected( false );
			selectedUpgradeCard.CardSelected( false );
		}
		else if ( selectedCollectionCard != null
			|| selectedDeckCard != null
			|| selectedUpgradeCard != null )
		//for adding cards to empty slots
		{
			Card selectedCard = null;

			if ( selectedCollectionCard != null )
				selectedCard = selectedCollectionCard;
			else if ( selectedDeckCard != null )
				selectedCard = selectedDeckCard;
			else if ( selectedUpgradeCard != null )
				selectedCard = selectedUpgradeCard;

			// Remove card selected in collection from the master collection (by reducing the amount we have)
			PlayerCard addedCard = playerCards.GetCollection.First( card => card.Card.Name == selectedCard.Name );

			if ( selectedCard == selectedCollectionCard )
			{
				if ( otherSlot.name.Contains( "Deck" ) )
				{
					//addedCard.DefaultPlayerOwned--;
					//addedCard.DefaultInDeck++;
				}
				else if ( otherSlot.name.Contains( "Upgrade" ) )
				{
					otherSlot.GetComponent<CardSlot>( ).Set( selectedCard.gameObject, 1, 0, ( x, y ) => { }, x => { } );
				}
			}
			else if ( selectedCard == selectedDeckCard )
			{
				//addedCard.DefaultInDeck--;

				//if ( otherSlot.name.Contains( "Collection" ) )
				//addedCard.DefaultPlayerOwned++;

				//Deck card can not access upgrade
			}
			else if ( selectedCard == selectedUpgradeCard )
			{
				//addedCard.AmountInDeck--; //MISSING How can I remove this card from Upgrade?
				selectedUpgradeCard.transform.parent.parent.GetComponent<CardSlot>( ).Clear( );

				//if ( otherSlot.name.Contains( "Collection" ) )
				//addedCard.DefaultPlayerOwned++;

				//Upgrade card can not access deck
			}

			tooltip.text = "Card Moved";

			selectedCard.CardSelected( false );
		}

		selectedSlot.GetComponent<CardSlot>( ).Select( false );
		otherSlot.GetComponent<CardSlot>( ).Select( false );

		selectedCollectionCard = null;
		selectedDeckCard = null;
		selectedUpgradeCard = null;

		selectedSlot = null;
		otherSlot = null;

		// Update changes
		UpdateCollection( );
		UpdateDeck( );
	}

	private void UpdateCollection( )
	{
		/*int minAmount = upgrading ? 1 : 0; // We need more then 1 cards for upgrading and more then 0 for deck building

		// Find all the cards we have or, if we are upgrading, all the cards we can combine
		Card[] cards = playerCards.GetCollection.Where( card => card.Amount > minAmount && !( upgrading && card.Card.Level == CardLevel.Level3 ) ).Select( card => card.Card ).ToArray( );
		int[] amounts = playerCards.GetCollection.Where( card => card.Amount > minAmount && !( upgrading && card.Card.Level == CardLevel.Level3 ) ).Select( card => card.Amount ).ToArray( );

		playerCards.GetCollection.Clear( );
		for ( int i = 0; i < cards.Length; i++ )
		{
			playerCards.GetCollection.Add( new PlayerCard( )
			{
				Card = cards[i],
				Amount = amounts[i]
			} );
		}*/

		///playerCards.Collection = playerCards.Collection.OrderBy( card => card.Card.name ).ToList( );

		/*foreach ( var slot in collectionSlots )
			slot.SetEmpty( );

		for ( int i = 0; i < playerCards.GetCollection.Count; i++ )
			playerCards.GetCollection[i].Card = collectionSlots[i].Set( playerCards.GetCollection[i].Card.gameObject, playerCards.GetCollection[i].Amount );*/
	}

	private void UpdateDeck( )
	{
		// Find all the cards in master collection that we have in deck
		/*List<PlayerCard> cards = new List<PlayerCard>( );
		foreach ( var card in playerCards.GetCollection )
		{
			for ( int i = 0; i < card.Amount; i++ )
			{
				cards.Add( new PlayerCard( ) { Card = card.Card, Amount = card.Amount } );
			}
		}

		///playerCards.Collection = cards;
		///playerCards.Deck = playerCards.GetDeck.OrderBy( card => card.Card.Name ).ToList( );

		for ( int i = 0; i < playerCards.GetDeck.Count; i++ )
			deckSlots[i].Set( playerCards.GetDeck[i].Card.gameObject, 1 );*/
	}

	private void LoadCollectionAndDeck( ) => playerCards.LoadPlayerCardsData( );

	private void SaveCollectionAndDeck( ) => playerCards.SavePlayerCardsData( );
}
