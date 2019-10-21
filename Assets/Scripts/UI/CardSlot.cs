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

	private GameObject card = null;

	void Start ()
	{
		Assert.IsNotNull( amount, $"Please assign <b>{nameof( amount )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( slot, $"Please assign <b>{nameof( slot )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( selection, $"Please assign <b>{nameof( selection )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

	}

	void Update ()
	{

	}

	public void Set( GameObject card, float amount )
	{
		this.card = Instantiate( card, slot.transform );
		this.card.GetComponent<Card>( ).SelectionMode = mode;

		this.amount.SetActive( true );

		if ( amountLabel )
			amountLabel.text = amount.ToString( );
	}

	public void SetEmpty( )
	{
		amount.SetActive( false );

		if ( card )
			Destroy( card );
	}

	public void Select( bool selected )
	{
		selection.SetActive( selected );
	}
}
