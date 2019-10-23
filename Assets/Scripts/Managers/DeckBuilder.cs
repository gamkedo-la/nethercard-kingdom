/**
 * Description: Manages the deck.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[System.Serializable]
public class CardsData
{
	public string[] Name;
	public int[] AmountPlayerOwns;
	public int[] AmountInDeck;
}

[System.Serializable]
public class CardInMasterCollection
{
	public Card Card;
	public int AmountPlayerOwns;
	public int AmountInDeck;
}

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
	[SerializeField] private Button combineButton = null;
	[SerializeField] private GameObject[] toHideOnClose = null;
	[SerializeField] private GameObject[] toShowOnClose = null;
	[SerializeField] private TextMeshProUGUI tooltip = null;

	[Header("Master Collection")]
	[SerializeField] private CardInMasterCollection[] allPlayerCards = null;

	[Header("Collection")]
	[SerializeField] private CardSlot[] collectionSlots = null;
	private CardInCollection[] cardsInCollection = null;

	[Header("Deck")]
	[SerializeField] private CardSlot[] deckSlots = null;
	private Card[] cardsInDeck = null;
	[SerializeField] private int maxIdenticalDeckCards = 3;

	[Header("Upgrade")]
	[SerializeField] private CardSlot[] upgradeSlots = null;
	[SerializeField] private GameObject[] toShowOnUpgrade = null;
	[SerializeField] private GameObject[] toHideOnUpgrade = null;

	private Card selectedCollectionCard = null;
	private Card selectedDeckCard = null;
	private bool upgrading = false;

	const int CardsInDeck = 10;
	const int IdenticalCardsInDeckMax = 3;

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
		Assert.IsNotNull( combineButton, $"Please assign <b>{nameof( combineButton )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( tooltip, $"Please assign <b>{nameof( tooltip )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		Assert.AreNotEqual( toHideOnClose.Length, 0, $"Please assign <b>{nameof( toHideOnClose )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.AreNotEqual( toShowOnClose.Length, 0, $"Please assign <b>{nameof( toShowOnClose )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.AreNotEqual( allPlayerCards.Length, 0, $"Please assign <b>{nameof( allPlayerCards )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		int cardsInDeck = 0;
		foreach ( var card in allPlayerCards )
			cardsInDeck += card.AmountInDeck;
		Assert.AreEqual( cardsInDeck, CardsInDeck, $"<b>{nameof( allPlayerCards )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object has to have <b>exactly</b> {CardsInDeck} cards in deck" );

		Assert.AreNotEqual( collectionSlots.Length, 0, $"Please assign <b>{nameof( collectionSlots )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		//Assert.AreNotEqual( cardsInCollection.Length, 0, $"Please assign <b>{nameof( cardsInCollection )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		Assert.AreEqual( deckSlots.Length, CardsInDeck, $"Please make sure <b>{nameof( deckSlots )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object has 10 elements (number of cards a Deck should have)" );
		//Assert.AreEqual( cardsInDeck.Length, 10, $"Please make sure <b>{nameof( cardsInDeck )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object has 10 elements (number of cards a Deck should have)" );

		Assert.AreEqual( upgradeSlots.Length, 2, $"Please make sure <b>{nameof( upgradeSlots )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object has 2 elements (number of cards needed for an upgrade)" );
		Assert.AreNotEqual( toShowOnUpgrade.Length, 0, $"Please assign <b>{nameof( toShowOnUpgrade )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.AreNotEqual( toHideOnUpgrade.Length, 0, $"Please assign <b>{nameof( toHideOnUpgrade )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

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
		foreach ( var go in toHideOnClose )
			go.SetActive( false );

		foreach ( var go in toShowOnClose )
			go.SetActive( true );
	}

	public void CloseAndSave( )
	{
		SaveCollectionAndDeck( );
		Close( );
	}

	public void ShowUpgrade( )
	{
		foreach ( var go in toHideOnUpgrade )
			go.SetActive( false );

		foreach ( var go in toShowOnUpgrade )
			go.SetActive( true );

		upgrading = true;
		foreach ( var slot in upgradeSlots )
			slot.SetEmpty( );

		combineButton.interactable = false;
		UpdateCollection( );
	}

	public void HideUpgrade( )
	{
		foreach ( var go in toHideOnUpgrade )
			go.SetActive( true );

		foreach ( var go in toShowOnUpgrade )
			go.SetActive( false );

		upgrading = false;
		UpdateCollection( );
	}

	public void UpgradeCard( )
	{
		// De-select
		Card cardToUpgrade = selectedCollectionCard;
		CardClicked( selectedCollectionCard, CardSelectionMode.InCollection );

		// Find selected card in master collection
		CardInMasterCollection lowerCardVersion = allPlayerCards.First( card => card.Card.Name == cardToUpgrade.Name );
		// Find it's upgraded version
		CardInMasterCollection higherCardVersion = allPlayerCards.First( card => card.Card.Name == cardToUpgrade.HigherLevelVersion.Name );
		// Take 2 source cards...
		lowerCardVersion.AmountPlayerOwns -= 2;
		// ...and turn them in to a better one
		higherCardVersion.AmountPlayerOwns += 1;

		UpdateCollection( );
	}

	public Card[] GetPlayerDeck( ) => cardsInDeck;

	public void CheckCollectionCardSelection( Card card ) { if(card.SelectionMode == CardSelectionMode.InCollection) selectedCollectionCard = card; }
	public void CheckDeckCardSelection( Card card ) { if(card.SelectionMode == CardSelectionMode.InDeck) selectedDeckCard = card; }
	public bool SwapCards() { if(selectedCollectionCard && selectedDeckCard) { TrySwapCards(); return true; } return false; }
	public bool IsCollectionCardSelected() { return selectedCollectionCard; }
	public bool IsDeckCardSelected() { return selectedDeckCard; }
	public void CompareAndRemoveSelection( Card card ) { if(selectedCollectionCard == card) selectedCollectionCard = null; else if(selectedDeckCard == card) selectedDeckCard = null; }

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

		// Remove card selected in collection from the master collection (by reducing the amount we have)
		CardInMasterCollection cardInCollectionToSwap = allPlayerCards.First( card => card.Card.Name == selectedCollectionCard.Name );
		cardInCollectionToSwap.AmountPlayerOwns--;

		// Remove card selected in the deck from the master deck
		CardInMasterCollection cardInDeckToSwap = allPlayerCards.First( card => card.Card.Name == selectedDeckCard.Name );
		cardInDeckToSwap.AmountInDeck--;

		// Add card selected from the collection to master deck
		cardInCollectionToSwap.AmountInDeck++;

		// Add card selected from the deck to master collection
		cardInDeckToSwap.AmountPlayerOwns++;

		tooltip.text = "Cards swapped";

		// -- Cards swapped --

		// Unselect both cards
		selectedCollectionCard.CardSelected( false );
		selectedCollectionCard.transform.parent.parent.GetComponent<CardSlot>( ).Select( false );
		selectedDeckCard.CardSelected( false );
		selectedDeckCard.transform.parent.parent.GetComponent<CardSlot>( ).Select( false );
		selectedCollectionCard = null;
		selectedDeckCard = null;

		// Update changes
		UpdateCollection( );
		UpdateDeck( );
	}

	private void UpdateCollection( )
	{
		int minAmount = upgrading ? 1 : 0; // We need more then 1 cards for upgrading and more then 0 for deck building

		// Find all the cards we have or, if we are upgrading, all the cards we can combine
		Card[] cards = allPlayerCards.Where( card => card.AmountPlayerOwns > minAmount && !( upgrading && card.Card.Level == CardLevel.Level3 ) ).Select( card => card.Card ).ToArray( );
		int[] amounts = allPlayerCards.Where( card => card.AmountPlayerOwns > minAmount && !( upgrading && card.Card.Level == CardLevel.Level3 ) ).Select( card => card.AmountPlayerOwns ).ToArray( );

		cardsInCollection = new CardInCollection[cards.Length];
		for ( int i = 0; i < cards.Length; i++ )
		{
			cardsInCollection[i] = new CardInCollection( )
			{
				Card = cards[i],
				Amount = amounts[i]
			};
		}

		cardsInCollection = cardsInCollection.OrderBy( card => card.Card.name).ToArray( );

		foreach ( var slot in collectionSlots )
			slot.SetEmpty( );

		for ( int i = 0; i < cardsInCollection.Length; i++ )
			cardsInCollection[i].Card = collectionSlots[i].Set( cardsInCollection[i].Card.gameObject, cardsInCollection[i].Amount );
	}

	private void UpdateDeck( )
	{
		// Find all the cards in master collection that we have in deck
		List<Card> cards = new List<Card>( );
		foreach ( var card in allPlayerCards )
		{
			for ( int i = 0; i < card.AmountInDeck; i++ )
			{
				cards.Add( card.Card );
			}
		}

		cardsInDeck = cards.ToArray( );
		cardsInDeck = cardsInDeck.OrderBy( card => card.name ).ToArray( );

		for ( int i = 0; i < cardsInDeck.Length; i++ )
			cardsInDeck[i] = deckSlots[i].Set( cardsInDeck[i].gameObject, 1 );
	}

	private void SaveCollectionAndDeck( )
	{
		CardsData cardsData = new CardsData( )
		{
			Name = allPlayerCards.Select( card => card.Card.Name ).ToArray( ),
			AmountPlayerOwns = allPlayerCards.Select( card => card.AmountPlayerOwns ).ToArray( ),
			AmountInDeck = allPlayerCards.Select( card => card.AmountInDeck ).ToArray( )
		};

		XmlSerializer xmlSerializer = new XmlSerializer( typeof( CardsData ) );
		using ( StringWriter writer = new StringWriter( ) )
		{
			xmlSerializer.Serialize( writer, cardsData );
			PlayerPrefs.SetString( "CardsData", writer.ToString( ) );
		}
	}

	private void LoadCollectionAndDeck( )
	{
		XmlSerializer xmlSerializer = new XmlSerializer( typeof( CardsData ) );
		string loadedData = PlayerPrefs.GetString( "CardsData" );

		// No card data found, using defaults
		if ( loadedData == "" )
			return;

		// Data found
		using ( StringReader reader = new StringReader( loadedData ) )
		{
			CardsData cardsData = xmlSerializer.Deserialize( reader ) as CardsData;

			for ( int i = 0; i < cardsData.Name.Length; i++ )
			{
				if ( allPlayerCards[i].Card.Name != cardsData.Name[i] )
				{
					Debug.LogError( "Saved data out of sync with Master Collection. Save data reset recommended. Using default data." );
					return;
				}

				allPlayerCards[i].AmountInDeck = cardsData.AmountInDeck[i];
				allPlayerCards[i].AmountPlayerOwns = cardsData.AmountPlayerOwns[i];
			}
		}
	}

	[ContextMenu( "Remove Saved Card Data" )]
	private void RemovdSavedCardData( )
	{
		PlayerPrefs.DeleteKey( "CardsData" );
	}

	private Card GetCardByName( string cardName ) => allPlayerCards.First( card => card.Card.Name == cardName ).Card;
}
