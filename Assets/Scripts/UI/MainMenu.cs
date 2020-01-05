/**
 * Description: Main menu.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[SerializeField] private Animator animator = null;

	public void Start ()
	{
		Assert.IsNotNull( animator, $"Please assign <b>{nameof( animator )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	public void Play( )
	{
		animator.SetTrigger( "Hide" );
	}

	public void ShowMap( )
	{
		SceneManager.LoadScene( "World Map", LoadSceneMode.Single );
	}

	public void ShowCredits( )
	{
		animator.SetTrigger( "Show Credits" );
	}

	public void CloseCredits( )
	{
		animator.SetTrigger( "Hide Credits" );
	}

	public void QuiteGame( )
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit( );
		#endif
	}
}
