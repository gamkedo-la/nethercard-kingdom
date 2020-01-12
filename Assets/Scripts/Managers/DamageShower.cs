/**
 * Description: Shows damage on objects based on their HP %.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class DamageShower : MonoBehaviour
{
	[SerializeField] private HP hp = null;
	[SerializeField] private float[] thresholds = null;
	[SerializeField] private GameObject[] states = null;
	[SerializeField] private float checkInverval = 1f;

	void Start ()
	{
		Assert.IsNotNull( hp, $"Please assign <b>{nameof( hp )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.AreNotEqual( thresholds.Length, 0, $"Please assign <b>{nameof( thresholds )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.AreNotEqual( states.Length, 0, $"Please assign <b>{nameof( states )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.AreEqual( thresholds.Length, states.Length, $"Both should be equal" );

		InvokeRepeating( nameof( Check ), checkInverval, checkInverval );
	}

	private void Check( )
	{
		float hpPercent = hp.CurrentHP / hp.MaxHP;

		for ( int i = 0; i < thresholds.Length; i++ )
		{
			if ( hpPercent < thresholds[i] )
			{
				states[i].SetActive( true );
			}
			else
			{
				return; // So we don't have to check the rest. Assumes good order in thresholds
			}
		}
	}
}
