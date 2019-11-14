/**
 * Description: Collection of all of player's cards (both in their deck and outside).
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(AllGameCards))]
public class PlayerCards : MonoBehaviour
{
	[System.Serializable]
	public class CardsSaveData
	{
		public string[] Name;
		public int[] Amount;
	}

	public List<PlayerCard> GetCollection { get { return new List<PlayerCard>( collection ); } }
	public List<PlayerCard> GetDeck { get { return new List<PlayerCard>( deck ); } }

	public const int MaxCardsInDeck = 10;
	public const int MaxIdenticalCardsInDeck = 3;

	private const string PlayerCollection = "Player Collection";
	private const string PlayerDeck = "Player Deck";

	private List<PlayerCard> collection = new List<PlayerCard>( );
	private List<PlayerCard> deck = new List<PlayerCard>( );

	private AllGameCards gameCards;

	void Start ()
	{
		gameCards = GetComponent<AllGameCards>( );
		Assert.IsNotNull( gameCards, $"Please assign <b>{nameof( gameCards )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	public bool AreCardsOfTheSameType( PlayerCard cardToCheck, PlayerCard otherCard )
	{
		// Same level cards
		if ( otherCard.Card.Name == cardToCheck.Card.Name )
			return true;

		// Level 2 or Level 3 card vs. lower level
		if ( otherCard.Card.LowerLevelVersion && otherCard.Card.LowerLevelVersion.Name == cardToCheck.Card.Name )
			return true;

		// Level 3 card vs. Level 1
		if ( otherCard.Card.LowerLevelVersion && otherCard.Card.LowerLevelVersion.LowerLevelVersion && otherCard.Card.LowerLevelVersion.LowerLevelVersion.Name == cardToCheck.Card.Name )
			return true;

		// Level 1 and Level 2 card vs. higher level
		if ( otherCard.Card.HigherLevelVersion && otherCard.Card.HigherLevelVersion.Name == cardToCheck.Card.Name )
			return true;

		// Level 1 card vs. Level 3
		if ( otherCard.Card.HigherLevelVersion && otherCard.Card.HigherLevelVersion.HigherLevelVersion && otherCard.Card.HigherLevelVersion.HigherLevelVersion.Name == cardToCheck.Card.Name )
			return true;

		return false;
	}

	public void SavePlayerCardsData( )
	{
		XmlSerializer xmlSerializer = new XmlSerializer( typeof( CardsSaveData ) );

		// Collection
		CardsSaveData collectionData = new CardsSaveData( )
		{
			Name = collection.Select( card => card.Card.Name ).ToArray( ),
			Amount = collection.Select( card => card.Amount ).ToArray( )
		};

		using ( StringWriter writer = new StringWriter( ) )
		{
			xmlSerializer.Serialize( writer, collectionData );
			PlayerPrefs.SetString( PlayerCollection, writer.ToString( ) );
		}

		// Deck
		CardsSaveData deckData = new CardsSaveData( )
		{
			Name = deck.Select( card => card.Card.Name ).ToArray( ),
			Amount = deck.Select( card => card.Amount ).ToArray( )
		};

		using ( StringWriter writer = new StringWriter( ) )
		{
			xmlSerializer.Serialize( writer, deckData );
			PlayerPrefs.SetString( PlayerDeck, writer.ToString( ) );
		}
	}

	public void LoadPlayerCardsData( )
	{
		XmlSerializer xmlSerializer = new XmlSerializer( typeof( CardsSaveData ) );

		string loadedPlayerCollection = PlayerPrefs.GetString( PlayerCollection );
		string loadedPlayerDeck = PlayerPrefs.GetString( PlayerDeck );

		// No card data found, using defaults
		if ( loadedPlayerCollection == "" || loadedPlayerDeck == "" )
		{
			LoadDefaultPlayerCards( );
			return;
		}

		// Data found

		collection.Clear( );
		deck.Clear( );

		using ( StringReader reader = new StringReader( loadedPlayerCollection ) ) // Collection
		{
			CardsSaveData cardsData = xmlSerializer.Deserialize( reader ) as CardsSaveData;

			for ( int i = 0; i < cardsData.Name.Length; i++ )
			{
				Card loadedCard = gameCards.GetPlayerCardByName( cardsData.Name[i] );
				if ( !loadedCard )
				{
					Debug.LogError( "Saved collection data out of sync with game's player cards. Save data reset recommended. Using default data." );
					LoadDefaultPlayerCards( );

					return;
				}

				collection.Add( new PlayerCard( ) { Card = loadedCard, Amount = cardsData.Amount[i] } );
			}
		}

		using ( StringReader reader = new StringReader( loadedPlayerDeck ) ) // Deck
		{
			CardsSaveData cardsData = xmlSerializer.Deserialize( reader ) as CardsSaveData;

			for ( int i = 0; i < cardsData.Name.Length; i++ )
			{
				Card loadedCard = gameCards.GetPlayerCardByName( cardsData.Name[i] );
				if ( !loadedCard )
				{
					Debug.LogError( "Saved deck data out of sync with game's player cards. Save data reset recommended. Using default data." );
					LoadDefaultPlayerCards( );

					return;
				}

				deck.Add( new PlayerCard( ) { Card = loadedCard, Amount = cardsData.Amount[i] } );
			}
		}
	}

	private void LoadDefaultPlayerCards( )
	{
		collection.Clear( );
		deck.Clear( );

		foreach ( var card in gameCards.GamesPlayerCards )
		{
			// Collection
			if ( card.DefaultPlayerOwned > 0 )
				collection.Add( new PlayerCard( ) { Card = card.Card, Amount = card.DefaultPlayerOwned } );

			// Deck
			if ( card.DefaultInDeck > 0 )
				for ( int i = 0; i < card.DefaultInDeck; i++ ) // Add as many cards of this type as we have
					deck.Add( new PlayerCard( ) { Card = card.Card, Amount = 1 } );
		}
	}

	[ContextMenu( "Remove Saved Player Cards Data" )]
	private void RemovdSavedPlayerCardsData( )
	{
		PlayerPrefs.DeleteKey( "CardsData" );
		PlayerPrefs.DeleteKey( PlayerCollection );
		PlayerPrefs.DeleteKey( PlayerDeck );
	}
}
