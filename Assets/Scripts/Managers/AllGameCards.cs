/**
 * Description: Contains all of game cards.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class PlayerGameCard
{
	public Card Card;
	public int DefaultPlayerOwned;
	public int DefaultInDeck;
}

public class AllGameCards : MonoBehaviour
{
	public PlayerGameCard[] GamesPlayerCards { get { return allPlayerGameCards; } }
	public Unit[] GamesOpponentCards { get { return allOpponentGameCards; } }

	[Tooltip("All player cards there are in game (with default/start amounts the player has).")]
	[SerializeField] private PlayerGameCard[] allPlayerGameCards = null;
	[Tooltip("All enemy cards there are in game.")]
	[SerializeField] private Unit[] allOpponentGameCards = null;

	void Start ()
	{
		Assert.AreNotEqual( allPlayerGameCards.Length, 0, $"Please assign <b>{nameof( allPlayerGameCards )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		int cardsInDeck = 0;
		foreach ( var card in allPlayerGameCards )
			cardsInDeck += card.DefaultInDeck;
		Assert.AreEqual( cardsInDeck, PlayerCards.MaxCardsInDeck, $"<b>{nameof( allPlayerGameCards )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object has to have <b>exactly</b> {PlayerCards.MaxCardsInDeck} cards in deck" );

		foreach ( var card in allPlayerGameCards )
			if ( card.DefaultInDeck > PlayerCards.MaxIdenticalCardsInDeck || card.DefaultInDeck < 0 )
				Debug.LogError( $"You can't have more then {PlayerCards.MaxIdenticalCardsInDeck} of identical cards (card name: {card.Card.Name}" );

		Assert.AreNotEqual( allOpponentGameCards.Length, 0, $"Please assign <b>{nameof( allOpponentGameCards )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	public Card GetPlayerCardByName( string cardName ) => GamesPlayerCards.FirstOrDefault( card => card.Card.Name == cardName ).Card;
}
