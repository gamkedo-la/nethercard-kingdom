/**
 * Description: Main functionality of a Spell Instance.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class Spell : MonoBehaviour
{
	[SerializeField] protected CardType type = CardType.DirectOffensiveSpell;
	[SerializeField] private GameObject optionalToSpawn = null;
	[SerializeField] private float destroyAfter = 0.3f;
	[SerializeField] protected float effectAmount = 1f;

	virtual public void Start ()
	{
		//Assert.IsNotNull( , $"Please assign <b>{nameof(  )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

	}

	virtual public void Update ()
	{

	}

	virtual public void SetTarget( Targetable target )
	{
		if ( optionalToSpawn )
			Instantiate( optionalToSpawn, transform.position, Quaternion.identity );

		Invoke( nameof( DestroyMe ), destroyAfter );
	}

	private void DestroyMe()
	{
		Destroy( gameObject );
	}
}
