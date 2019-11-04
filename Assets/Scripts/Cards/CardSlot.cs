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
	[SerializeField] private GameObject slot = null;
	[SerializeField] private GameObject selection = null;
	[SerializeField] private TextMeshProUGUI amountLabel = null;
	[SerializeField] private CardSelectionMode mode = CardSelectionMode.InCollection;

	private GameObject cardInSlot;
	private GameObject cancel = null;

	void Start ()
	{
		Assert.IsNotNull( amount, $"Please assign <b>{nameof( amount )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( slot, $"Please assign <b>{nameof( slot )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( selection, $"Please assign <b>{nameof( selection )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( amountLabel, $"Please assign <b>{nameof( amountLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		if(transform.childCount >= 5)
			cancel = transform.GetChild(4).gameObject;
	}

	public Card Set( GameObject cardObject, float amount )
	{
		SetEmpty( );

		GameObject oldCardInSlot = cardInSlot;
		GameObject toSpawn = cardObject.GetComponent<Card>( ).Prefab ? cardObject.GetComponent<Card>( ).Prefab : cardObject;

		cardInSlot = Instantiate( toSpawn, slot.transform );
		Card card = cardInSlot.GetComponent<Card>( );
		card.SelectionMode = mode;
		card.Prefab = toSpawn;

		this.amount.SetActive( true );
		amountLabel.text = amount.ToString( );

		if ( mode != CardSelectionMode.InCollection )
			this.amount.SetActive( false );

		if ( oldCardInSlot )
			Destroy( oldCardInSlot );

		if ( cancel )
			cancel.SetActive( true );

		return card;
	}

	public bool IsEmpty( )
	{
		return !( cardInSlot && cardInSlot.activeSelf );
	}

	public void SetEmpty( )
	{
		amount.SetActive( false );

		if ( cardInSlot )
			cardInSlot.SetActive( false );

		if ( cancel )
			cancel.SetActive( false );
	}

	public void Select( bool selected )
	{
		selection.SetActive( selected );
	}
}
