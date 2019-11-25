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

	[SerializeField] private int totalCardsAllowed = 3;
	[SerializeField] private Vector2 discardedCardOffset = Vector2.zero;
	[SerializeField] private bool addWidthToDiscardedX = true;
	[SerializeField] private float xOffset = 0f;
	[SerializeField] private float yOffset = 0f;
	[SerializeField] private float angleOffset = 0f;

	[Header("Cards Layout Properties")]
	[SerializeField] private float xOffsetBetweenCards = 100.0f;
	[SerializeField] private float yOffsetBetweenCards = 30.0f;
	[SerializeField] private float yOffsetOnOver = -40.0f;
	[SerializeField] private float yOffsetOnDrag = -60.0f;
	[SerializeField] private float angleOffsetBetweenCards = -10.0f;

	[Header("Hover Card Properties")]
	[SerializeField] private float hoverCardGap = 50.0f;
	[SerializeField] private float hoverCardYPosition = 5.0f;
	[SerializeField] private float hoverCardToMousePositionRatio = 0.15f;

	[Space]
	[SerializeField] private float draggedCardYPosition = 10.0f;
	[SerializeField] private float hideCardsYPositionOnDrag = -20.0f;

	private List<Card> cardsInHand = new List<Card>();
	private Card cardBeingAdded = null;
	private Card cardBeingDragged = null;
	private Card cardBeingOver = null;

	void Start( )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		/*if ( addWidthToDiscardedX )
			discardedCardOffset += new Vector2( Screen.width, 0.0f );*/
	}

	void Update( )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		// Delay adding card until it's reveled
		if ( cardBeingAdded && !cardBeingAdded.Vizuals.Revealing )
		{
			cardsInHand.Insert( 0, cardBeingAdded );
			cardBeingAdded = null;
		}

		// Move cards
		for ( int i = 0; i < cardsInHand.Count; i++ )
		{
			if ( cardBeingDragged && cardsInHand[i] == cardBeingDragged ) // We are dragging this card
			{
				DragCard( cardsInHand[i] );
			}
			else
			{
				SetCardPosition( cardsInHand[i], i, cardsInHand.Count, cardBeingOver == cardsInHand[i] ? false : cardBeingOver, cardBeingDragged == cardsInHand[i] ? false : cardBeingDragged );
				SetCardRotation( cardsInHand[i], i, cardsInHand.Count, cardBeingOver == cardsInHand[i] ? false : cardBeingOver, cardBeingDragged == cardsInHand[i] ? false : cardBeingDragged );
			}
		}

		// Drag spell preview if we have a card being dragged
		//cardBeingDragged?.Vizuals.ShowPreview( Input.mousePosition );
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

		//Debug.Log( "Drag Start" );
		cardBeingDragged = card;
		card.Vizuals.DraggedFromHand( );
		card.Vizuals.ShowPreview( true );
		StartSummoning( card );
	}

	private void OnCardDragEnd( Card card )
	{
		if ( !cardBeingDragged )
			return;

		//Debug.Log( "Drag End" );
		card.Vizuals.ShowPreview( false );
		cardBeingDragged = null;
		OnCardOverExit( card );
		EndSummoning( card );
	}

	private void OnCardCliked( Card card )
	{
		if ( cardBeingDragged )
			return;

		//OnCardDragStart( card );
		//Debug.Log( "Card Clicked" );
		card.OnBeginDrag( );
		//cardBeingDragged = card;
	}

	private void OnCardReleased( Card card )
	{
		if ( !cardBeingDragged )
			return;

		//Debug.Log( "Card Released" );
		//OnCardDragEnd( card );
		//cardBeingDragged = null;
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

	private void DragCard( Card card )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;
		// TODO: Show and drag live preview
		//Vector2 cardsNewPosition = Input.mousePosition;
		//card.transform.position = cardsNewPosition;
	}

	private void StartSummoning( Card card )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		//Debug.Log( $"On Start Summoning: {name}" );

		//if ( !SummoningManager.Instance.EnoughMana( card.UseCost ) )
			//return;

		//draggedCard = this;
		//OnOverEnter( );

		SummoningManager.Instance.Summoning( Camera.main.ScreenToWorldPoint( Input.mousePosition ), card.Type, true );
		//card.Vizuals.StartSummoning( );
	}

	private void EndSummoning( Card card )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		//Debug.Log( $"On End Summoning: {name}" );

		/*if ( draggedCard != this )
			return;*/

		//OnOverExit( );

		bool canSummon = SummoningManager.Instance.Summoning( Vector2.zero, card.Type, false );

		if ( canSummon )
		{
			GameObject instance = Instantiate( card.ToSummon, (Vector2)Camera.main.ScreenToWorldPoint( Input.mousePosition ), Quaternion.identity );
			if ( card.Type == CardType.DirectDefensiveSpell || card.Type == CardType.DirectOffensiveSpell || card.Type == CardType.AoeSpell )
				instance.GetComponent<Spell>( ).SetTarget( SummoningManager.Instance.LastTarget );

			SummoningManager.Instance.RemoveMana( card.UseCost );
			card.Vizuals.EndSummoning( );
			DestroyCard( card );
		}
		else
			card.Vizuals.CancelSummoning( );
	}

	private void SetCardPosition( Card card, int index, int totalCards, bool haveOverCard, bool haveDragCard )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

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

		/*Vector3 cardPosition = transform.GetChild( index ).position;

		Vector3 newCardPosition = Vector3.zero;
		newCardPosition.x = ( Screen.width / 2 ) + xOffset + ( ( index - ( totalCards / 2 ) )
			* ( xOffsetBetweenCards / totalCards ) );
		newCardPosition.y = yOffset + ( ( index > ( totalCards / 2 ) ? index : ( totalCards - index ) )
			* ( yOffsetBetweenCards / totalCards ) );

		if ( hoverCardIndex > -1 )
		{
			if ( index < hoverCardIndex )
				newCardPosition.x -= hoverCardGap;
			else if ( index > hoverCardIndex )
				newCardPosition.x += hoverCardGap;
			else
			{
				newCardPosition.y = hoverCardYPosition;
				newCardPosition = Vector2.Lerp( newCardPosition, Input.mousePosition, hoverCardToMousePositionRatio );
			}
		}

		if ( Card.draggedCard != null && index < totalCardsAllowed )
		{
			if ( Card.draggedCard == transform.GetChild( index ).GetComponent<Card>( ) )
			{
				newCardPosition.y = draggedCardYPosition;

				return;
				//reason: don't change the position of dragged card because
				//it is suppose to move to the cursor via Card script
			}
			else
			{
				newCardPosition.y = hideCardsYPositionOnDrag;
			}
		}

		if ( index >= totalCardsAllowed )
		{
			newCardPosition = new Vector2( xOffset, yOffset ) + discardedCardOffset;
		}

		transform.GetChild( index ).position = Vector3.Lerp( cardPosition, newCardPosition, lerpFactor );*/
	}

	private void SetCardRotation( Card card, int index, int totalCards, bool haveOverCard, bool haveDragCard )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		// Let's wait till the card is shown
		if ( card.Vizuals.Revealing )
			return;

		float newAngle = angleOffsetBetweenCards * ( index - ( totalCards / 2 ) );

		Quaternion newRotation = Quaternion.Euler( 0, 0, newAngle );
		newRotation = Quaternion.Lerp( card.transform.localRotation, newRotation, lerpFactor );

		card.transform.localRotation = newRotation;

		/*transform.GetChild( index ).rotation = Quaternion.Lerp( transform.GetChild( index ).rotation,
			Quaternion.Euler( 0f, 0f, ( index == hoverCardIndex ? 0.0f : angleOffset + ( ( (float)index - ( totalCards / 2.0f ) )
			* ( (float)angleOffsetBetweenCards / totalCards ) ) ) ),
			lerpFactor );*/
	}
}
