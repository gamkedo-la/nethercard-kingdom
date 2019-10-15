/**
 * Description: Spell that targets everything in range.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class AoeSpell : Spell
{
	[SerializeField] private float range = 2f;
	[SerializeField] private ConflicSide side = ConflicSide.All;

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

		if ( side == ConflicSide.All )
			DoEffect( UnitsManager.Instance.FindUnitsInRange( transform.position, range ) );
		else
			DoEffect( UnitsManager.Instance.FindUnitsInRange( transform.position, range, side ) );
	}

	virtual protected void DoEffect( Unit[] unitsInRange ) { }
}
