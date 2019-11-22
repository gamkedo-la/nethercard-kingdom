/**
 * Description: Manages summoning of instances.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class SummoningManager : MonoBehaviour
{
	public static SummoningManager Instance { get; private set; }
	public CardType SummoningCardType { get; private set; } = CardType.Undefined;
	public Targetable LastTarget { get; private set; } = null;
	public bool CanSummon { get; set; } = true;

	[SerializeField] private PlaySound manaSound = null;
	[SerializeField] private TextMeshProUGUI manaCounter = null;
	[SerializeField] private Image progressImage = null;
	[SerializeField] private GameObject summoningAreaUnits = null;
	[SerializeField] private GameObject summoningAreaAoe = null;
	[SerializeField] private GameObject good = null;
	[SerializeField] private GameObject bad = null;
	[SerializeField] private LineRenderer line = null;
	[SerializeField] private float manaTickTime = 2f;
	[SerializeField] private int manaTickAmount = 1;
	[SerializeField] private int startMana = 3;

	private List<Targetable> targetables = new List<Targetable>();
	private bool overValidTarget = false;
	private int currentMana = 0;
	private float currentManaProgress = 0;
	private CardType currentSummoningType = CardType.Undefined;

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
		Assert.IsNotNull( manaSound, $"Please assign <b>{nameof( manaSound )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( manaCounter, $"Please assign <b>{nameof( manaCounter )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( progressImage, $"Please assign <b>{nameof( progressImage )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( summoningAreaUnits, $"Please assign <b>{nameof( summoningAreaUnits )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( summoningAreaAoe, $"Please assign <b>{nameof( summoningAreaAoe )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( good, $"Please assign <b>{nameof( good )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( bad, $"Please assign <b>{nameof( bad )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( line, $"Please assign <b>{nameof( line )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		AddMana( startMana, false );
	}

	void Update ()
	{
		UpdareIndicatorsPos( );

		if ( !CanSummon )
			return;

		ManaProgress( );
	}

	private void UpdareIndicatorsPos( )
	{
		Vector2 endPoint = Camera.main.ScreenToWorldPoint( Input.mousePosition );

		good.transform.position = endPoint;
		bad.transform.position = endPoint;
	}

	public void AddTargetable( Targetable targetable )
	{
		targetables.Add( targetable );
		targetable.SetActiveState( currentSummoningType );
	}
	public void RemoveTargetable( Targetable targetable ) => targetables.Remove( targetable );

	public bool Summoning( Vector2 startPos, CardType type, bool started )
	{
		if ( !CanSummon )
		{
			bad.SetActive( started );
			good.SetActive( false );

			return false;
		}

		if ( started )
		{
			currentSummoningType = type;
			foreach ( var t in targetables )
				t.SetActiveState( type );
		}
		else
			currentSummoningType = CardType.Undefined;

		if ( type == CardType.Unit )
			summoningAreaUnits.SetActive( started );

		if ( type == CardType.AoeSpell )
			summoningAreaAoe.SetActive( started );

		SummoningCardType = started ? type : CardType.Undefined;
		bad.SetActive( started );

		if ( !started )
		{
			good.SetActive( false );
			bad.SetActive( false );
		}

		if ( !started && overValidTarget )
		{
			overValidTarget = false;
			return true;
		}

		return false;
	}

	public bool EnoughMana( int amount ) => currentMana >= amount;

	public void AddMana( int amount, bool playSound = true )
	{
		if ( playSound )
			manaSound.Play( );

		currentMana += amount;
		manaCounter.text = currentMana.ToString( );
		currentManaProgress = 0;
	}

	public void RemoveMana( int amount )
	{
		currentMana -= amount;
		manaCounter.text = currentMana.ToString( );
	}

	public void MouseOverTarget( Targetable target, CardType targetableBy, bool isOver )
	{
		if ( !CanSummon )
			return;

		//Debug.Log( $"MouseOverTarget: SummoningCardType = {SummoningCardType},  targetableBy: {targetableBy}" );
		LastTarget = target;

		if ( targetableBy.HasFlag( SummoningCardType ) )
		{
			//Debug.Log( "SAME TYPES, isOver = " + isOver );
			overValidTarget = isOver;
			good.SetActive( isOver );
			bad.SetActive( !isOver );

			return;
		}
		else if ( SummoningCardType != CardType.Undefined )
		{
			overValidTarget = false;
			good.SetActive( false );
			bad.SetActive( true );
		}

		overValidTarget = false;
		good.SetActive( false );
		bad.SetActive( false );

		/*
		// Summoning Direct Offensive Spell and over an enemy Unit
		if ( SummoningCardType == CardType.DirectOffensiveSpell && targetableBy == CardType.EnemyUnit )
		{
			Debug.Log( "Over Summoning Enemy, isOver = " + isOver );
			overValidTarget = isOver;
			good.SetActive( isOver );
			bad.SetActive( !isOver );

			return;
		}

		// Summoning Direct Defensive Spell and over an player Unit
		if ( SummoningCardType == CardType.DirectDefensiveSpell && targetableBy == CardType.Unit )
		{
			Debug.Log( "Over Summoning Player, isOver = " + isOver );
			overValidTarget = isOver;
			good.SetActive( isOver );
			bad.SetActive( !isOver );

			return;
		}

		// Over not compatible target
		if ( SummoningCardType != targetableBy )
		{
			Debug.Log( "Non compatible types, isOver = " + isOver );
			overValidTarget = false;
			good.SetActive( false );
			bad.SetActive( true );

			return;
		}
		*/
		// Summoning a Unit and over Summoning Area
		/*if ( SummoningCardType == CardType.Unit && targetType == CardType.Unit )
		{
			Debug.Log( "Over Summoning " );
			overValidTarget = isOver;
			good.SetActive( isOver );
			bad.SetActive( !isOver );

			return;
		}*/

		// Summoning Unit and over a Unit
		/*if ( UsingMode == CardType.Unit && type == CardType.Unit )
		{
			overValidTarget = false;
			good.SetActive( false );
			bad.SetActive( true );

			return;
		}*/

		// Summoning AoE Spell and over a AoE summoning area
		/*if ( SummoningCardType == CardType.AoeSpell && targetType == CardType.AoeSpell && side == ConflicSide.All )
		{
			overValidTarget = isOver;
			good.SetActive( isOver );
			bad.SetActive( !isOver );

			return;
		}*/
	}

	private void ManaProgress( )
	{
		currentManaProgress += Time.deltaTime / manaTickTime;
		progressImage.fillAmount = currentManaProgress;

		if ( currentManaProgress < 1 )
			return;

		AddMana( manaTickAmount );
	}
}
