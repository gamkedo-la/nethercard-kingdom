/**
 * Description: Main menu.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class MainMenu : MonoBehaviour
{
	[SerializeField] private Animator animator = null;
	[SerializeField] private GameObject map = null;

	public void Start ()
	{
		Assert.IsNotNull( animator, $"Please assign <b>{nameof( animator )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( map, $"Please assign <b>{nameof( map )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	public void Play( )
	{
		animator.SetTrigger( "Hide" );
	}

	public void ShowMap( )
	{
		map.SetActive( true );
	}
}
