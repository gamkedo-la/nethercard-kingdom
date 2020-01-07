/**
 * Description: Controls stars showing on the Won Screen.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class EndGameStars : MonoBehaviour
{
	[SerializeField] private Animator star1 = null;
	[SerializeField] private Animator star2 = null;
	[SerializeField] private Animator star3 = null;
	[SerializeField] private float starDelay = 2f;
	[SerializeField] private float delayBetweenStars = 1f;

	void Start( )
	{
		Assert.IsNotNull( star1, $"Please assign <b>{nameof( star1 )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( star2, $"Please assign <b>{nameof( star2 )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( star3, $"Please assign <b>{nameof( star3 )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	public void Go( int starsUnlocked )
	{
		if ( starsUnlocked > 0 )
			Invoke( nameof( Activate1 ), starDelay + delayBetweenStars * 1 );
		if ( starsUnlocked > 1 )
			Invoke( nameof( Activate2 ), starDelay + delayBetweenStars * 2 );
		if ( starsUnlocked > 2 )
			Invoke( nameof( Activate3 ), starDelay + delayBetweenStars * 3 );
	}

	private void Activate1( ) =>  star1.enabled = true;
	private void Activate2( ) =>  star2.enabled = true;
	private void Activate3( ) =>  star3.enabled = true;
}
