/**
 * Description: Changes the visuals of a card slot.
 * Authors: Kornel, Bilal
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CardSlotVisuals : MonoBehaviour
{
	[SerializeField] private Image slotFill = null;
	[SerializeField] private Color hoverColor = Color.white;

	private Color currentFillColor;
	private Color originalFillColor;

	void Start( )
	{
		Assert.IsNotNull( slotFill, $"Please assign <b>{nameof( slotFill )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		originalFillColor = slotFill.color;
		currentFillColor = originalFillColor;
	}

	void Update( )
	{
		slotFill.color = Color.Lerp( slotFill.color, currentFillColor, 0.25f ); // TODO: Make it more frame-rate independent?
	}

	public void OnHoverEnter( )
	{
		currentFillColor = hoverColor;
	}

	public void OnHoverExit( )
	{
		currentFillColor = originalFillColor;
	}
}
