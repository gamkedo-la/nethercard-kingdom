/**
 * Description: Forces card data update.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;

public class ForceCardUpdate : MonoBehaviour
{
	void Start( )
	{
		GetComponent<CardAudioVisuals>( ).enabled = true;
		GetComponent<Card>( ).enabled = true;

		Invoke( nameof( Done ), 0.1f );
	}

	private void Done( )
	{
		GetComponent<CardAudioVisuals>( ).enabled = false;
		GetComponent<Card>( ).enabled = false;
	}
}
