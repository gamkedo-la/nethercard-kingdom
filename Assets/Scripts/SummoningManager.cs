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

	[SerializeField] private TextMeshProUGUI manaCounter = null;
	[SerializeField] private Image progressImage = null;
	[SerializeField] private GameObject summoningArea = null;
	[SerializeField] private GameObject good = null;
	[SerializeField] private GameObject bad = null;
	[SerializeField] private LineRenderer line = null;
	[SerializeField] private float manaTickTime = 2f;
	[SerializeField] private int manaTickAmount = 1;

	private Vector2 lineStartPoint;
	private bool draging = false;
	private bool overSummoningArea = false;
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
		if ( draging )
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

	public bool Summoning( bool started, Vector2 startPos )
	{
		summoningArea.SetActive( started );
		bad.SetActive( started );
		draging = started;
		line.enabled = started;
		lineStartPoint = startPos;

		if ( !started )
			good.SetActive( false );

		if ( !started && overSummoningArea )
			return true;

		return false;
	}

	public bool EnoughMana( int amount ) => currentMana >= amount;

	public void UseMana( int amount )
	{
		currentMana -= amount;
		manaCounter.text = currentMana.ToString( );
	}

	public void MouseOverSummoningArea( bool isOver )
	{
		overSummoningArea = isOver;
		good.SetActive( isOver && draging );
		bad.SetActive( !isOver && draging );
	}
}
