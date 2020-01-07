/**
 * Description: A standard Unit attack.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class BeamAttack : Attack
{
	[SerializeField] private LineRenderer line = null;
	[SerializeField] private Transform startPoint = null;
	[SerializeField] protected float atackDuration = 0.3f;

	private float timeToNextAttack = 0;
	private Unit currentOpponent = null;
	private Color color;

	override protected void Start( )
	{
		base.Start( );

		Assert.IsNotNull( line, $"Please assign <b>{nameof( line )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( startPoint, $"Please assign <b>{nameof( startPoint )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		color = line.startColor;
		line.enabled = false;
	}

	void Update( )
	{
		TryToAttack( );
	}

	public void OnNewOponent( Unit newOponent ) => currentOpponent = newOponent;

	private void TryToAttack( )
	{
		if ( Frozen )
			return;

		timeToNextAttack -= Time.deltaTime;

		if ( attackSound )
			attackSound.Play( );

		// Needs to be in attack range of an oponent and have no attack cool-down
		if ( !currentOpponent || timeToNextAttack > 0 )
			return;

		line.enabled = true;

		currentOpponent.HP.DoDamage( atackDamage, currentOpponent.Center );
		timeToNextAttack = atackDelay;

		StartCoroutine( Utilities.ChangeOverTime( atackDuration / 2, AttackFirstHalf, StartSecondHalf ) );
	}

	private void AttackFirstHalf( float percent )
	{
		if ( !currentOpponent )
		{
			StopAllCoroutines( );
			StartSecondHalf( );

			return;
		}

		color.a = percent;

		line.SetPosition( 0, startPoint.position );
		line.SetPosition( 1, currentOpponent.Center );
		line.startColor = color;
		line.endColor = color;
	}

	private void StartSecondHalf( )
	{
		StartCoroutine( Utilities.ChangeOverTime( atackDuration / 2, AttackSecondHalf, EndAttack ) );
	}

	private void AttackSecondHalf( float percent )
	{
		color.a = 1f - percent;

		line.SetPosition( 0, startPoint.position );
		if( currentOpponent ) line.SetPosition( 1, currentOpponent.Center );
		line.startColor = color;
		line.endColor = color;
	}

	private void EndAttack( )
	{
		line.enabled = false;
	}
}
