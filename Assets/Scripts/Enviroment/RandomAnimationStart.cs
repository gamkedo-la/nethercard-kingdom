/**
 * Description: Starts animation with a delay.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class RandomAnimationStart : MonoBehaviour
{
	[SerializeField] private float maxDelay = 0.3f;

	private Animator animator = null;

	void Start( )
	{
		animator = GetComponent<Animator>( );
		Assert.IsNotNull( animator, $"Please assign <b>{nameof( animator )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	void OnEnable( )
	{
		animator = GetComponent<Animator>( );
		animator.Play( 0, 0, Random.Range( 0f, maxDelay ) );
	}
}
