/**
 * Description: Enemy oponent player.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class OponentDeckCard
{
	public GameObject Instance;
	public int SummonCost;
}

public class EnemyOponentAlt : MonoBehaviour
{
	public static EnemyOponent Instance { get; private set; }
	public bool IsPlaying { get; set; } = true;

	[SerializeField] private OponentDeckCard[] deck = null;
	[SerializeField] private GameObject spawnIndicator = null;
	[SerializeField] private GameObject summoningPoint = null;
	[SerializeField] private Vector2 summonArea = new Vector2(2f, 4f);
	[SerializeField] private float startDelay = 5f;
	[SerializeField] private float summonDelay = 0.5f;
	[SerializeField] private float delayBetweenCardPlays = 1f;
	[SerializeField] private float manaTickTime = 2f;
	[SerializeField] private int manaTickAmount = 1;
	[SerializeField] private int startMana = 3;

	[SerializeField] private int currentMana = 0;
	[SerializeField] private float currentManaProgress = 0f;
	private float timeToNextSummon = 0f;
	private Queue<OponentDeckCard> drawQueue = new Queue<OponentDeckCard>();
	private List<OponentDeckCard> hand = new List<OponentDeckCard>();

	private void Awake( )
	{
		if ( Instance != null && Instance != this )
			Destroy( this );
		else
			Instance = this;
	}

	private void OnDestroy( ) { if ( this == Instance ) { Instance = null; } }

	void Start( )
	{
		Assert.AreNotEqual( deck.Length, 0, $"Please assign <b>{nameof( deck )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( summoningPoint, $"Please assign <b>{nameof( summoningPoint )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( spawnIndicator, $"Please assign <b>{nameof( spawnIndicator )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		AddMana( startMana );
	}

	void Update( )
	{
		if ( startDelay > 0 )
			startDelay -= Time.deltaTime;

		if ( !IsPlaying || startDelay > 0 )
			return;

		ManaProgress( );
		TrySummon( );
	}

	private void TrySummon( )
	{
		// We wait before summons at lease a min. amount of time
		timeToNextSummon -= Time.deltaTime;
		if ( timeToNextSummon > 0 )
			return;

		// We draw cards to always have 3
		while ( hand.Count < 3 )
		{
			hand.Add( GetCardFromDeck( ) );
			hand = hand.OrderByDescending( card => card.SummonCost ).ToList( );
		}

		// Lets try to cast the most powerful card and if not then any card
		OponentDeckCard cardToSummon = null;
		if ( EnoughMana( hand[0].SummonCost ) )
			cardToSummon = hand[0];
		else if ( EnoughMana( hand[1].SummonCost ) )
			cardToSummon = hand[1];
		else if ( EnoughMana( hand[2].SummonCost ) )
			cardToSummon = hand[2];

		// No card to summon
		if ( cardToSummon == null )
			return;

		timeToNextSummon = delayBetweenCardPlays;
		StartCoroutine( Summon( cardToSummon, GetRandomPoint( ) ) );
	}

	private IEnumerator Summon( OponentDeckCard card, Vector2 pos )
	{
		// Show summoning spot
		Instantiate( spawnIndicator, pos, Quaternion.identity );

		// Wait for indicator to run it's animation
		yield return new WaitForSeconds( summonDelay );

		// Summon
		Instantiate( card.Instance, pos, Quaternion.identity );
		RemoveMana( card.SummonCost );
	}

	private bool EnoughMana( int amount ) => currentMana >= amount;

	private void AddMana( int amount )
	{
		currentMana += amount;
		//manaCounter.text = currentMana.ToString( );
		currentManaProgress = 0;
	}

	private void RemoveMana( int amount )
	{
		currentMana -= amount;
	}

	private void ManaProgress( )
	{
		currentManaProgress += Time.deltaTime / manaTickTime;

		if ( currentManaProgress < 1 )
			return;

		AddMana( manaTickAmount );
	}

	private Vector2 GetRandomPoint( )
	{
		return new Vector2( )
		{
			x = summoningPoint.transform.position.x + Random.Range( -summonArea.x, summonArea.x ),
			y = summoningPoint.transform.position.y + Random.Range( -summonArea.y, summonArea.y )
		};
	}

	void OnDrawGizmosSelected( )
	{
		Color col = Color.green;
		col.a = 0.3f;
		Gizmos.color = col;

		Gizmos.DrawCube( summoningPoint.transform.position, summonArea * 2 );

		Gizmos.color = Color.white;
	}

	private OponentDeckCard GetCardFromDeck( )
	{
		if ( drawQueue.Count <= 0 )
			drawQueue = NewRandomizedDrawQueue( );

		return drawQueue.Dequeue( );
	}

	private Queue<OponentDeckCard> NewRandomizedDrawQueue( )
	{
		return new Queue<OponentDeckCard>( deck.OrderBy( x => Random.Range( 0, 10000000 ) ).ToArray( ) );
	}
}
