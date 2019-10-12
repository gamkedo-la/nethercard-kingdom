/**
 * Description: Detects mouse interaction and sends events.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;

public class SummoningArea : MonoBehaviour
{
	void OnMouseOver( )
	{
		SummoningManager.Instance.MouseOverSummoningArea( true );
	}

	void OnMouseExit( )
	{
		SummoningManager.Instance.MouseOverSummoningArea( false );
	}
}
