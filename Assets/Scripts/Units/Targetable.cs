/**
 * Description: Allows GameObject to be targeted by cards.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class Targetable : MonoBehaviour
{
	[SerializeField] private Collider2D col2D;
	[SerializeField] private CardType targetableBy = CardType.Undefined;
	//[SerializeField] private ConflicSide side = ConflicSide.Player;

	private bool active = false;

	void Start( )
	{
		Assert.IsNotNull( col2D, $"Please assign <b>{nameof( col2D )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	void OnMouseEnter( )
	{
		if ( active )
			SummoningManager.Instance.MouseOverTarget( this, targetableBy, true );
	}

	void OnMouseExit( )
	{
		if ( active )
			SummoningManager.Instance.MouseOverTarget( this, targetableBy, false );
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

	public void SetActiveState( CardType incommingType )
	{
		active = targetableBy.HasFlag( incommingType );
		col2D.enabled = active;
		//if (active)
			//Debug.Log( name + " is active" );
	}
}
