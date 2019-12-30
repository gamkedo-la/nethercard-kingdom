/**
 * Description: Heals friendly units.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class Heal : Attack
{
	[SerializeField] private Unit unit = null;
	[SerializeField] private Transform sprite = null;

	[Header("Effect")]
	[SerializeField] private float distance = 2f;

	[Header("Attack animation")]
	[SerializeField] private float attackLength = 0.2f;
	[SerializeField] private float attackTime = 0.1f;
	[SerializeField] private float attackBackTime = 0.3f;

	private float timeToNextAttack = 0;
	private Vector2 oldSpritePos;
	private Vector2 newSpritePos;

	override protected void Start( )
	{
		base.Start( );

		Assert.IsNotNull( unit, $"Please assign <b>{nameof( unit )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( sprite, $"Please assign <b>{nameof( sprite )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	void Update( )
	{
		TryToHeal( );
	}

	private void TryToHeal( )
	{
		if ( Frozen )
			return;

		timeToNextAttack -= Time.deltaTime;

		if ( timeToNextAttack > 0 )
			return;

		var unitsInRange = UnitsManager.Instance.FindUnitsInRange( unit.transform.position, distance, ConflicSide.Enemy );
		if ( unitsInRange.Length <= 0 )
			return;

		Unit target = unitsInRange.OrderBy( u => u.HP.CurrentHP / u.HP.MaxHP ).First( );

		if ( !target || target.HP.CurrentHP == target.HP.MaxHP )
			return;

		target.HP.Heal( atackDamage );
		unit.Freez( 0, attackLength + atackDelay + attackBackTime );

		timeToNextAttack = atackDelay;
		if ( CheatAndDebug.Instance.ShowDebugInfo )
			Debug.DrawLine( unit.Center, target.Center, Color.red, 0.2f );

		animator.enabled = false;

		Vector3 moveDirection = target.Center - unit.Center;
		oldSpritePos = sprite.localPosition;
		newSpritePos = sprite.localPosition + moveDirection * attackLength;

		StartCoroutine( Utilities.ChangeOverTime( attackTime, Attack, StartMoveBack ) );
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
		StartCoroutine( Utilities.ChangeOverTime( attackBackTime, MoveBack, OnDoneAttack ) );
	}

	private void OnDoneAttack( )
	{
		animator.enabled = true;
		animator.SetTrigger( "Idle" );
	}
}
