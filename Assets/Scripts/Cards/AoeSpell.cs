/**
 * Description: Spell that targets everything in range.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;

public class AoeSpell : Spell
{
	[SerializeField] private float range = 2f;
	[SerializeField] private ConflicSide side = ConflicSide.All;

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
