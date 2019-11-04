/**
 * Description: Allows GameObject to be targeted by cards.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;

public class Targetable : MonoBehaviour
{
	[SerializeField] private Unit unit = null;
	[SerializeField] private CardType targetType = CardType.None;
	[SerializeField] private ConflicSide side = ConflicSide.Player;

	void OnMouseOver( )
	{
		SummoningManager.Instance.MouseOverTarget( true, unit ? CardType.Unit : targetType, unit ? unit.Side : side, this );
	}

	void OnMouseExit( )
	{
		SummoningManager.Instance.MouseOverTarget( false, unit ? CardType.Unit : targetType, unit ? unit.Side : side, this );
	}
}
