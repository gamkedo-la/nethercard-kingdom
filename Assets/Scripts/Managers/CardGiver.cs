/**
 * Description: Gives cards on game win.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CardGiver : MonoBehaviour
{
	[SerializeField] private CollectionManager collection = null;
	[SerializeField] private Animator animator = null;
	[SerializeField] private Card[] cardsToGive = null;
	[SerializeField] private Transform[] holders = null;
	[SerializeField] private GameObject[] toEnable = null;

	void Start ()
	{
		Assert.IsNotNull( collection, $"Please assign <b>{nameof( collection )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( animator, $"Please assign <b>{nameof( animator )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.AreEqual( cardsToGive.Length, holders.Length );

		bool shouldGive = PlayerPrefs.GetInt( gameObject.name, -1 ) < 1;
		if ( shouldGive )
			GiveCards( );
		else
			End( );
	}

	private void GiveCards( )
	{
		PlayerPrefs.SetInt( gameObject.name, 1 ); // Mark that we gave out the cards
		collection.SortCollection( );

		for ( int i = 0; i < cardsToGive.Length; i++ )
		{
			GameObject go = Instantiate( cardsToGive[i].gameObject, holders[i] );
			go.GetComponent<ForceCardUpdate>( ).enabled = true;

			collection.AwardCard( new PlayerCard( ) { Card = cardsToGive[i], Amount = 1 } );
		}

		animator.enabled = true;

		collection.SortCollection( );
		collection.Save( );
	}

	public void End ()
	{
		foreach ( var item in toEnable )
			item.SetActive( true );
	}

	[ContextMenu( "Remove Game Progress Data" )]
	public void RemoveData( )
	{
		PlayerPrefs.DeleteKey( gameObject.name );
	}
}
