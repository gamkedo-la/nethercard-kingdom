/**
 * Description: Allows cards to be trashed.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class CardTrash : Targetable
{
	[SerializeField] private GameObject normal = null;
	[SerializeField] private GameObject open = null;

	override public void Start( )
	{
		base.Start( );

		Assert.IsNotNull( normal, $"Please assign <b>{nameof( normal )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( open, $"Please assign <b>{nameof( open )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	void OnMouseEnter( )
	{
		if ( !active )
			return;

		normal.SetActive( false );
		open.SetActive( true );
		SummoningManager.Instance.MouseOverTarget( this, targetableBy, true, true );
	}

	void OnMouseExit( )
	{
		if ( !active )
			return;

		normal.SetActive( true );
		open.SetActive( false );
		SummoningManager.Instance.MouseOverTarget( this, targetableBy, false, false );
	}

	override public void SetActiveState( CardType incomingType )
	{
		// True if this entity is marked to respond to the incoming (being played) type of card
		active = targetableBy.HasFlag( incomingType );

		col2D.enabled = active;
		normal.SetActive( active );
		open.SetActive( false );
	}
}
