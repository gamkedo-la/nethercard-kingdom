/**
 * Description: Spell that heals a single target.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class DirectHealingSpell : Spell
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

	public override void SetTarget( Targetable target )
	{
		base.SetTarget( target );

		HP hp = target.GetComponent<HP>( );
		hp.Heal( effectAmount );
	}
}
