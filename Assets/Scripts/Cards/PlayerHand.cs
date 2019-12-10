/**
 * Description: Takes care of player's hand and cards in it during battles.
 * Authors: Kornel, Bilal
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
	[Header("Hand Properties")]
	[SerializeField] private Vector2 handOffset = new Vector2(75,-50);
	[SerializeField] private float lerpFactor = 0.25f;

	[Header("Cards Layout Properties")]
	[SerializeField] private float xOffsetBetweenCards = 100.0f;
	[SerializeField] private float yOffsetBetweenCards = 30.0f;
	[SerializeField] private float yOffsetOnOver = -40.0f;
	[SerializeField] private float yOffsetOnDrag = -60.0f;
	[SerializeField] private float angleOffsetBetweenCards = -10.0f;

	private List<Card> cardsInHand = new List<Card>();
	private Card cardBeingAdded = null;
	private Card cardBeingDragged = null;
	private Card cardBeingOver = null;

	void Update( )
	{
		// Delay adding card until it's reveled
		if ( cardBeingAdded && !cardBeingAdded.Vizuals.Revealing )
		{
			cardsInHand.Insert( 0, cardBeingAdded );
			cardBeingAdded = null;
		}

		// Move cards
		for ( int i = 0; i < cardsInHand.Count; i++ )
		{
			SetCardPosition( cardsInHand[i], i, cardsInHand.Count, cardBeingOver == cardsInHand[i] ? false : cardBeingOver, cardBeingDragged == cardsInHand[i] ? false : cardBeingDragged );
			SetCardRotation( cardsInHand[i], i, cardsInHand.Count );
		}

		CheckIfWeForceCanceled( );
	}

	public void AddCard( Card newCard )
	{
		cardBeingAdded = newCard;
		newCard.onStartedDrag.AddListener( OnCardDragStart );
		newCard.onEndedDrag.AddListener( OnCardDragEnd );
		newCard.onOverEnter.AddListener( OnCardOverEnter );
		newCard.onOverExit.AddListener( OnCardOverExit );
		newCard.onClicked.AddListener( OnCardCliked );
		newCard.onRelease.AddListener( OnCardReleased );
	}

	private void CheckIfWeForceCanceled( )
	{
		if ( cardBeingDragged == null || !Input.GetMouseButtonDown( 1 ) )
			return;

		SummoningManager.Instance.Summoning( cardBeingDragged.Type, false );
		cardBeingDragged.Vizuals.ShowPreview( false );
		cardBeingDragged.Vizuals.CancelSummoning( );
		cardBeingDragged.Vizuals.NormalCard( );
		cardBeingDragged = null;
	}

	private void OnCardOverEnter( Card card )
	{
		cardBeingOver = card;

		if ( cardBeingDragged )
			return;

		card.Vizuals.HighlightCardInHand( );
	}

	private void OnCardOverExit( Card card )
	{
		cardBeingOver = null;

		if ( cardBeingDragged )
			return;

		card.Vizuals.NormalCard( );
	}

	private void OnCardDragStart( Card card )
	{
		if ( cardBeingDragged )
			return;

		if ( !SummoningManager.Instance.EnoughMana( card.UseCost ) )
			return;

		cardBeingDragged = card;
		card.Vizuals.DraggedFromHand( );
		card.Vizuals.ShowPreview( true );
		StartSummoning( card );
		LevelManager.Instance.PlayingCard( true );
	}

	private void OnCardDragEnd( Card card )
	{
		if ( !cardBeingDragged )
			return;

		card.Vizuals.ShowPreview( false );
		cardBeingDragged = null;
		OnCardOverExit( card );
		EndSummoning( card );
		LevelManager.Instance.PlayingCard( false );
	}

	private void OnCardCliked( Card card )
	{
		if ( cardBeingDragged )
			return;

		card.OnBeginDrag( );
	}

	private void OnCardReleased( Card card )
	{
		if ( !cardBeingDragged )
			return;
	}

	private void StartSummoning( Card card )
	{
		SummoningManager.Instance.Summoning( card.Type, true );
	}

	private void EndSummoning( Card card )
	{
		bool canSummon = SummoningManager.Instance.Summoning( card.Type, false );

		if (!canSummon)
		{
			card.Vizuals.CancelSummoning( );
			return;
		}

		GameObject instance = Instantiate( card.ToSummon, (Vector2)Camera.main.ScreenToWorldPoint( Input.mousePosition ), Quaternion.identity );
		if ( card.Type == CardType.DirectDefensiveSpell || card.Type == CardType.DirectOffensiveSpell || card.Type == CardType.AoeSpell )
			instance.GetComponent<Spell>( ).SetTarget( SummoningManager.Instance.LastTarget );

		SummoningManager.Instance.RemoveMana( card.UseCost );
		card.Vizuals.EndSummoning( );
		DestroyCard( card );
	}

	private void SetCardPosition( Card card, int index, int totalCards, bool haveOverCard, bool haveDragCard )
	{
		// Let's wait till the card is shown
		if ( card.Vizuals.Revealing )
			return;

		float startXOffset = -xOffsetBetweenCards * totalCards / 2;

		Vector3 cardsNewPosition = transform.position + (Vector3)handOffset;
		cardsNewPosition.x += startXOffset + ( xOffsetBetweenCards * index );

		float yOffset = 0;
		if ( index < totalCards / 2 )
			yOffset += yOffsetBetweenCards * ( index - ( totalCards / 2 ) );
		else if ( index > totalCards / 2 )
			yOffset -= yOffsetBetweenCards * ( index - ( totalCards / 2 ) );
		else if ( index == totalCards - 1 )
			yOffset -= yOffsetBetweenCards;

		cardsNewPosition.y += yOffset;

		if ( haveOverCard && !haveDragCard )
			cardsNewPosition.y += yOffsetOnOver;
		if ( haveDragCard )
			cardsNewPosition.y += yOffsetOnDrag;

		cardsNewPosition = Vector2.Lerp( card.transform.position, cardsNewPosition, lerpFactor );

		card.transform.position = cardsNewPosition;
	}

	private void SetCardRotation( Card card, int index, int totalCards )
	{
		// Let's wait till the card is shown
		if ( card.Vizuals.Revealing )
			return;

		float newAngle = angleOffsetBetweenCards * ( index - ( totalCards / 2 ) );

		Quaternion newRotation = Quaternion.Euler( 0, 0, newAngle );
		newRotation = Quaternion.Lerp( card.transform.localRotation, newRotation, lerpFactor );

		card.transform.localRotation = newRotation;
	}

	private void DestroyCard( Card card )
	{
		card.onStartedDrag.RemoveAllListeners( );
		card.onEndedDrag.RemoveAllListeners( );
		card.onOverEnter.RemoveAllListeners( );
		card.onOverExit.RemoveAllListeners( );
		card.onClicked.RemoveAllListeners( );
		card.onRelease.RemoveAllListeners( );

		cardsInHand.Remove( card );
		Destroy( card.gameObject );
	}
}
