/**
 * Description: Manages summoning of instances.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class SummoningManager : MonoBehaviour
{
	public static SummoningManager Instance { get; private set; }
	public CardType UsingMode { get; private set; } = CardType.None;
	public Targetable LastTarget { get; private set; } = null;

	[SerializeField] private TextMeshProUGUI manaCounter = null;
	[SerializeField] private Image progressImage = null;
	[SerializeField] private GameObject summoningArea = null;
	[SerializeField] private GameObject good = null;
	[SerializeField] private GameObject bad = null;
	[SerializeField] private LineRenderer line = null;
	[SerializeField] private float manaTickTime = 2f;
	[SerializeField] private int manaTickAmount = 1;

	private Vector2 lineStartPoint;
	private bool overValidTarget = false;
	private int currentMana = 0;
	private float currentManaProgress = 0;

	private void Awake( )
	{
		if ( Instance != null && Instance != this )
			Destroy( this );
		else
			Instance = this;
	}

	private void OnDestroy( ) { if ( this == Instance ) { Instance = null; } }

	void Start ()
	{
		Assert.IsNotNull( manaCounter, $"Please assign <b>{nameof( manaCounter )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( progressImage, $"Please assign <b>{nameof( progressImage )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( summoningArea, $"Please assign <b>{nameof( summoningArea )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( good, $"Please assign <b>{nameof( good )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( bad, $"Please assign <b>{nameof( bad )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( line, $"Please assign <b>{nameof( line )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		InvokeRepeating( nameof( ManaTick ), manaTickTime, manaTickTime );
	}

	void Update ()
	{
		if ( UsingMode != CardType.None )
		{
			Vector2 endPoint = Camera.main.ScreenToWorldPoint( Input.mousePosition );

			line.SetPosition( 0, lineStartPoint );
			line.SetPosition( 1,  endPoint);

			good.transform.position = endPoint;
			bad.transform.position = endPoint;
		}

		currentManaProgress += Time.deltaTime / manaTickTime;
		progressImage.fillAmount = currentManaProgress;
	}

	private void ManaTick( )
	{
		currentMana += manaTickAmount;
		manaCounter.text = currentMana.ToString( );
		currentManaProgress = 0;
	}

	public bool Summoning( Vector2 startPos, CardType type, bool started )
	{
		if ( type == CardType.Unit || type == CardType.None )
			summoningArea.SetActive( started );

		UsingMode = started ? type : CardType.None;
		bad.SetActive( started );
		line.enabled = started;
		lineStartPoint = startPos;

		if ( !started )
			good.SetActive( false );

		if ( !started && overValidTarget )
			return true;

		return false;
	}

	public bool EnoughMana( int amount ) => currentMana >= amount;

	public void UseMana( int amount )
	{
		currentMana -= amount;
		manaCounter.text = currentMana.ToString( );
	}

	public void MouseOverTarget( bool isOver, CardType type, ConflicSide side, Targetable target )
	{
		LastTarget = target;

		// We aren't summoning anything
		if ( UsingMode == CardType.None )
		{
			good.SetActive( false );
			bad.SetActive( false );

			return;
		}

		// Summoning a Unit and over Summoning Area
		if ( UsingMode == CardType.Unit && type == CardType.None )
		{
			overValidTarget = isOver;
			good.SetActive( isOver );
			bad.SetActive( !isOver );

			return;
		}

		// Summoning Unit and over a Unit
		if ( UsingMode == CardType.Unit && type == CardType.Unit )
		{
			overValidTarget = false;
			good.SetActive( false );
			bad.SetActive( true );

			return;
		}

		// Summoning Spell and over an enemy Unit
		if ( UsingMode == CardType.DirectOffensiveSpell && type == CardType.Unit && side == ConflicSide.Enemy )
		{
			overValidTarget = isOver;
			good.SetActive( isOver );
			bad.SetActive( !isOver );

			return;
		}
	}
}
