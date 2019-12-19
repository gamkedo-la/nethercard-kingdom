/**
 * Description: Moves a Unit.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

[System.Serializable]
public class UnitEvent : UnityEvent<Unit> { }

public class Unit : MonoBehaviour
{
	public Vector3 Center { get { return transform.position + (Vector3)unitCenter; } }
	public HP HP { get { return hp; } }
	public ConflicSide Side { get { return side; } }
	public float MoveSpeed { get { return moveSpeed; } }
	public float DPS { get { return attack.DPS; } }
	public bool HQ { get { return hq; } }

	[Header("External objects")]
	[SerializeField] private HP hp = null;
	[SerializeField] private Attack attack = null;
	[SerializeField] private Animator animator = null;
	[SerializeField] private UnitVisuals visuals = null;

    [Header("Physical parameters")]
	[SerializeField] private ConflicSide side = ConflicSide.Player;
	[SerializeField] private Vector2 unitCenter = new Vector2(0f, 0.7f);
	[SerializeField] private bool hq = false;
	[SerializeField] private bool nonMovable = false;

	[Header("Combat")]
	[SerializeField] private float attackRange = 1f;

	[Header("Movement")]
	[SerializeField] private float moveSpeed = 2f;
	[SerializeField] private Vector2 movementRange = new Vector2(14f, 4f);
	[SerializeField] private Vector2 movementOffset = new Vector2(0, -1);
	[SerializeField] private AnimationCurve movementSpringSameSide = new AnimationCurve ( new Keyframe(0,0), new Keyframe(1,1));
	[SerializeField] private AnimationCurve movementSpringOppositeSide = new AnimationCurve ( new Keyframe(0,0), new Keyframe(1,1));

	[Header("Other")]
	[SerializeField] private bool debugMode = false;

	[Header("Events")]
	[SerializeField] private UnitEvent onEnemyInRange = null;

	private Vector2 moveDirection = Vector2.left;

	private Unit currentOpponent = null;
	private bool inAttackRange = false;
	private bool frozen = false;

	private List<LineRenderer> springs = new List<LineRenderer>();

	void Start ()
	{
		Assert.IsNotNull( hp, $"Please assign <b>{nameof( hp )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( attack, $"Please assign <b>{nameof( attack )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( animator, $"Please assign <b>{nameof( animator )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( visuals, $"Please assign <b>{nameof( visuals )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	void OnEnable( )
	{
		if ( UnitsManager.Instance )
			UnitsManager.Instance.AddUnit( this, side );
	}

	void OnDisable( )
	{
		// Clean up debug lines
		foreach ( var item in springs )
			if ( item != null )
				Destroy( item.gameObject );

		if ( UnitsManager.Instance )
			UnitsManager.Instance.RemoveUnit( this, side );
	}

	void Update ()
	{
		if ( hq )
			return;

		if ( frozen )
			return;

		TryFindAttackTarget( );
		CalculateMoveVector( );
		Move( );
	}

	void OnDrawGizmosSelected( )
	{
		Color col = Color.green;
		col.a = 0.1f;
		Gizmos.color = col;
		Gizmos.DrawCube( movementOffset, movementRange );

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere( transform.position + (Vector3)unitCenter, attackRange );

		Gizmos.color = Color.white;

		debugMode = true;
	}

	public void Stop( )
	{
		if ( animator.enabled )
			animator.SetTrigger( "Idle" );

		frozen = true;
		attack.Frozen = true;
	}

	public void Freez( float shockDuration, float afterFreezDuration )
	{
		StartCoroutine( DoFreez( shockDuration, afterFreezDuration ) );
	}

	private IEnumerator DoFreez( float shockDuration, float afterFreezDuration )
	{
		frozen = true;
		attack.Frozen = true;
		animator.SetTrigger( "Shocked" );
		yield return new WaitForSeconds( shockDuration );

		animator.SetTrigger( "Shocked" ); // For some reason it goes in to Idle the Shock animation is not triggered a 2nd time
		yield return new WaitForSeconds( afterFreezDuration );

		frozen = false;
		attack.Frozen = false;
		animator.SetTrigger( "Moving" );
	}

	private void TryFindAttackTarget( )
	{
		Unit newOponent = UnitsManager.Instance.FindOpponent( side, Center, attackRange );

		// No current target
		if ( !currentOpponent )
		{
			// We lost target
			if ( inAttackRange )
			{
				if ( CheatAndDebug.Instance.ShowDebugInfo )
					Debug.Log( $"{name} lost attack target" );

				onEnemyInRange.Invoke( null );
				inAttackRange = false;
			}

			// New target found
			if ( newOponent )
			{
				onEnemyInRange.Invoke( newOponent );
				inAttackRange = true;

				if ( !nonMovable )
					animator.SetTrigger( "Idle" );

				return;
			}

			if ( !nonMovable )
				animator.SetTrigger( "Moving" );

			return;
		}
	}

	private void CalculateMoveVector( )
	{
		// Static units (like a wall or castle)
		if ( nonMovable )
			return;

		// Default move direction
		moveDirection = side == ConflicSide.Player ? Vector2.right : Vector2.left;
		moveDirection /= 10;

		// We are in attack range, we should not move
		if ( inAttackRange )
			return;

		// Clear debug line list
		foreach ( var item in springs )
			Destroy( item.gameObject );
		springs.Clear( );

		// Get all the spring vectors
		foreach ( var unit in UnitsManager.Instance.PlayerUnits )
			moveDirection += CalculateSpring( unit );

		foreach ( var unit in UnitsManager.Instance.EnemyUnits )
			moveDirection += CalculateSpring( unit );

		// Get just the direction info
		moveDirection.Normalize( );
	}

	private Vector2 CalculateSpring( Unit unit )
	{
		// Spring with initial direction and distance
		Vector2 spring = unit.Center - Center;

		// Spring's length (strength)
		float strenght = unit.Side == side ? movementSpringSameSide.Evaluate( spring.magnitude ) : movementSpringOppositeSide.Evaluate( spring.magnitude );
		strenght = unit.Side == side && unit.HQ ? 0 : strenght; // Disregard friendly HQ
		spring = spring.normalized * strenght;

		// OPTIONAL: Discard if length less then X?

		// Draw debug lines
		if ( debugMode )
		{
			float lineThickness = strenght / 30;
			bool positive = strenght >= 0;
			Color color = positive ? Color.green : Color.red;
			color.a = 0.9f;
			LineRenderer line = Utilities.DrawDebugLine( Center, unit.Center, color, lineThickness, lineThickness );
			line.sortingLayerName = "Foreground";
			springs.Add( line );
		}

		return spring;
	}

	private void Move( )
	{
		// For static units
		if ( nonMovable )
			return;

		// We are in attack range, we should not move
		if ( inAttackRange )
			return;

		Vector2 newPosition = transform.position + (Vector3)moveDirection * moveSpeed * Time.deltaTime;
		visuals.MoveDir( moveDirection, moveSpeed );

		// Restrict movement
		float maxX = movementOffset.x + movementRange.x / 2;
		float minX = movementOffset.x - movementRange.x / 2;
		float maxY = movementOffset.y + movementRange.y / 2;
		float minY = movementOffset.y - movementRange.y / 2;
		newPosition.x = Mathf.Clamp( newPosition.x, minX, maxX );
		newPosition.y = Mathf.Clamp( newPosition.y, minY, maxY );

		transform.position = newPosition;
	}
}
