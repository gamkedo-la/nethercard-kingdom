/**
 * Description: Spell that heals a single target.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;

public class DirectHealingSpell : Spell
{
	override public void SetTarget( Targetable target )
	{
		if ( !target )
			Debug.Log( $"Spell {name} has no target. Target probably has been destroyed." );

		base.SetTarget( target );

		HP hp = target.GetComponent<HP>( );
		hp.Heal( effectAmount );
	}
}
