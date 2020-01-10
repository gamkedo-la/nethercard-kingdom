/**
 * Description: Manages state of enemy node on the world map.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class EnemyNode : MonoBehaviour
{
	public int Level { get { return level; } }

	[SerializeField] private int level = 1;
	[SerializeField] private GameObject[] toShowOnLevelUnlocked = null;
	[SerializeField] private GameObject[] toHideOnLevelUnlocked = null;

	void Start ()
	{
		//Assert.IsNotNull( , $"Please assign <b>{nameof(  )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		int starsUnlocked = ProgressManager.Instance.GetLevelData( level );

		if ( starsUnlocked > 0 || ProgressManager.Instance.MaxUnlockedLevel >= level ) // Unlocked
		{
			foreach ( var item in toShowOnLevelUnlocked )
				item.SetActive( true );

			foreach ( var item in toHideOnLevelUnlocked )
				item.SetActive( false );
		}
		else  // Locked
		{
			foreach ( var item in toShowOnLevelUnlocked )
				item.SetActive( false );

			foreach ( var item in toHideOnLevelUnlocked )
				item.SetActive( true );
		}
	}
}
