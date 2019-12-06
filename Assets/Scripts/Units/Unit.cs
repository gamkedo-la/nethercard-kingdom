/**
 * Description: Moves a Unit.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections;
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

	[Header("External objects")]
	[SerializeField] private HP hp = null;
	[SerializeField] private Attack attack = null;
	[SerializeField] private Animator animator = null;
	[SerializeField] private UnitVisuals visuals = null;

    [Header("Physical parameters")]
	[SerializeField] private ConflicSide side = ConflicSide.Player;
	[SerializeField] private Vector2 unitCenter = new Vector2(0f, 0.7f);
	[SerializeField] private bool HQ = false;

	[Header("Combat")]
	[SerializeField] private float detectionRange = 3f;
	[SerializeField] private float attackRange = 1f;

	[Header("Movement")]
	[SerializeField] private float moveSpeed = 2f;
	[SerializeField] private Vector2 movementRange = new Vector2(14f, 4f);
	[SerializeField] private Vector2 movementOffset = new Vector2(0, -1);

	[Header("Events")]
	[SerializeField] private UnitEvent onEnemyDetected = null;
	[SerializeField] private UnitEvent onEnemyInRange = null;

	private Vector2 moveDirection = Vector2.left;

	private Unit currentOpponent = null;
	private bool hadOponent = false;
	private bool inAttackRange = false;
	private bool frozen = false;

	void Start ()
	{
		Assert.IsNotNull( hp, $"Please assign <b>{nameof( hp )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( attack, $"Please assign <b>{nameof( attack )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( animator, $"Please assign <b>{nameof( animator )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( visuals, $"Please assign <b>{nameof( visuals )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		moveDirection = side == ConflicSide.Player ? Vector2.right : Vector2.left;
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
		if ( HQ )
			return;

		if ( frozen )
			return;

		SearchForOpenentToTarget( );
		SearchForOpenentToAttack( );
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
			animator.SetTrigger( "Idle" );
		}

		visuals.MoveDir( Vector2.zero, 0 ); // We are standing still
		inAttackRange = true;
	}

	private void Move( )
	{
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
