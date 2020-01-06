/**
 * Description: Manages the deck.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class DeckBuilder : MonoBehaviour
{
	public static DeckBuilder Instance { get; private set; }

	[Header("Objects")]
	[SerializeField] private CollectionManager collectionManager = null;
	[SerializeField] private Animator animator = null;
	[SerializeField] private TextMeshProUGUI tooltip = null;
	[SerializeField] private string sceneToLoadOnCloseDone = "Main";
	[SerializeField] private GameObject[] toHideOnClose = null;
	[SerializeField] private GameObject[] toShowOnClose = null;
	[SerializeField] private bool autoShow = false;

	private void Awake( )
	{
		if ( Instance != null && Instance != this )
			Destroy( this );
		else
			Instance = this;
	}

	private void OnDestroy( ) { if ( this == Instance ) { Instance = null; } }

	void Start( )
	{
		Assert.IsNotNull( collectionManager, $"Please assign <b>{nameof( collectionManager )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( tooltip, $"Please assign <b>{nameof( tooltip )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( animator, $"Please assign <b>{nameof( animator )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		if ( autoShow )
			Show( );
	}

	public void Show( )
	{
		animator.enabled = true;
	}

	public void Close( )
	{
		animator.SetTrigger( "Close" );

		foreach ( var go in toHideOnClose )
			go.SetActive( false );
	}

	public void OnDoneClose( )
	{
		animator.enabled = false;

		foreach ( var go in toShowOnClose )
			go.SetActive( true );

		SceneManager.LoadScene( sceneToLoadOnCloseDone );
	}

	public void ShowUpgrade( )
	{
		animator.SetTrigger( "ShowUpgrade" );
		collectionManager.UpgradingWindow( true );
	}

	public void HideUpgrade( )
	{
		animator.SetTrigger( "HideUpgrade" );
		collectionManager.UpgradingWindow( false );
	}

	public void OnTooltipWarning( )
	{
		animator.SetBool( "TooltipWarning", true );
		float tooltipWarningDuration = 0.3f;
		Invoke( nameof( OffTooltipWarning ), tooltipWarningDuration );
	}

	public void OffTooltipWarning( ) => animator.SetBool( "TooltipWarning", false );
}
