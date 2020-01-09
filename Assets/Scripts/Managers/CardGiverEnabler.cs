/**
 * Description: Enabled cards to give.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;

public class CardGiverEnabler : MonoBehaviour
{
	[SerializeField] private GameObject[] giver = null;
	[SerializeField] private float dealy = 1.5f;

	void Start( )
	{
		int stars = ProgressManager.Instance.GetLevelData( ProgressManager.Instance.SelectedLevel );
		Invoke( nameof( Give ), stars * dealy );
	}

	private void Give( )
	{
		giver[ProgressManager.Instance.SelectedLevel - 1].SetActive( true );
	}
}
