﻿/**
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

	[Header("Health")]
	[SerializeField] private ParticleSystem healEffect = null;
	[SerializeField] private GameObject hpLabel = null;

	[Header("Shock")]
	[SerializeField] private GameObject skeleton = null;
	[SerializeField] private GameObject shadow = null;
	[SerializeField] private string shockLayer = "Foreground";
	[SerializeField] private Color shockColor = Color.black;

	[Header("Death")]
	[SerializeField] private GameObject deathEffect = null;
	[SerializeField] private float deathEffectScale = 1.0f;

	[Header("Other Effects")]
	[SerializeField] private GameObject footprints = null;
	[SerializeField] private float footprintsScale = 1.0f;

	private float originalXScale;
	private Transform prints;

	void Start( )
	{
		Assert.IsNotNull( fillSprite, $"Please assign <b>{nameof( fillSprite )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( borderSprite, $"Please assign <b>{nameof( borderSprite )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( group, $"Please assign <b>{nameof( group )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( mainSprites, $"Please assign <b>{nameof( mainSprites )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( healEffect, $"Please assign <b>{nameof( healEffect )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( hpLabel, $"Please assign <b>{nameof( hpLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( skeleton, $"Please assign <b>{nameof( skeleton )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( shadow, $"Please assign <b>{nameof( shadow )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( deathEffect, $"Please assign <b>{nameof( deathEffect )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		originalXScale = mainSprites.transform.localScale.x;

		if ( footprints )
		{
			prints = Instantiate( footprints, transform.position, Quaternion.identity ).transform;
			prints.localScale = Vector3.one * footprintsScale;
		}
	}

	void Update( )
	{
		hpLabel.SetActive( CheatAndDebug.Instance.ShowHPLabels );

		if ( footprints )
			prints.position = transform.position;
	}

	public void Shocked( float time ) => StartCoroutine( DoShock( time ) );

	public void Healed( ) => healEffect.Play( );

	public void Killed( )
	{
		GameObject go = Instantiate( deathEffect, transform.position, Quaternion.Euler( 0, 0, 66.5f ) );
		go.transform.localScale = Vector3.one * deathEffectScale;

		Destroy( gameObject );
	}

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
