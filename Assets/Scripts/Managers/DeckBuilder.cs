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
	[SerializeField] private Animator animator = null;
	[SerializeField] private Button combineButton = null;
	[SerializeField] private TextMeshProUGUI tooltip = null;
	[SerializeField] private GameObject[] toHideOnClose = null;
	[SerializeField] private GameObject[] toShowOnClose = null;

	[Header("Master Collection")]
	[SerializeField] private CardInMasterCollection[] allPlayerCards = null;

	[Header("Collection")]
	[SerializeField] private CardSlot[] collectionSlots = null;

	[Header("Deck")]
	[SerializeField] private CardSlot[] deckSlots = null;
	[SerializeField] private int maxIdenticalDeckCards = 3;

	[Header("Upgrade")]
	[SerializeField] private CardSlot[] upgradeSlots = null;

	private CardInCollection[] cardsInCollection = null;
	private Card[] cardsInDeck = null;

	public GameObject selectedSlot = null;
	public GameObject otherSlot = null;

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

	void Start( )
	{
		Assert.IsNotNull( combineButton, $"Please assign <b>{nameof( combineButton )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( tooltip, $"Please assign <b>{nameof( tooltip )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( animator, $"Please assign <b>{nameof( animator )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		Assert.AreNotEqual( toHideOnClose.Length, 0, $"Please assign <b>{nameof( toHideOnClose )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.AreNotEqual( toShowOnClose.Length, 0, $"Please assign <b>{nameof( toShowOnClose )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.AreNotEqual( allPlayerCards.Length, 0, $"Please assign <b>{nameof( allPlayerCards )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		int cardsInDeck = 0;
		foreach ( var card in allPlayerCards )
			cardsInDeck += card.AmountInDeck;
		Assert.AreEqual( cardsInDeck, CardsInDeck, $"<b>{nameof( allPlayerCards )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object has to have <b>exactly</b> {CardsInDeck} cards in deck" );

		Assert.AreNotEqual( collectionSlots.Length, 0, $"Please assign <b>{nameof( collectionSlots )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.AreEqual( deckSlots.Length, CardsInDeck, $"Please make sure <b>{nameof( deckSlots )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object has 10 elements (number of cards a Deck should have)" );

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
			slot.SetEmpty( );

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

	public Card SelectedCard( ) { if ( selectedSlot != null ) return GetCardFromSlot( selectedSlot ); return null; }
	public Card GetCardFromSlot( GameObject slot ) { if ( slot != null && slot.transform.GetChild( 3 ) != null && slot.transform.GetChild( 3 ).GetChild( 0 ) != null && slot.transform.GetChild( 3 ).GetChild( 0 ).GetComponent<Card>( ) != null ) return slot.transform.GetChild( 3 ).GetChild( 0 ).GetComponent<Card>( ); return null; }

	public void UpgradeCard( )
	{
		// De-select
		if ( !selectedSlot )
			return;

		Card cardToUpgrade = SelectedCard( ).SelectionMode == CardSelectionMode.InCollection ? SelectedCard( ) : null;
		//CardClicked( selectedCollectionCard, CardSelectionMode.InCollection );

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
		return ( selectedSlot && SelectedCard( )?.SelectionMode == CardSelectionMode.InCollection )
		|| ( otherSlot && GetCardFromSlot( otherSlot )?.SelectionMode == CardSelectionMode.InCollection );
	}

	public bool IsDeckCardSelected( )
	{
		return ( selectedSlot && SelectedCard( ).SelectionMode == CardSelectionMode.InDeck )
		|| ( otherSlot && GetCardFromSlot( otherSlot ).SelectionMode == CardSelectionMode.InDeck );
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
			foreach ( var card in cardsInDeck )
			{
				// Same level cards
				if ( card.Name == selectedCollectionCard.Name )
					sameCardsInDeck++;

				// Level 2 or Level 3 card vs. lower level
				if ( card.LowerLevelVersion && card.LowerLevelVersion.Name == selectedCollectionCard.Name )
					sameCardsInDeck++;

				// Level 3 card vs. Level 1
				if ( card.LowerLevelVersion && card.LowerLevelVersion.LowerLevelVersion && card.LowerLevelVersion.LowerLevelVersion.Name == selectedCollectionCard.Name )
					sameCardsInDeck++;

				// Level 1 and Level 2 card vs. higher level
				if ( card.HigherLevelVersion && card.HigherLevelVersion.Name == selectedCollectionCard.Name )
					sameCardsInDeck++;

				// Level 1 card vs. Level 3
				if ( card.HigherLevelVersion && card.HigherLevelVersion.HigherLevelVersion && card.HigherLevelVersion.HigherLevelVersion.Name == selectedCollectionCard.Name )
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

			selectedCollectionCard.CardSelected( false );
			selectedDeckCard.CardSelected( false );
		}
		else if ( selectedCollectionCard != null && selectedUpgradeCard != null )
		{
			// Tried to add more then max identical Cards to the Deck
			int sameCardsInDeck = cardsInDeck.Count( card => card.Name == selectedCollectionCard.Name );
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
			CardInMasterCollection cardInCollectionToSwap = allPlayerCards.First( card => card.Card.Name == selectedCollectionCard.Name );

			//cardInCollectionToSwap.AmountPlayerOwns--; //Only SHOW in upgrade

			// Remove card selected in the upgrade
			CardInMasterCollection cardInUpgradeToSwap = allPlayerCards.First( card => card.Card.Name == selectedUpgradeCard.Name );
			//cardInUpgradeToSwap.AmountInDeck--; // MISSING How can I remove this card from Upgrade?

			//selectedUpgradeCard.transform.parent.parent.GetComponent<CardSlot>().SetEmpty(); //Only SHOW in upgrade

			// Add card selected from the collection to upgrade
			//cardInCollectionToSwap.AmountInDeck++; // MISSING How can I add this card to Upgrade?
			if ( otherSlot.GetComponent<CardSlot>( ).IsEmpty( ) )
				otherSlot.GetComponent<CardSlot>( ).Set( selectedCollectionCard.gameObject, 1 );

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
			CardInMasterCollection addedCard = allPlayerCards.First( card => card.Card.Name == selectedCard.Name );

			if ( selectedCard == selectedCollectionCard )
			{
				if ( otherSlot.name.Contains( "Deck" ) )
				{
					addedCard.AmountPlayerOwns--;
					addedCard.AmountInDeck++;
				}
				else if ( otherSlot.name.Contains( "Upgrade" ) )
				{
					otherSlot.GetComponent<CardSlot>( ).Set( selectedCard.gameObject, 1 );
				}
			}
			else if ( selectedCard == selectedDeckCard )
			{
				addedCard.AmountInDeck--;

				if ( otherSlot.name.Contains( "Collection" ) )
					addedCard.AmountPlayerOwns++;

				//Deck card can not access upgrade
			}
			else if ( selectedCard == selectedUpgradeCard )
			{
				//addedCard.AmountInDeck--; //MISSING How can I remove this card from Upgrade?
				selectedUpgradeCard.transform.parent.parent.GetComponent<CardSlot>( ).SetEmpty( );

				if ( otherSlot.name.Contains( "Collection" ) )
					addedCard.AmountPlayerOwns++;

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

		cardsInCollection = cardsInCollection.OrderBy( card => card.Card.name ).ToArray( );

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
