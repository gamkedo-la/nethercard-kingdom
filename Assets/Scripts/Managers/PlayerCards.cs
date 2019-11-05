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
	private class CardsSaveData
	{
		public string[] Name;
		public int[] Amount;
	}

	public List<PlayerCard> Collection { get; set; } = new List<PlayerCard>( );
	public List<PlayerCard> Deck { get; set; } = new List<PlayerCard>( );

	public const int MaxCardsInDeck = 10;
	public const int MaxIdenticalCardsInDeck = 3;

	private const string PlayerCollection = "Player Collection";
	private const string PlayerDeck = "Player Deck";

	private AllGameCards gameCards;

	void Start ()
	{
		gameCards = GetComponent<AllGameCards>( );
		Assert.IsNotNull( gameCards, $"Please assign <b>{nameof( gameCards )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	public void SavePlayerCardsData( )
	{
		XmlSerializer xmlSerializer = new XmlSerializer( typeof( CardsSaveData ) );

		// Collection
		CardsSaveData collectionData = new CardsSaveData( )
		{
			Name = Collection.Select( card => card.Card.Name ).ToArray( ),
			Amount = Collection.Select( card => card.Amount ).ToArray( )
		};

		using ( StringWriter writer = new StringWriter( ) )
		{
			xmlSerializer.Serialize( writer, collectionData );
			PlayerPrefs.SetString( PlayerCollection, writer.ToString( ) );
		}

		// Deck
		CardsSaveData deckData = new CardsSaveData( )
		{
			Name = Deck.Select( card => card.Card.Name ).ToArray( ),
			Amount = Deck.Select( card => card.Amount ).ToArray( )
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

		Collection.Clear( );
		Deck.Clear( );

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

				Collection.Add( new PlayerCard( ) { Card = loadedCard, Amount = cardsData.Amount[i] } );
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

				Deck.Add( new PlayerCard( ) { Card = loadedCard, Amount = cardsData.Amount[i] } );
			}
		}
	}

	private void LoadDefaultPlayerCards( )
	{
		Collection.Clear( );
		Deck.Clear( );

		foreach ( var card in gameCards.GamesPlayerCards )
		{
			// Collection
			if ( card.DefaultPlayerOwned > 0 )
				Collection.Add( new PlayerCard( ) { Card = card.Card, Amount = card.DefaultPlayerOwned } );

			// Deck
			if ( card.DefaultInDeck > 0 )
				Deck.Add( new PlayerCard( ) { Card = card.Card, Amount = card.DefaultInDeck } );
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
