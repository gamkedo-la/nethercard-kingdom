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
	public bool ShowHPLabels { get { return showHPLabels; } set { showHPLabels = value; } }
	public bool ShowOpponentsMana { get { return showOpponentsMana; } set { showOpponentsMana = value; } }

	[Header("Objects")]
	[SerializeField] private PlayerCards playerCards = null;
	[SerializeField] private GameObject tooltip = null;
	[SerializeField] private GameObject gameboyModeLayer = null;

	[Header("Debug Flags")]
	[Tooltip("Allow cheat codes")]
	[SerializeField] private bool allowCheats = false;
	[Tooltip("Show debug informations")]
	[SerializeField] private bool showDebugInfo = false;
	[SerializeField] private bool showHPLabels = false;
	[SerializeField] private bool showOpponentsMana = true;

	[Header("Debug Parameters")]
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
		if ( Input.GetKeyDown( KeyCode.G ) )
			gameboyModeLayer.SetActive( !gameboyModeLayer.activeSelf );

		// Everything below only available if cheats are enabled
		if ( !AllowCheats )
			return;

		if ( Input.GetKeyDown( KeyCode.M ) )
			AddMana( );

		if ( Input.GetKeyDown( KeyCode.LeftShift ) && Input.GetKeyDown( KeyCode.LeftControl ) && Input.GetKeyDown( KeyCode.R ) )
			playerCards.RemovdSavedPlayerCardsData( );

		if ( Input.GetKeyDown( KeyCode.R ) )
			SceneManager.LoadScene( 0 );

		if ( Input.GetKeyDown( KeyCode.T ) )
			tooltip.SetActive( !tooltip.activeSelf );
	}

	public void AddMana( )
	{
		if ( !AllowCheats )
			return;

		SummoningManager.Instance.AddMana( manaCheatAmount );
	}
}
