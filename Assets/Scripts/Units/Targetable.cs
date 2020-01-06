/**
 * Description: Allows GameObject to be targeted by cards.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class Targetable : MonoBehaviour
{
	[SerializeField] protected Collider2D col2D = null;
	[SerializeField] protected GameObject indicator = null;
	[SerializeField] protected Animator targeted = null;
	[SerializeField] protected CardType targetableBy = CardType.Undefined;

	protected bool active = false;

	virtual public void Start( )
	{
		Assert.IsNotNull( col2D, $"Please assign <b>{nameof( col2D )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( indicator, $"Please assign <b>{nameof( indicator )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		indicator.SetActive( false );
	}

	void OnMouseEnter( )
	{
		if ( active )
		{
			SummoningManager.Instance.MouseOverTarget( this, targetableBy, true );

			if ( targeted )
				targeted.enabled = true;
		}
	}

	void OnMouseExit( )
	{
		if ( active )
		{
			SummoningManager.Instance.MouseOverTarget( this, targetableBy, false );

			targeted.enabled = false;
		}
	}

	void OnEnable( )
	{
		SummoningManager.Instance.AddTargetable( this );
	}

	void OnDisable( )
	{
		if ( SummoningManager.Instance )
			SummoningManager.Instance.RemoveTargetable( this );
	}

	virtual public void SetActiveState( CardType incomingType )
	{
		// True if this entity is marked to respond to the incoming (being played) type of card
		active = targetableBy.HasFlag( incomingType );
		col2D.enabled = active;
		indicator.SetActive( active );
	}
}
