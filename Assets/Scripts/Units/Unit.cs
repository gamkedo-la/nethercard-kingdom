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
	[SerializeField] private float detectionRange = 3f;
	[SerializeField] private float minRange = 0.3f;
	[SerializeField] private float attackRange = 1f;

	[Header("Movement")]
	[SerializeField] private float moveSpeed = 2f;
	[SerializeField] private Vector2 movementRange = new Vector2(14f, 4f);
	[SerializeField] private Vector2 movementOffset = new Vector2(0, -1);
	[SerializeField] private AnimationCurve movementSpringSameSide = new AnimationCurve ( new Keyframe(0,0), new Keyframe(1,1));
	[SerializeField] private AnimationCurve movementSpringOppositeSide = new AnimationCurve ( new Keyframe(0,0), new Keyframe(1,1));

	[Header("Events")]
	[SerializeField] private UnitEvent onEnemyDetected = null;
	[SerializeField] private UnitEvent onEnemyInRange = null;

	private Vector2 moveDirection = Vector2.left;

	private Unit currentOpponent = null;
	private bool hadOponent = false;
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
		if ( UnitsManager.Instance )
			UnitsManager.Instance.RemoveUnit( this, side );
	}

	void Update ()
	{
		if ( hq )
			return;

		if ( frozen )
			return;

		//SearchForOpenentToTarget( );
		//SearchForOpenentToAttack( );
		CalculateMoveVector( );
		Move( );
	}

	void OnDrawGizmosSelected( )
	{
		Color col = Color.green;
		col.a = 0.3f;
		Gizmos.color = col;
		Gizmos.DrawCube( movementOffset, movementRange );

		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere( transform.position + (Vector3)unitCenter, detectionRange );

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere( transform.position + (Vector3)unitCenter, attackRange );

		Gizmos.color = Color.white;
	}

	void OnDrawGizmos( )
	{
		Color color = Color.blue;
		color.a = 1.0f;
		Gizmos.color = color;

		Gizmos.DrawRay( Center, moveDirection );

		/*foreach ( var spring in springs )
		{
			float lineThickness = spring.Strenght;

			color = spring.Positive ? Color.green : Color.red;
			color.a = lineThickness;
			Gizmos.color = color;

			Gizmos.DrawLine( Center, spring.EndPosition );
		}*/

		/*foreach ( var spring in springs )
		{
			float lineThickness = spring.Strenght;

			color = spring.Positive ? Color.green : Color.red;
			//color.a = lineThickness;
			//Gizmos.color = color;

			spring.Line = Utilities.DrawDebugLine( Center, spring.EndPosition, color, lineThickness, lineThickness );
		}*/

		Gizmos.color = Color.white;
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

	private void SearchForOpenentToTarget( )
	{
		// Go to the current oponent if we still have one
		if ( currentOpponent )
		{
			//  Calculate new move direction
			moveDirection = currentOpponent.Center - Center;
			if ( CheatAndDebug.Instance.ShowDebugInfo )
				Debug.DrawLine( Center, Center + (Vector3)moveDirection, Color.blue );

			if ( Vector2.Distance( currentOpponent.Center, Center ) < minRange )
				moveDirection = Vector2.zero;
			else
				moveDirection.Normalize( );

			return;
		}

		// We do not have an oponent or it became lost
		Unit newOponent = UnitsManager.Instance.FindOponent( side, Center, detectionRange );

		// Lost an oponent
		if ( !newOponent && hadOponent )
		{
			if ( CheatAndDebug.Instance.ShowDebugInfo )
				Debug.Log( $"{name} lost oponent" );

			currentOpponent = null;
			onEnemyDetected.Invoke( null );
			hadOponent = false;
			inAttackRange = false;
			moveDirection = side == ConflicSide.Player ? Vector2.right : Vector2.left;

			return;
		}

		// Detected new oponent
		if ( newOponent && !hadOponent )
		{
			if ( CheatAndDebug.Instance.ShowDebugInfo )
				Debug.Log( $"{name} detected oponent: {newOponent.name}" );

			currentOpponent = newOponent;
			hadOponent = true;
			inAttackRange = false;
			onEnemyDetected.Invoke( newOponent );
		}

		//  Calculate new move direction
		if ( newOponent )
		{
			moveDirection = newOponent.Center - Center;
			if ( CheatAndDebug.Instance.ShowDebugInfo )
				Debug.DrawLine( Center, Center + (Vector3)moveDirection, Color.blue );
			moveDirection.Normalize( );
		}
	}

	private void SearchForOpenentToAttack( )
	{
		// We have no target
		if ( currentOpponent == null )
		{
			// We lost target
			if ( inAttackRange )
			{
				if ( CheatAndDebug.Instance.ShowDebugInfo )
					Debug.Log( $"{name} lost attack target" );

				onEnemyInRange.Invoke( null );
			}

			if ( !nonMovable )
				animator.SetTrigger( "Moving" );

			inAttackRange = false;

			return;
		}

		// Oponent out of range
		if ( Vector2.Distance( Center, currentOpponent.Center ) > attackRange )
			return;

		// We passed all checks, we are in attack range

		// We got a new oponent
		if ( !inAttackRange )
		{
			if ( CheatAndDebug.Instance.ShowDebugInfo )
				Debug.Log( $"{name} attacking: {currentOpponent.name}" );

			onEnemyInRange.Invoke( currentOpponent );

			if ( !nonMovable )
				animator.SetTrigger( "Idle" );
		}

		if ( !nonMovable )
			visuals.MoveDir( Vector2.zero, 0 ); // We are standing still
		inAttackRange = true;
	}

	private void CalculateMoveVector( )
	{
		// Default move direction
		moveDirection = side == ConflicSide.Player ? Vector2.right : Vector2.left;
		moveDirection /= 10;

		// For static units
		if ( nonMovable )
			return;

		// We are in attack range, we should not move
		if ( inAttackRange )
			return;

		foreach ( var item in springs )
			Destroy( item.gameObject );
		springs.Clear( );

		foreach ( var unit in UnitsManager.Instance.PlayerUnits )
			moveDirection += CalculateSpring( unit );

		foreach ( var unit in UnitsManager.Instance.EnemyUnits )
			moveDirection += CalculateSpring( unit );

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

		// OPTION: Discard if length less then X?

		// Debug lines (temp. code)
		float lineThickness = strenght / 30;
		bool positive = strenght >= 0;
		Color color = positive ? Color.green : Color.red;
		color.a = 0.9f;
		LineRenderer line = Utilities.DrawDebugLine( Center, unit.Center, color, lineThickness, lineThickness );
		line.sortingLayerName = "Foreground";
		springs.Add( line );

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
