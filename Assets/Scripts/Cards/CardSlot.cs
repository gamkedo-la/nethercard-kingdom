/**
 * Description: Card slot for use in deck building and card collection.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class CardSlot : MonoBehaviour
{
	[SerializeField] private GameObject amount = null;
	[SerializeField] private Transform cardHolder = null;
	[SerializeField] private GameObject selection = null;
	[SerializeField] private TextMeshProUGUI amountLabel = null;
	[SerializeField] private CardSelectionMode mode = CardSelectionMode.InCollection;

	private Card cardInSlot;
	private bool isBeingDraged = false;

	void Start ()
	{
		Assert.IsNotNull( amount, $"Please assign <b>{nameof( amount )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( cardHolder, $"Please assign <b>{nameof( cardHolder )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( selection, $"Please assign <b>{nameof( selection )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( amountLabel, $"Please assign <b>{nameof( amountLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	void Update( )
	{
		if ( isBeingDraged && cardInSlot )
		{
			cardInSlot.transform.position = Vector2.Lerp( cardInSlot.transform.position, Input.mousePosition + new Vector3( 0.0f, -Screen.height / 4.0f, 0.0f ), 0.25f );
		}
		else if ( cardInSlot )
		{
			cardInSlot.transform.position = Vector2.Lerp( cardInSlot.transform.position, cardHolder.position, 0.15f );
		}
	}

	public void Set( GameObject cardObject, float amount )
	{
		SetEmpty( );

		GameObject go = Instantiate( cardObject, cardHolder );
		cardInSlot = go.GetComponent<Card>( );
		cardInSlot.SelectionMode = mode;
		cardInSlot.onStartedDrag.AddListener( ( ) => isBeingDraged = true );
		cardInSlot.onEndedDrag.AddListener( ( ) => isBeingDraged = false );

		amountLabel.text = amount.ToString( );

		if ( mode == CardSelectionMode.InCollection )
			this.amount.SetActive( true );
	}

	public bool IsEmpty( )
	{
		return !cardInSlot;
	}

	public void SetEmpty( )
	{
		amount.SetActive( false );

		if ( cardInSlot )
		{
			cardInSlot.onStartedDrag.RemoveAllListeners( );
			cardInSlot.onEndedDrag.RemoveAllListeners( );
			Destroy( cardInSlot.gameObject );
		}
	}

	public void Select( bool selected )
	{
		selection.SetActive( selected );
	}

	public void OnRelease( )
	{
		///DeckBuilder.Instance.otherSlot = transform.parent.gameObject;
		///DeckBuilder.Instance.MoveSlot( );
	}
}
