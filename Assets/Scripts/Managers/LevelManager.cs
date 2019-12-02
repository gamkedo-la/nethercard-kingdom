﻿/**
 * Description: Manages level state.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI gameSpeedLabel = null;
	[SerializeField] private UnitsManager unitsManager = null;
	[SerializeField] private GameObject wonScreen = null;
	[SerializeField] private GameObject gameoverScreen = null;
	[SerializeField] private PlaySound wonSound = null;
	[SerializeField] private PlaySound gameoverSound = null;
	[SerializeField] private GameObject[] toDisableOnEnd = null;
	[SerializeField] private UnityEvent onGameEnd = null;

	void Start ()
	{
		Assert.IsNotNull( gameSpeedLabel, $"Please assign <b>{nameof( gameSpeedLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( unitsManager, $"Please assign <b>{nameof( unitsManager )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( wonScreen, $"Please assign <b>{nameof( wonScreen )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( gameoverScreen, $"Please assign <b>{nameof( gameoverScreen )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( wonSound, $"Please assign <b>{nameof( wonSound )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( gameoverSound, $"Please assign <b>{nameof( gameoverSound )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		SetGameSpeed( 3.0f ); // Normal speed
	}

	public void SetGameSpeed( float value )
	{
		float speed = 0;
		string label = "";

		switch ( (int)value )
		{
			case 0:
			speed = 0.1f;
			label = "0.1x Speed";
			break;

			case 1:
			speed = 0.25f;
			label = "0.25x Speed";
			break;

			case 2:
			speed = 0.5f;
			label = "0.5x Speed";
			break;

			case 3:
			speed = 1.0f;
			label = "Normal Speed";
			break;

			case 4:
			speed = 1.25f;
			label = "1.25x Speed";
			break;

			case 5:
			speed = 1.5f;
			label = "1.5x Speed";
			break;

			case 6:
			speed = 2.0f;
			label = "2x Speed";
			break;

			case 7:
			speed = 4.0f;
			label = "4x Speed";
			break;

			default:
			break;
		}

		Time.timeScale = speed;
		Time.fixedDeltaTime = Time.timeScale * 0.02f;
		gameSpeedLabel.text = label;
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
