/**
 * Description: A standard Unit attack.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class StandardAttack : Attack
{
	[SerializeField] private Unit unit = null;
	[SerializeField] private Transform sprite = null;

	[Header("Attack animation")]
	[SerializeField] private float attackLength = 0.2f;
	[SerializeField] private float attackTime = 0.1f;
	[SerializeField] private float attackBackTime = 0.3f;

	private float timeToNextAttack = 0;
	private Vector2 oldSpritePos;
	private Vector2 newSpritePos;
	private Unit currentOpponent = null;

	override protected void Start ()
	{
		base.Start( );

		Assert.IsNotNull( unit, $"Please assign <b>{nameof( unit )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( sprite, $"Please assign <b>{nameof( sprite )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	void Update ()
	{
		TryToAttack( );
	}

	public void OnNewOponent( Unit newOponent ) => currentOpponent = newOponent;

	private void TryToAttack( )
	{
		if ( Frozen )
			return;

		timeToNextAttack -= Time.deltaTime;

		// Needs to be in attack range of an oponent and have no attack cool-down
		if ( !currentOpponent || timeToNextAttack > 0 )
			return;

		currentOpponent.HP.DoDamage( atackDamage, currentOpponent.Center );
		timeToNextAttack = atackDelay;

		if ( attackSound )
			attackSound.Play( );

		if ( CheatAndDebug.Instance.ShowDebugInfo )
			Debug.DrawLine( unit.Center, currentOpponent.Center, Color.red, 0.2f );

		animator.enabled = false;

		Vector3 moveDirection = currentOpponent.Center - unit.Center;
		oldSpritePos = sprite.localPosition;
		newSpritePos = sprite.localPosition + moveDirection * attackLength;

		StartCoroutine( Utils.ChangeOverTime( attackTime, Attack, StartMoveBack ) );
	}

	private void Attack( float percent )
	{
		sprite.localPosition = Vector2.Lerp( oldSpritePos, newSpritePos, percent );
	}

	private void MoveBack( float percent )
	{
		sprite.localPosition = Vector2.Lerp( newSpritePos, oldSpritePos, percent );
	}

	private void StartMoveBack( )
	{
		StartCoroutine( Utils.ChangeOverTime( attackBackTime, MoveBack, OnDoneAttack ) );
	}

	private void OnDoneAttack( )
	{
		animator.enabled = true;
		animator.SetTrigger( "Idle" );
	}
}
