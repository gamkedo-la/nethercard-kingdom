/**
 * Description: End game.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class GameEnd : MonoBehaviour
{
	[SerializeField] private GameObject endMessage = null;

	void Start ()
	{
		Assert.IsNotNull( endMessage, $"Please assign <b>{nameof( endMessage )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		if ( ProgressManager.Instance.SelectedLevel == 6 )
			endMessage.SetActive( true );
	}
}
