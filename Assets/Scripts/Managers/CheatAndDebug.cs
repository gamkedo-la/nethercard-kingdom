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
	public bool AllowCheats { get { return allowCheats; } }

	[SerializeField] private bool showDebugInfo = false;
	[SerializeField] private bool allowCheats = true;
	[SerializeField] private int manaCheatAmount = 20;

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

		if ( AllowCheats && Input.GetKeyDown( KeyCode.M ) )
			AddMana( );
    }

	public void AddMana( )
	{
		if ( !AllowCheats )
			return;

		SummoningManager.Instance.AddMana( manaCheatAmount );
	}
}
