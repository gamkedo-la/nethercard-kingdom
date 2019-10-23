/**
 * Description: Cheat and Debug functionality.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatAndDebug : MonoBehaviour
{
	public static CheatAndDebug Instance { get; private set; }
	public bool ShowDebugInfo { get { return showDebugInfo; } }
	[SerializeField] private bool showDebugInfo = true;

	private bool paused = false;

	private void Awake( )
	{
		if ( Instance != null && Instance != this )
			Destroy( this );
		else
			Instance = this;
	}

	private void OnDestroy( ) { if ( this == Instance ) { Instance = null; } }

	void Update ()
	{
		if ( Input.GetKeyDown( KeyCode.R ) )
			SceneManager.LoadScene( 0 );

		/*if ( Input.GetKeyDown( KeyCode.P ) )
		{
			paused = !paused;
			Time.timeScale = paused ? 0f : 1f;
		}*/

        if (Input.GetKeyDown(KeyCode.M))
        {
            SummoningManager.Instance.AddMana(5);
        }
    }
}
