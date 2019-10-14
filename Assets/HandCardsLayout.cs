using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCardsLayout : MonoBehaviour
{
    [SerializeField] private float lerpFactor = 0.1f;

    [Space]
    [SerializeField] private float xOffset = 0f;
    [SerializeField] private float yOffset = 0f;
    [SerializeField] private float angleOffset = 0f;

    [Space]
    [SerializeField] private float xOffsetBetweenCards = 35.0f;
    [SerializeField] private float yOffsetBetweenCards = 5.0f;
    [SerializeField] private float angleOffsetBetweenCards = 20.0f;

    [Space]
    [SerializeField] private float hoverCardGap = 50.0f;
    [SerializeField] private float hoverCardYPosition = 5.0f;

    void Start()
    {
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
        newCardPosition.x = xOffset + (((float)index - (totalCards / 2.0f))
            * ((float)xOffsetBetweenCards / totalCards));
        newCardPosition.y = yOffset + ((index > totalCards / 2.0f ? index : (totalCards - index))
            * ((float)yOffsetBetweenCards / totalCards));

        if(hoverCardIndex > -1)
        {
            if(index < hoverCardIndex) newCardPosition.x -= hoverCardGap;
            else if(index > hoverCardIndex) newCardPosition.x += hoverCardGap;
            else newCardPosition.y = hoverCardYPosition;
        }

        transform.GetChild(index).position = Vector3.Lerp(cardPosition, newCardPosition, lerpFactor);;
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
            SetCardPosition(totalCards, i, hoverCardIndex);
            SetCardRotation(totalCards, i, hoverCardIndex);
        }
    }
}
