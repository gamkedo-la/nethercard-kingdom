/**
 * Description: Does damage in a set range.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

public class OffensiveAoeSpell : AoeSpell
{
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
