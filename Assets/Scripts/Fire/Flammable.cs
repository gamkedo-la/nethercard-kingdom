/**
 * Description: Reacts to fire.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class Flammable : MonoBehaviour
{
	[Header("Objects")]
	[SerializeField] private HP hp = null;
	[SerializeField] private ParticleSystem particles = null;

	[Header("Self Burning")]
	[SerializeField] private float particlesEmmisionRate = 20;
	[SerializeField] private float damageTick = 1f;
	[SerializeField] private float burnTime = 3f;
	[SerializeField] private float damage = 1f;
	[SerializeField] private bool isBurning = false;

	[Header("Burning Other")]
	[SerializeField] private float tryBurnTick = 0.5f;
	[SerializeField] private float burnChance = 0.7f;
	[SerializeField] private float burnRadius = 0.8f;

	private float burnTimeRemaining = 0;

	void Start( )
	{
		//Assert.IsNotNull( hp, $"Please assign <b>{nameof( hp )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		//Assert.IsNotNull( particles, $"Please assign <b>{nameof( particles )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	void OnEnable( )
	{
		if ( isBurning )
		{
			burnTimeRemaining = burnTime;

			if ( particles )
			{
				var emission = particles.emission;
				emission.rateOverTime = particlesEmmisionRate;
			}

			DoDamage( );
			TryBurn( );
		}
	}

	void Update( )
	{
		if ( isBurning )
			burnTimeRemaining -= Time.deltaTime;
	}

	public void Set( float damage )
	{
		if ( isBurning )
			return;

		this.damage = damage;
		isBurning = true;
		burnTimeRemaining = burnTime;

		if ( particles )
		{
			var emission = particles.emission;
			emission.rateOverTime = particlesEmmisionRate;
		}

		Invoke( nameof( DoDamage ), damageTick );
		Invoke( nameof( TryBurn ), tryBurnTick * 4 );
	}

	private void DoDamage( )
	{
		if ( burnTimeRemaining >= 0 )
		{
			hp?.DoDamage( damage, transform.position );
			Invoke( nameof( DoDamage ), damageTick );
		}
		else
		{
			isBurning = false;

			if ( particles )
			{
				var emission = particles.emission;
				emission.rateOverTime = 0;
			}
		}
	}

	private void TryBurn( )
	{
		if ( !isBurning )
			return;

		RaycastHit2D[] hits = Physics2D.CircleCastAll( transform.position, burnRadius, Vector2.zero, 0f );
		foreach ( var hit in hits )
		{
			Flammable other = hit.transform.gameObject.GetComponent<Flammable>( );
			if ( other && Random.Range( 0f, 1f ) <= burnChance )
				other.Set( damage );
		}

		Invoke( nameof( TryBurn ), tryBurnTick );
	}
}
