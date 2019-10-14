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
	[SerializeField] private CardType type = CardType.Unit;
	[SerializeField] private int cost = 2;

	static public Card hoverCard = null;

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

		if(hoverCard == null)
			hoverCard = this;
	}

	public void OnOverExit( )
	{
		if ( isDraged )
			return;

		transform.localScale = Vector3.one;
		canvas.overrideSorting = false;
		canvas.sortingOrder = 0;

		if(hoverCard == this)
			hoverCard = null;
	}

	public void OnCliked( )
	{
		if ( !SummoningManager.Instance.EnoughMana( cost ) )
			return;

		canvasGroup.alpha = 0.5f;
		isDraged = true;
		OnOverEnter( );

		SummoningManager.Instance.Summoning( Camera.main.ScreenToWorldPoint( Input.mousePosition ), type, true );
	}

	public void OnReleased( )
	{
		if ( !isDraged )
			return;

		canvasGroup.alpha = 1f;
		isDraged = false;
		OnOverExit( );

		bool canSummon = SummoningManager.Instance.Summoning( Vector2.zero, type, false );

		if ( canSummon )
		{
			GameObject instance = Instantiate( toSummon, (Vector2)Camera.main.ScreenToWorldPoint( Input.mousePosition ), Quaternion.identity );
			if ( type == CardType.DirectDefensiveSpell || type == CardType.DirectOffensiveSpell )
				instance.GetComponent<Spell>( ).SetTarget( SummoningManager.Instance.LastTarget );

			SummoningManager.Instance.UseMana( cost );
			Destroy( gameObject );
		}
	}
}
