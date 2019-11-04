/**
 * Description: Main functionality of a Spell Instance.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;

public class Spell : MonoBehaviour
{
	[SerializeField] private GameObject optionalToSpawn = null;
	[SerializeField] protected CardType type = CardType.DirectOffensiveSpell;
	[SerializeField] protected float effectAmount = 1f;
	[SerializeField] private float destroyAfter = 0.3f;

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
