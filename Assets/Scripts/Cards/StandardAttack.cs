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

	private float timeToNextAttack = 0;
	private Vector2 oldSpritePos;
	private Vector2 newSpritePos;
	private Unit currentOpponent = null;

	void Start ()
	{
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
		timeToNextAttack -= Time.deltaTime;

		// Need to be in attack range of an oponent and has no attack cool-down
		if ( !currentOpponent || timeToNextAttack > 0 )
			return;

		currentOpponent.HP.DoDamage( atackDamage, currentOpponent.Center );
		timeToNextAttack = atackDelay;
		if ( CheatAndDebug.Instance.ShowDebugInfo )
			Debug.DrawLine( unit.Center, currentOpponent.Center, Color.red, 0.2f );

		Vector3 moveDirection = currentOpponent.Center - unit.Center;
		oldSpritePos = sprite.localPosition;
		newSpritePos = sprite.localPosition + moveDirection * 0.2f;
		sprite.localPosition = newSpritePos;
		StartCoroutine( Utilities.ChangeOverTime( 0.3f, MoveBack ) );
	}

	private void MoveBack( float percent )
	{
		sprite.localPosition = Vector2.Lerp( newSpritePos, oldSpritePos, percent );
	}
}
