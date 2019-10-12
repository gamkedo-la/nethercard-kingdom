/**
 * Description: Main card functionality.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class Card : MonoBehaviour
{
	[SerializeField] private Canvas canvas = null;
	[SerializeField] private CanvasGroup canvasGroup = null;
	[SerializeField] private GameObject toSummon = null;
	[SerializeField] private int cost = 2;

	private bool isDraged = false;

	void Start ()
	{
		Assert.IsNotNull( canvas, $"Please assign <b>{nameof( canvas )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( canvasGroup, $"Please assign <b>{nameof( canvasGroup )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( toSummon, $"Please assign <b>{nameof( toSummon )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	void Update( )
	{
		if ( SummoningManager.Instance.EnoughMana( cost ) )
			canvasGroup.alpha = 1f;
		else
			canvasGroup.alpha = 0.9f;
	}

	public void OnOverEnter( )
	{
		transform.localScale = Vector3.one * 1.3f;
		canvas.overrideSorting = true;
		canvas.sortingOrder = 1100;
	}

	public void OnOverExit( )
	{
		if ( isDraged )
			return;

		transform.localScale = Vector3.one;
		canvas.overrideSorting = false;
		canvas.sortingOrder = 0;
	}

	public void OnCliked( )
	{
		if ( !SummoningManager.Instance.EnoughMana( cost ) )
			return;

		canvasGroup.alpha = 0.5f;
		isDraged = true;
		OnOverEnter( );

		SummoningManager.Instance.Summoning( true, Camera.main.ScreenToWorldPoint( Input.mousePosition ) );
	}

	public void OnReleased( )
	{
		if ( !isDraged )
			return;

		canvasGroup.alpha = 1f;
		isDraged = false;
		OnOverExit( );

		bool canSummon = SummoningManager.Instance.Summoning( false, Vector2.zero );

		if ( canSummon )
		{
			Instantiate( toSummon, Camera.main.ScreenToWorldPoint( Input.mousePosition ), Quaternion.identity );
			SummoningManager.Instance.UseMana( cost );
			Destroy( gameObject );
		}
	}
}
