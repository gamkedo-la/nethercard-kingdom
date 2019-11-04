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

	[Header("Movement")]
	[SerializeField] private GameObject mainSprites = null;
	[SerializeField] private GameObject mainSpritesPivot = null;
	[SerializeField] private int dirOfMovement = 1;
	[SerializeField] private float anglePerSpeed = -10f;

	[Header("Healing")]
	[SerializeField] private ParticleSystem healEffect = null;

	[Header("Shock")]
	[SerializeField] private GameObject skeleton = null;
	[SerializeField] private GameObject shadow = null;
	[SerializeField] private string shockLayer = "Foreground";
	[SerializeField] private Color shockColor = Color.black;

	private float originalXScale;

	void Start( )
	{
		Assert.IsNotNull( fillSprite, $"Please assign <b>{nameof( fillSprite )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( borderSprite, $"Please assign <b>{nameof( borderSprite )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( group, $"Please assign <b>{nameof( group )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( mainSprites, $"Please assign <b>{nameof( mainSprites )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( healEffect, $"Please assign <b>{nameof( healEffect )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( skeleton, $"Please assign <b>{nameof( skeleton )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( shadow, $"Please assign <b>{nameof( shadow )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		originalXScale = mainSprites.transform.localScale.x;
	}

	public void Shocked( float time ) => StartCoroutine( DoShock( time ) );

	public void Healed( ) => healEffect.Play( );

	public void MoveDir( Vector2 dir, float speed )
	{
		if ( dir.x < 0 )
			mainSprites.transform.localScale = new Vector3( -dirOfMovement * originalXScale, mainSprites.transform.localScale.y, mainSprites.transform.localScale.z );
		else if ( dir.x > 0 )
			mainSprites.transform.localScale = new Vector3( dirOfMovement * originalXScale, mainSprites.transform.localScale.y, mainSprites.transform.localScale.z );

		mainSpritesPivot.transform.localRotation = Quaternion.Euler
		(
			mainSpritesPivot.transform.localRotation.eulerAngles.x,
			mainSpritesPivot.transform.localRotation.eulerAngles.y,
			speed * anglePerSpeed
		);
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
		int orderFill = fillSprite.sortingOrder;
		int orderBorder = borderSprite.sortingOrder;

		fillSprite.color = shockColor;
		shadow.SetActive( false );

		fillSprite.sortingLayerID = SortingLayer.NameToID( shockLayer );
		borderSprite.sortingLayerID = SortingLayer.NameToID( shockLayer );
		group.sortingLayerID = SortingLayer.NameToID( shockLayer );

		fillSprite.sortingOrder = 1000;
		borderSprite.sortingOrder = 1000;

		skeleton.SetActive( true );

		yield return new WaitForSeconds( time );

		fillSprite.color = col;
		shadow.SetActive( true );

		fillSprite.sortingLayerID = layer;
		borderSprite.sortingLayerID = layer;
		group.sortingLayerID = layer;

		fillSprite.sortingOrder = orderFill;
		borderSprite.sortingOrder = orderBorder;

		skeleton.SetActive( false );
	}
}
