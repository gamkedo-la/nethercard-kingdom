/**
 * Description: Manages game progress.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System;
using System.IO;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class ProgressManager : MonoBehaviour
{
	[System.Serializable]
	public class ProgressData
	{
		public LeveData[] Levels;
		public int SelectedLevel = 1;
		public int MaxUnlockedLevel = 1;
	}

	[System.Serializable]
	public class LeveData
	{
		public int Level = 1;
		public int StartAmount = 0;
	}

	public static ProgressManager Instance { get; private set; }
	public int MaxUnlockedLevel { get { return gameProgress.MaxUnlockedLevel; } }
	public int MaxLevels { get { return maxLevels; } }
	public int SelectedLevel { get { return gameProgress.SelectedLevel; } set { gameProgress.SelectedLevel = value; SaveData( ); } }

	[SerializeField] private int maxLevels = 12;

	private const string ProgressDataPrefs = "Progress Data";

	private ProgressData gameProgress;

	void Start( )
	{
		//Assert.IsNotNull( gameSpeedLabel, $"Please assign <b>{nameof( gameSpeedLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		LoadData( );
	}

	private void Awake( )
	{
		if ( Instance != null && Instance != this )
			Destroy( this );
		else
			Instance = this;
	}

	void OnEnable( )
	{
		LoadData( );
	}

	private void OnDestroy( ) { if ( this == Instance ) { Instance = null; } }

	public void SetLevelData( int level, int starsObtained )
	{
		if ( level > maxLevels )
			return;

		foreach ( var item in gameProgress.Levels )
		{
			if ( item.Level == level ) // Found level data
			{
				item.StartAmount = starsObtained;
				SaveData( );
				return;
			}
		}

		// New data
		Array.Resize( ref gameProgress.Levels, gameProgress.Levels.Length+1 );
		gameProgress.Levels[gameProgress.Levels.Length - 1] = new LeveData( ) { Level = level, StartAmount = starsObtained };

		SaveData( );
	}

	public int GetLevelData( int level )
	{
		if ( level > maxLevels )
			return 0;

		foreach ( var item in gameProgress.Levels )
		{
			if ( item.Level == level )
				return item.StartAmount;
		}

		return 0;
	}

	public void UnlockNewLevel( )
	{
		if ( gameProgress.MaxUnlockedLevel + 1 > maxLevels )
		{
			Debug.Log( "Won last level" );
			return;
		}

		gameProgress.MaxUnlockedLevel++;
		SaveData( );
	}

	private void LoadDefaultProgressData( )
	{
		gameProgress = new ProgressData( );
		gameProgress.Levels = new LeveData[0];
	}

	private void LoadData( )
	{
		XmlSerializer xmlSerializer = new XmlSerializer( typeof( ProgressData ) );

		string loadedProgressData = PlayerPrefs.GetString( ProgressDataPrefs );

		// No card data found, using defaults
		if ( loadedProgressData == "" )
		{
			LoadDefaultProgressData( );
			return;
		}

		// Data found
		using ( StringReader reader = new StringReader( loadedProgressData ) )
		{
			gameProgress = xmlSerializer.Deserialize( reader ) as ProgressData;
		}
	}

	private void SaveData( )
	{
		XmlSerializer xmlSerializer = new XmlSerializer( typeof( ProgressData ) );

		using ( StringWriter writer = new StringWriter( ) )
		{
			xmlSerializer.Serialize( writer, gameProgress );
			PlayerPrefs.SetString( ProgressDataPrefs, writer.ToString( ) );
		}

		LoadData( );
	}

	[ContextMenu( "Remove Game Progress Data" )]
	public void RemovdGameProgressData( )
	{
		PlayerPrefs.DeleteKey( ProgressDataPrefs );
	}
}
