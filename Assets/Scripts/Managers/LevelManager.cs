/**
 * Description: Manages level state.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
	[SerializeField] private UnitsManager unitsManager = null;
	[SerializeField] private GameObject wonScreen = null;
	[SerializeField] private GameObject gameoverScreen = null;
	[SerializeField] private PlaySound wonSound = null;
	[SerializeField] private PlaySound gameoverSound = null;
	[SerializeField] private GameObject[] toDisableOnEnd = null;
	[SerializeField] private UnityEvent onGameEnd = null;

	void Start ()
	{
		Assert.IsNotNull( unitsManager, $"Please assign <b>{nameof( unitsManager )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( wonScreen, $"Please assign <b>{nameof( wonScreen )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( gameoverScreen, $"Please assign <b>{nameof( gameoverScreen )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( wonSound, $"Please assign <b>{nameof( wonSound )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( gameoverSound, $"Please assign <b>{nameof( gameoverSound )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	public void PlayerKilled( )
	{
		GameLost( );
	}

	public void OpponentKilled( )
	{
		GameWon( );
	}

	private void GameWon( )
	{
		wonScreen.SetActive( true );
		wonSound.Play( );

		EndGame( );
	}

	private void GameLost( )
	{
		gameoverScreen.SetActive( true );
		gameoverSound.Play( );

		EndGame( );
	}

	private void EndGame( )
	{
		foreach ( var item in toDisableOnEnd )
		{
			item.SetActive( false );
		}

		unitsManager.StopUnits( );
		onGameEnd?.Invoke( );
	}
}
