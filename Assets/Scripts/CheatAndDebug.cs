/**
 * Description: Cheat and Debug functionality.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatAndDebug : MonoBehaviour
{
	void Update ()
	{
		if ( Input.GetKeyDown( KeyCode.R ) )
			SceneManager.LoadScene( 0 );

		if ( Input.GetKeyDown( KeyCode.P ) )
		{
			if ( Time.timeScale == 0f )
				Time.timeScale = 1f;
			else
				Time.timeScale = 0f;
		}
	}
}
