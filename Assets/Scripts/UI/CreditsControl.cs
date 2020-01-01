/**
 * Description: Allow for credit control/scrolling.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class CreditsControl : MonoBehaviour
{
	[SerializeField] private MainMenu mainMenu = null;
	[SerializeField] private Transform creditText = null;
	[SerializeField] private Animator animator = null;
	[SerializeField] private float scrollSpeed = 300f;

	void Start( )
	{
		Assert.IsNotNull( creditText );
		Assert.IsNotNull( animator );
		Assert.IsNotNull( mainMenu );
	}

	void Update( )
	{
		if ( Input.GetAxis( "Vertical" ) != 0 )
		{
			if ( animator.enabled )
			{
				animator.StopPlayback( );
				animator.enabled = false;
			}

			Vector2 pos = creditText.position;
			pos.y += Input.GetAxis( "Vertical" ) * scrollSpeed * Time.deltaTime;
			creditText.position = pos;
		}
	}

	public void EndCredits( )
	{
		mainMenu.CloseCredits( );
	}
}
