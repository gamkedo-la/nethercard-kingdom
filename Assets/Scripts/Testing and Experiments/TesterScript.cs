/**
 * Description: Tests different things.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class TesterScript : MonoBehaviour
{
	[SerializeField] private FloatingText floatingText = null;

	void Start ()
	{
		//Assert.IsNotNull( , $"Please assign <b>{nameof(  )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	void Update ()
	{

	}

	public void OnEnter()
	{
		Debug.Log( "On" );
	}

	public void OnExit( )
	{
		Debug.Log( "Off" );
	}

	public void DoShake( )
	{
		Debug.Log( "Screenshake..." );
		ScreenshakeManager.Instance.DoShake( );
	}

	public void DoShake( float strength )
	{
		Debug.Log( "Screenshake with strength..." );
		ScreenshakeManager.Instance.DoShake( strength );
	}

	public void SpawnFT( )
	{
		Debug.Log( "Spawning FT..." );
		GameObject go = Instantiate( floatingText.gameObject, new Vector3(-22, 0, 0), Quaternion.identity );
		FloatingText ft = go.GetComponent<FloatingText>( );
		ft.SetPrameters( Random.Range(1, 10).ToString( ), true, 1.0f, 1.0f, Color.white );
	}

	private void OnMouseEnter( )
	{
		Debug.Log( "M On" );
	}

	private void OnMouseExit( )
	{
		Debug.Log( "M Off" );
	}
}
