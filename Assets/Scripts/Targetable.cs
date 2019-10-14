/**
 * Description: Allows GameObject to be targeted by cards.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;

public class Targetable : MonoBehaviour
{
	[SerializeField] private Unit unit = null;

	void OnMouseOver( )
	{
		SummoningManager.Instance.MouseOverTarget( true, unit ? CardType.Unit : CardType.None, unit ? unit.Side : ConflicSide.Player, this );
	}

	void OnMouseExit( )
	{
		SummoningManager.Instance.MouseOverTarget( false, unit ? CardType.Unit : CardType.None, unit ? unit.Side : ConflicSide.Player, this );
	}
}
