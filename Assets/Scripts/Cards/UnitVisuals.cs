/**
 * Description: Manages visuals and effects of a unit.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class UnitVisuals : MonoBehaviour
{
	[Header("Main")]
	[SerializeField] private SpriteRenderer borderSprite = null;
	[SerializeField] private SpriteRenderer fillSprite = null;
	[SerializeField] private SortingGroup group = null;

	[Header("Healing")]
	[SerializeField] private ParticleSystem healEffect = null;

	[Header("Shock")]
	[SerializeField] private GameObject skeleton = null;
	[SerializeField] private string shockLayer = "Foreground";
	[SerializeField] private Color shockColor = Color.black;

	void Start( )
	{
		Assert.IsNotNull( fillSprite, $"Please assign <b>{nameof( fillSprite )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( borderSprite, $"Please assign <b>{nameof( borderSprite )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( group, $"Please assign <b>{nameof( group )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( healEffect, $"Please assign <b>{nameof( healEffect )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( skeleton, $"Please assign <b>{nameof( skeleton )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	public void Shocked( float time )
	{
		StartCoroutine( DoShock( time ) );
	}

	public void Healed( )
	{
		healEffect.Play( );
	}

	public void UpdateVisuals( Sprite unitBorderSprite, Sprite unitFillSprite )
	{
		borderSprite.sprite = unitBorderSprite;
		fillSprite.sprite = unitFillSprite;
	}

	private IEnumerator DoShock( float time )
	{
		Color col = fillSprite.color;
		int layer = fillSprite.sortingLayerID;
		int order = fillSprite.sortingOrder;

		fillSprite.color = shockColor;

		fillSprite.sortingLayerID = SortingLayer.NameToID( shockLayer );
		borderSprite.sortingLayerID = SortingLayer.NameToID( shockLayer );
		group.sortingLayerID = SortingLayer.NameToID( shockLayer );

		fillSprite.sortingOrder = 1000;
		borderSprite.sortingOrder = 1000;

		skeleton.SetActive( true );

		yield return new WaitForSeconds( time );

		fillSprite.color = col;

		fillSprite.sortingLayerID = layer;
		borderSprite.sortingLayerID = layer;
		group.sortingLayerID = layer;

		fillSprite.sortingOrder = order;
		borderSprite.sortingOrder = order;

		skeleton.SetActive( false );
	}
}
