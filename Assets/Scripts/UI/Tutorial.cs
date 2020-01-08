/**
 * Description: Controls the tutorial.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class Tutorial : MonoBehaviour
{
	[SerializeField] private GameObject[] tips = null;
	[SerializeField] private Animator animator = null;

	private int tutNum = 0;

	void Start ()
	{
		Assert.IsNotNull( animator, $"Please assign <b>{nameof( animator )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		tips[tutNum].SetActive( true );
		Time.timeScale = 0f;
	}

	public void ShowNext( )
	{
		tutNum++;
		if ( tutNum > tips.Length - 1 )
		{
			animator.SetTrigger( "Hide" );
			return;
		}

		tips[tutNum-1].SetActive( false );
		tips[tutNum].SetActive( true );
	}

	public void Close( )
	{
		gameObject.SetActive( false );
		Time.timeScale = 1f;
	}
}
