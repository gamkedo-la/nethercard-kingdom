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

	public bool Summoning( Vector2 startPos, CardType type, bool started )
	{
		if ( !CanSummon )
		{
			bad.SetActive( started );
			good.SetActive( false );

			return false;
		}

		if ( type == CardType.Unit || type == CardType.None )
			summoningAreaUnits.SetActive( started );

		if ( type == CardType.AoeSpell )
			summoningAreaAoe.SetActive( started );

		UsingMode = started ? type : CardType.None;
		bad.SetActive( started );

		if ( !started )
			good.SetActive( false );

		if ( !started && overValidTarget )
			return true;

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

	public void MouseOverTarget( bool isOver, CardType type, ConflicSide side, Targetable target )
	{
		if ( !CanSummon )
			return;

		LastTarget = target;

		// We aren't summoning anything (canceled)
		if ( UsingMode == CardType.None )
		{
			overValidTarget = false;
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

		// Summoning Direct Offensive Spell and over an enemy Unit
		if ( UsingMode == CardType.DirectOffensiveSpell && type == CardType.Unit && side == ConflicSide.Enemy )
		{
			overValidTarget = isOver;
			good.SetActive( isOver );
			bad.SetActive( !isOver );

			return;
		}

		// Summoning Direct Defensive Spell and over an player Unit
		if ( UsingMode == CardType.DirectDefensiveSpell && type == CardType.Unit && side == ConflicSide.Player )
		{
			overValidTarget = isOver;
			good.SetActive( isOver );
			bad.SetActive( !isOver );

			return;
		}

		// Summoning AoE Spell and over a AoE summoning area
		if ( UsingMode == CardType.AoeSpell && type == CardType.AoeSpell && side == ConflicSide.All )
		{
			overValidTarget = isOver;
			good.SetActive( isOver );
			bad.SetActive( !isOver );

			return;
		}
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
