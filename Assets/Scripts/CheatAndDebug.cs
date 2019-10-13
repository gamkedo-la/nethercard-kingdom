/**
 * Description: Cheat and Debug functionality.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatAndDebug : MonoBehaviour
{
	private bool paused = false;

	void Update ()
	{
		if ( Input.GetKeyDown( KeyCode.R ) )
			SceneManager.LoadScene( 0 );

		if ( Input.GetKeyDown( KeyCode.P ) )
		{
			paused = !paused;
			Time.timeScale = paused ? 0f : 1f;
		}
	}
}
