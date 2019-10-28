/**
 * Description: Fires on screenshakes in response to event.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;

public class ScreenshakeManager : MonoBehaviour
{
	public static ScreenshakeManager Instance { get; private set; }

	[SerializeField] private CinemachineImpulseSource impulse = null;

	private void Awake( )
	{
		if ( Instance != null && Instance != this )
			Destroy( this );
		else
			Instance = this;
	}

	private void OnDestroy( ) { if ( this == Instance ) { Instance = null; } }

	void Start( )
	{
		Assert.IsNotNull( impulse, $"Please assign <b>{nameof( impulse )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	public void DoShake( )
	{
		impulse.GenerateImpulse( );
	}

	public void DoShake( float strength )
	{
		impulse.GenerateImpulse( new Vector3(0, -strength, 0 ) );
	}
}
