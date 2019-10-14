using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCardsLayout : MonoBehaviour
{
    [SerializeField] private float xOffset = 0f;
    [SerializeField] private float yOffset = 0f;
    [SerializeField] private float angleOffset = 0f;

    [SerializeField] private float xOffsetBetweenCards = 35.0f;
    [SerializeField] private float yOffsetBetweenCards = 5.0f;
    [SerializeField] private float angleOffsetBetweenCards = 20.0f;

    void Start()
    {
    }

    void Update()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Vector3 cardPosition = transform.GetChild(i).position;
            cardPosition.x = xOffset + (((float)i - (transform.childCount / 2.0f)) * xOffsetBetweenCards);
            cardPosition.y = yOffset + ((i > transform.childCount / 2.0f ? i : (transform.childCount - i)) * yOffsetBetweenCards);
            transform.GetChild(i).position = cardPosition;

            transform.GetChild(i).rotation = Quaternion.Euler(0f, 0f,
            angleOffset + (((float)i - (transform.childCount / 2.0f)) * angleOffsetBetweenCards));
        }
    }
}
