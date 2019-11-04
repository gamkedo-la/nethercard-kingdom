/**
 * Description: Adds mana on clicking.
 * Authors: Kornel, Cole
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.EventSystems;

public class AddManaCheat : MonoBehaviour, IPointerDownHandler
{
	public void OnPointerDown( PointerEventData eventData )
	{
		OnClicked( );
	}

	public void OnClicked( )
	{
		CheatAndDebug.Instance.AddMana( );
	}
}
