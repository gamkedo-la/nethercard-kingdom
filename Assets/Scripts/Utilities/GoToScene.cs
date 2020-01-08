/**
 * Description: Goes to selected scene.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToScene : MonoBehaviour
{
	[SerializeField] private string scene = "Main";

	private void OnEnable( )
	{
		Invoke( nameof( Go ), 0.05f );
	}

	private void Go( )
	{
		SceneManager.LoadScene( scene, LoadSceneMode.Single );
	}
}
