/**
 * Description: Does damage in a set range.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class OffensiveAoeSpell : AoeSpell
{
	override public void Start( )
	{
		base.Start( );
		//Assert.IsNotNull( , $"Please assign <b>{nameof(  )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

	}

	override public void Update( )
	{
		base.Update( );
	}

	override public void SetTarget( Targetable target )
	{
		base.SetTarget( target );
	}

	override protected void DoEffect( Unit[] unitsInRange )
	{
		foreach ( var unit in unitsInRange )
			unit.HP.DoDamage( effectAmount, transform.position );
	}
}
