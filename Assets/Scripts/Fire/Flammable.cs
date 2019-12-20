/**
 * Description: Reacts to fire.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class Flammable : MonoBehaviour
{
	[SerializeField] private HP hp = null;
	[SerializeField] private ParticleSystem particles = null;
	[SerializeField] private float particlesEmmisionRate = 20;
	[SerializeField] private float damageTick = 1f;
	[SerializeField] private float burnTime = 3f;
	[SerializeField] private bool isBurning = false;

	private float damage;
	private float burnTimeRemaining = 0;
	private ParticleSystem.EmissionModule emission;

	void Start ()
	{
		Assert.IsNotNull( hp, $"Please assign <b>{nameof( hp )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( particles, $"Please assign <b>{nameof( particles )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		emission = particles.emission;
	}

	void OnEnable( )
	{
		if ( isBurning )
		{
			isBurning = false;
			Set( 1 );
		}
	}

	void Update ()
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
		//emission.rateOverTime = particlesEmmisionRate;
		var emission = particles.emission;
		emission.rateOverTime = particlesEmmisionRate;

		Invoke( nameof( DoDamage ), damageTick );
	}

	private void DoDamage( )
	{
		if ( burnTimeRemaining >= 0 )
		{
			hp.DoDamage( damage, transform.position );
			Invoke( nameof( DoDamage ), damageTick );
		}
		else
		{
			isBurning = false;
			//emission.rateOverTime = 0;
			var emission = particles.emission;
			emission.rateOverTime = 0;
		}
	}
}
