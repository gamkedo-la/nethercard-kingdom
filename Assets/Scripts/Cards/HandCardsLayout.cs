/**
 * Description: Takes care of player's hand and cards in it.
 * Authors: Bilal
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;

public class HandCardsLayout : MonoBehaviour
{
    [Header("Hand Properties")]
    [SerializeField] private float lerpFactor = 0.1f;
    [SerializeField] private int totalCardsAllowed = 3;
    [SerializeField] private Vector2 discardedCardOffset = Vector2.zero;
    [SerializeField] private bool addWidthToDiscardedX = true;
    [SerializeField] private float xOffset = 0f;
    [SerializeField] private float yOffset = 0f;
    [SerializeField] private float angleOffset = 0f;

    [Header("Cards Layout Properties")]
    [SerializeField] private float xOffsetBetweenCards = 35.0f;
    [SerializeField] private float yOffsetBetweenCards = 5.0f;
    [SerializeField] private float angleOffsetBetweenCards = 20.0f;

    [Header("Hover Card Properties")]
    [SerializeField] private float hoverCardGap = 50.0f;
    [SerializeField] private float hoverCardYPosition = 5.0f;
    [SerializeField] private float hoverCardToMousePositionRatio = 0.15f;

    [Space]
    [SerializeField] private float draggedCardYPosition = 10.0f;
    [SerializeField] private float hideCardsYPositionOnDrag = -20.0f;

    void Start()
    {
        if(addWidthToDiscardedX)
            discardedCardOffset += new Vector2(Screen.width, 0.0f);
    }

    private int GetHoverCardIndex( int totalCards )
    {
        for(int hoverCardIndex = 0; hoverCardIndex < totalCards; hoverCardIndex++)
            if(transform.GetChild(hoverCardIndex).GetComponent<Card>() == Card.hoverCard)
                return hoverCardIndex;

        return -1;
    }

    private void SetCardPosition( int totalCards, int index, int hoverCardIndex = -1 )
    {
        Vector3 cardPosition = transform.GetChild(index).position;

        Vector3 newCardPosition = Vector3.zero;
        newCardPosition.x = (Screen.width / 2) + xOffset + ((index - (totalCards / 2))
            * (xOffsetBetweenCards / totalCards));
        newCardPosition.y = yOffset + ((index > (totalCards / 2) ? index : (totalCards - index))
            * (yOffsetBetweenCards / totalCards));

        if(hoverCardIndex > -1)
        {
            if(index < hoverCardIndex) newCardPosition.x -= hoverCardGap;
            else if(index > hoverCardIndex) newCardPosition.x += hoverCardGap;
            else
            {
				newCardPosition.y = hoverCardYPosition;
				newCardPosition = Vector2.Lerp(newCardPosition, Input.mousePosition, hoverCardToMousePositionRatio);
			}
        }

        if(Card.draggedCard != null && index < totalCardsAllowed)
        {
            if(Card.draggedCard == transform.GetChild(index).GetComponent<Card>())
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

        if(index >= totalCardsAllowed)
        {
            newCardPosition = new Vector2(xOffset, yOffset) + discardedCardOffset;
        }

        transform.GetChild(index).position = Vector3.Lerp(cardPosition, newCardPosition, lerpFactor);
    }

    private void SetCardRotation( int totalCards, int index, int hoverCardIndex = -1 )
    {
        transform.GetChild(index).rotation = Quaternion.Lerp(transform.GetChild(index).rotation,
            Quaternion.Euler(0f, 0f, (index == hoverCardIndex ? 0.0f : angleOffset + (((float)index - (totalCards / 2.0f))
            * ((float)angleOffsetBetweenCards / totalCards)))),
            lerpFactor);
    }

    void Update()
    {
        int totalCards = transform.childCount;
        int hoverCardIndex = GetHoverCardIndex(totalCards);
        for(int i = 0; i < totalCards; i++)
        {
            Card card = transform.GetChild(i).GetComponent<Card>();
            if((card.lerpBackTimer <= 0f || !card.lerpBack ) && !card.Revealing)
            {
                SetCardPosition(totalCards, i, hoverCardIndex);
                SetCardRotation(totalCards, i, hoverCardIndex);
            }

            if(i >= totalCardsAllowed)
            {
                if(Vector2.Distance(transform.GetChild(i).position, new Vector2(xOffset, yOffset) + discardedCardOffset) < 25f)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
            }
        }
    }
}
