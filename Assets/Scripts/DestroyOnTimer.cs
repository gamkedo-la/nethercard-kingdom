/**
 * Description: Destroys GameObject after x seconds.
 * Authors: Kornel
 * Copyright: © 2018-2019 Rebel Game Studio. All rights reserved. For license see: 'LICENSE' file.
 **/

using UnityEngine;

public class DestroyOnTimer : MonoBehaviour
{
	[SerializeField] private float destroyDelay = 10f;
	[SerializeField] private bool detachChildren = false;

	void Awake( )
	{
		Invoke( nameof(DestroyMe), destroyDelay );
	}

	private void DestroyMe( )
	{
		if ( detachChildren )
			transform.DetachChildren( );

		Destroy( gameObject );
	}
}
