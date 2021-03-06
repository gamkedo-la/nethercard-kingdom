﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Deck : MonoBehaviour
{
	[Header("External")]
	[SerializeField] private PlayerCards playerCards = null;
	[SerializeField] private PlayerHand playerHand = null;

	[Header("Sounds")]
	[SerializeField] private PlaySound shuffleSound = null;
	[SerializeField] private PlaySound drawSound = null;
	[SerializeField] private float drawSoundDelay = 0.3f;
	[SerializeField] private float shuffleSoundDelay = 0.5f;

	[Header("Objects and Parameters")]
    [SerializeField] private GameObject hand = null;
    [SerializeField] private Animator animator = null;
    [SerializeField] private TextMeshProUGUI cardCountLabel = null;
	[SerializeField] private float lerpFactor = 0.25f;

	[Header("Cards")]
	[SerializeField] private GameObject card1 = null;
	[SerializeField] private GameObject card2 = null;
	[SerializeField] private GameObject card3 = null;
	[SerializeField] private GameObject card4 = null;

	[Header("Card Draw Conditions")]
    [SerializeField, Tooltip("If card limit <= 0, it means infinite.")] private int cardLimit = 3;
    [SerializeField, Tooltip("Auto draw will be disabled if value <= 0")] private float autoDrawDelay = 0.25f;
    [SerializeField] private int drawCost = 1;
    [SerializeField] private int incrementCostPerCard = 1;
    [SerializeField] private float delayAfterDraw = 3f;
    [SerializeField] private float startTime = 2f;

    [Header("New Card Properties")]
    [SerializeField] private Vector3 newCardPositionOffset = Vector3.zero;
    [SerializeField] private Vector3 newCardRotationOffset = Vector3.zero;

    private Vector3 scaleToLerp = Vector3.one;
    private float autoDrawTimer = 0.0f;
    private float timeTillNextDraw = 0.0f;
	private Queue<Card> drawQueue = new Queue<Card>();

	void OnEnable( )
	{
		card4.SetActive( true );
		card3.SetActive( true );
		card2.SetActive( true );
		card1.SetActive( true );

		animator.enabled = true;
		animator.SetTrigger( "Shuffle" );
		shuffleSound.Play( );
		timeTillNextDraw = startTime;
		cardCountLabel.text = $"{PlayerCards.MaxCardsInDeck} / {PlayerCards.MaxCardsInDeck}";
	}

	void OnDisable( )
	{
		animator.enabled = false;
	}

	void Update()
    {
		timeTillNextDraw -= Time.deltaTime;

        if (cardLimit <= 0 || hand.transform.childCount < cardLimit)
            if (autoDrawDelay > 0.0f && autoDrawTimer <= 0.0f)
                DrawCard();

        autoDrawTimer -= Time.deltaTime;

        transform.localScale = Vector3.Lerp(transform.localScale, scaleToLerp, lerpFactor);
    }

    public void OnHoverEnter()
    {
        scaleToLerp = Vector3.one * 1.2f;
    }

    public void OnHoverExit()
    {
        scaleToLerp = Vector3.one;
    }

    public void OnClicked()
    {
        if (cardLimit <= 0 || hand.transform.childCount < cardLimit)
            if (autoDrawDelay <= 0.0f)
                DrawCard();
    }

	private void DrawCard( )
	{
		// We wait between draws
		if ( timeTillNextDraw > 0 )
			return;
		timeTillNextDraw = delayAfterDraw;

		animator.SetTrigger( "Draw" );

		if ( !SummoningManager.Instance.EnoughMana( drawCost + ( incrementCostPerCard * hand.transform.childCount ) ) )
			return;

		SummoningManager.Instance.RemoveMana( drawCost + ( incrementCostPerCard * hand.transform.childCount ) );

		GameObject newCard = Instantiate
		(
			GetCardFromDeck( ),
			transform.position + newCardPositionOffset,
			Quaternion.Euler( newCardRotationOffset.x, newCardRotationOffset.y, newCardRotationOffset.z )
		);
		newCard.transform.SetParent( hand.transform, true );
		newCard.transform.SetSiblingIndex( 0 );

		cardCountLabel.text = $"{drawQueue.Count} / {PlayerCards.MaxCardsInDeck}";

		playerHand.AddCard( newCard.GetComponent<Card>( ) );
		newCard.GetComponent<CardAudioVisuals>( ).DoCardReveal( );

		autoDrawTimer = autoDrawDelay;

		StartCoroutine( PlayDrawSound( ) );
	}

	private IEnumerator PlayDrawSound()
	{
		yield return new WaitForSeconds( drawSoundDelay );

		drawSound.Play( );
	}

	private IEnumerator PlayShuffleSound( )
	{
		yield return new WaitForSeconds( shuffleSoundDelay );

		shuffleSound.Play( );
	}

	private GameObject GetCardFromDeck( )
	{
		if ( drawQueue.Count <= 0 )
			drawQueue = NewRandomizedDrawQueue( );

		Card card = drawQueue.Dequeue( );

		/*if ( drawQueue.Count <= 3 )
			card4.SetActive( false );
		if ( drawQueue.Count <= 2 )
			card3.SetActive( false );
		if ( drawQueue.Count <= 1 )
			card2.SetActive( false );
		if ( drawQueue.Count == 0 )
			card1.SetActive( false );*/

		return card.gameObject;
	}

	private Queue<Card> NewRandomizedDrawQueue( )
	{
		IEnumerable<Card> cards = playerCards.GetDeck( ).Select( card => card.Card );

		if ( CheatAndDebug.Instance.ShowDebugInfo )
		{
			string s = "";
			foreach ( var card in cards )
				s += $"{card.Name}\n";
			Debug.Log( s );
		}

		cards = cards.OrderBy( x => Random.Range( 0, 10000000 ) );

		return new Queue<Card>( cards );
	}
}
