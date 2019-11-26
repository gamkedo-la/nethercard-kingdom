/**
 * Description: Gives the ability to enable many components at once.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class EnableComponents : MonoBehaviour
{
	[SerializeField] private Behaviour[] toEnable = null;
	[SerializeField] private float autoEnableTime = 0;

	void Start( )
	{
		Assert.AreNotEqual( toEnable.Length, 0, $"Please assign <b>{nameof( toEnable )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		if ( autoEnableTime > 0 )
			Invoke( nameof( DoEnabling ), autoEnableTime );
	}

	public void DoEnabling( )
	{
		foreach ( var component in toEnable )
			component.enabled = true;
	}
}
