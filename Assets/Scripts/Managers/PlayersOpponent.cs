/**
 * Description: Enemy player.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class SummonEvent
{
	public GameObject[] Instances;
	public int SummonCost = 10;
	public float SummonCostVariance = 0.1f;
	public int EnrageMin = 0;
	public int EnrageMax = 100;
	public int Weight = 1;
}

public class PlayersOpponent : MonoBehaviour
{
	public static PlayersOpponent Instance { get; private set; }
	public bool IsPlaying { get; set; } = true;

	[Header("Deck")]
	[SerializeField] private SummonEvent[] summonEvents = null;

	[Header("Objects")]
	[SerializeField] private GameObject spawnIndicator = null;
	[SerializeField] private GameObject summoningPoint = null;

	[Header("Summoning")]
	[SerializeField] private Vector2 summonArea = new Vector2(2f, 4f);
	[SerializeField] private float summonDelay = 0.5f;
	[SerializeField] private float delayBetweenSummonsInOneEvent = 1f;
	[SerializeField, Range(0,100)] private float enrageMeter = 0f;
	[SerializeField, Range(0,200)] private float mana = 0f;
	[SerializeField, Range(0,30)] private float manaGainSpeed = 1f;

	private SummonEvent nextSummonEventToPlay = null;

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
		Assert.AreNotEqual( summonEvents.Length, 0, $"Please assign <b>{nameof( summonEvents )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( summoningPoint, $"Please assign <b>{nameof( summoningPoint )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( spawnIndicator, $"Please assign <b>{nameof( spawnIndicator )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	void Update( )
	{
		if ( !IsPlaying )
			return;

		mana += manaGainSpeed * Time.deltaTime;
		TryToSummon( );
	}

	private void TryToSummon( )
	{
		// Pick new event if we have non
		if ( nextSummonEventToPlay == null )
			nextSummonEventToPlay = PickNextSummonEvent( );

		// Do we have enough mana?
		if ( nextSummonEventToPlay.SummonCost > mana )
			return;

		if ( CheatAndDebug.Instance.ShowDebugInfo )
		{
			string s = "";
			foreach ( var u in nextSummonEventToPlay.Instances )
			{
				s += $"{u.name} ";
			}
			Debug.Log( $"Summoning: {s}" );
		}

		// Start summoning
		StartCoroutine( SummonEvent( nextSummonEventToPlay ) );
		nextSummonEventToPlay = null;
	}

	private IEnumerator SummonEvent( SummonEvent summonEvent )
	{
		mana -= nextSummonEventToPlay.SummonCost;

		// Summon every unit included in the event
		foreach ( var instance in summonEvent.Instances )
		{
			StartCoroutine( SummonUnit( instance, GetRandomPoint( ) ) );

			yield return new WaitForSeconds( delayBetweenSummonsInOneEvent ); // So the units aren't summoned all at the same time
		}
	}

	private IEnumerator SummonUnit( GameObject instance, Vector2 pos )
	{
		// Show summoning spot
		Instantiate( spawnIndicator, pos, Quaternion.identity );

		// Wait
		yield return new WaitForSeconds( summonDelay );

		// Summon
		Instantiate( instance, pos, Quaternion.identity );
	}

	private SummonEvent PickNextSummonEvent( )
	{
		SummonEvent summonEvent = null;

		// Summon events that meet the requirements
		var validChoices = summonEvents.Where( e => enrageMeter >= e.EnrageMin && enrageMeter <= e.EnrageMax ).ToList( );
		if ( CheatAndDebug.Instance.ShowDebugInfo )
		{
			string s = "Valid Choices: ";
			foreach ( var item in validChoices )
			{
				s += "[";
				foreach ( var u in item.Instances )
				{
					s += $"{u.name} ";
				}
				s += "] ";
			}
			Debug.Log( s );
		}

		// Sum of all valid choices' weights
		float weightSum = 0;
		foreach ( var item in validChoices )
			weightSum += item.Weight;

		float weigthPoint = Random.Range( 0, weightSum );
		if ( CheatAndDebug.Instance.ShowDebugInfo )
		Debug.Log( $"weightSum: {weightSum}, weigthPoint: {weigthPoint}" );

		// Pick a random event from all the valid ones
		// using weights - that way some have more probability to appear then others
		weightSum = 0;
		foreach ( var item in validChoices )
		{
			weightSum += item.Weight;
			if ( weightSum >= weigthPoint )
			{
				// Found next summon event
				summonEvent = item;
				if ( CheatAndDebug.Instance.ShowDebugInfo )
				{
					string s = "Next Event: ";
					foreach ( var u in item.Instances )
					{
						s += $"{u.name} ";
					}
					Debug.Log( s );
				}
				break;
			}
		}

		return summonEvent;
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
