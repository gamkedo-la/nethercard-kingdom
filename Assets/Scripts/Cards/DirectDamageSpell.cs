/**
 * Description: Spell that does damage to a single target.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class DirectDamageSpell : Spell
{
	[SerializeField] private float targetFreezTime = 0f;
	[SerializeField] private float targetShockTime = 0.2f;
	[SerializeField] private float shakeStrength = 1f;

	override public void Start ()
	{
		base.Start( );
		//Assert.IsNotNull( , $"Please assign <b>{nameof(  )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

	}

	override public void Update ()
	{
		base.Update( );
	}

	override public void SetTarget( Targetable target )
	{
		if ( !target )
			Debug.Log( $"Spell {name} has no target. Target probably has been destroyed." );

		base.SetTarget( target );

		HP hp = target.GetComponent<HP>( );
		hp.DoDamage( effectAmount, transform.position );

		if ( targetFreezTime > 0 )
		{
			Unit unit = target.GetComponent<Unit>( );
			unit.Freez( targetFreezTime, true );

			UnitVisuals unitVis = target.GetComponent<UnitVisuals>( );
			unitVis.Shocked( targetShockTime );
		}

		if ( shakeStrength > 0 )
			ScreenshakeManager.Instance.DoShake( shakeStrength );
	}
}
