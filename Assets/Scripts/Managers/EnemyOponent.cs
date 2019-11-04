/**
 * Description: Enemy player.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class OponentDeckCard
{
	public GameObject Instance;
	public int SummonCost;
}

public class EnemyOponent : MonoBehaviour
{
	public static EnemyOponent Instance { get; private set; }
	public bool IsPlaying { get; set; } = true;

	[SerializeField] private OponentDeckCard[] deck = null;
	[SerializeField] private GameObject spawnIndicator = null;
	[SerializeField] private GameObject summoningPoint = null;
	[SerializeField] private Vector2 summonArea = new Vector2(2f, 4f);
	[SerializeField] private float startDelay = 5f;
	[SerializeField] private float summonInterval = 5f;
	[SerializeField] private float summonDelay = 0.5f;

	private float timeToNextSummon = 0f;

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

		timeToNextSummon = startDelay;
	}

	void Update( )
	{
		if ( !IsPlaying )
			return;

		TrySummon( );
	}

	private void TrySummon( )
	{
		timeToNextSummon -= Time.deltaTime;
		if ( timeToNextSummon > 0 )
			return;

		timeToNextSummon = summonInterval;

		StartCoroutine( Summon( deck[Random.Range( 0, deck.Length )].Instance, GetRandomPoint( ) ) );
	}

	private IEnumerator Summon( GameObject instance, Vector2 pos )
	{
		// Show summoning spot
		Instantiate( spawnIndicator, pos, Quaternion.identity );

		// Wait
		yield return new WaitForSeconds( summonDelay );

		// Summon
		Instantiate( instance, pos, Quaternion.identity );
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
}
