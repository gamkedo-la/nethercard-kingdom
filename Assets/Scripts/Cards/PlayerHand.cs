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
	[SerializeField] private float xOffsetBetweenCards = 125.0f;

	[SerializeField] private float yOffsetBetweenCards = 5.0f;
	[SerializeField] private float angleOffsetBetweenCards = 20.0f;

	[Header("Hover Card Properties")]
	[SerializeField] private float hoverCardGap = 50.0f;
	[SerializeField] private float hoverCardYPosition = 5.0f;
	[SerializeField] private float hoverCardToMousePositionRatio = 0.15f;

	[Space]
	[SerializeField] private float draggedCardYPosition = 10.0f;
	[SerializeField] private float hideCardsYPositionOnDrag = -20.0f;

	private List<CardNew> cardsInHand = new List<CardNew>();
	private CardNew cardBeingAdded = null;

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
			SetCardPosition( cardsInHand[i], i, cardsInHand.Count );
			SetCardRotation( cardsInHand[i], i, cardsInHand.Count );
		}
	}

	public void AddCard( CardNew newCard )
	{
		cardBeingAdded = newCard;
		newCard.onOverEnter.AddListener( OnCardOverEnter );
		newCard.onOverExit.AddListener( OnCardOverExit );
	}

	private void OnCardOverEnter( CardNew card )
	{
		card.Vizuals.HighlightCardInHand( );
	}

	private void OnCardOverExit( CardNew card )
	{
		card.Vizuals.NormalCard( );
	}

	private void DestroyCard( CardNew card )
	{
		card.onOverEnter.RemoveAllListeners( );
		card.onOverExit.RemoveAllListeners( );

		cardsInHand.Remove( card );
		Destroy( card.gameObject );
	}

	private void SetCardPosition( CardNew card, int index, int totalCards )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		// Let's wait till the card is shown
		if ( card.Vizuals.Revealing )
			return;

		float startXOffset = -xOffsetBetweenCards * totalCards / 2;

		Vector3 cardPosition = transform.position + (Vector3)handOffset;
		cardPosition.x += startXOffset + ( xOffsetBetweenCards * index );

		cardPosition = Vector2.Lerp( card.transform.position, cardPosition, lerpFactor );

		card.transform.position = cardPosition;

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

	private void SetCardRotation( CardNew card, int index, int totalCards )
	{
		if ( !CheatAndDebug.Instance.UseAlternateImplementations )
			return;

		// Let's wait till the card is shown
		if ( card.Vizuals.Revealing )
			return;

		/*transform.GetChild( index ).rotation = Quaternion.Lerp( transform.GetChild( index ).rotation,
			Quaternion.Euler( 0f, 0f, ( index == hoverCardIndex ? 0.0f : angleOffset + ( ( (float)index - ( totalCards / 2.0f ) )
			* ( (float)angleOffsetBetweenCards / totalCards ) ) ) ),
			lerpFactor );*/
	}
}
