/**
 * Description: Enables enemy based on current selected level.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;

public class OpponentEnabler : MonoBehaviour
{
	public HP OpponentsHP { get { return opponentsHP; } }

	[SerializeField] private HP opponentsHP = null;
	[SerializeField] private GameObject[] opponents = null;

	void Start ()
	{
		opponents[ProgressManager.Instance.SelectedLevel - 1].SetActive( true );
	}
}
