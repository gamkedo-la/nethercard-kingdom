/**
 * Description: Manages visuals of a unit.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class UnitVisuals : MonoBehaviour
{
	[SerializeField] private SpriteRenderer borderSprite = null;
	[SerializeField] private SpriteRenderer fillSprite = null;

	void Start( )
	{
		Assert.IsNotNull( borderSprite, $"Please assign <b>{nameof( borderSprite )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( fillSprite, $"Please assign <b>{nameof( fillSprite )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	public void UpdateUnitStatsFromEditor( Sprite unitBorderSprite, Sprite unitFillSprite )
	{
		borderSprite.sprite = unitBorderSprite;
		fillSprite.sprite = unitFillSprite;
	}
}
