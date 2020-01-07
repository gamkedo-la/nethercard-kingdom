/**
 * Description: Manages stars shown on nodes.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class NodeStars : MonoBehaviour
{
	[SerializeField] private GameObject star1 = null;
	[SerializeField] private GameObject star2 = null;
	[SerializeField] private GameObject star3 = null;
	[SerializeField] private int level = 1;

	void Start ()
	{
		Assert.IsNotNull( star1, $"Please assign <b>{nameof( star1 )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( star2, $"Please assign <b>{nameof( star2 )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( star3, $"Please assign <b>{nameof( star3 )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		int starsUnlocked = ProgressManager.Instance.GetLevelData( level );

		star1.SetActive( starsUnlocked > 0 );
		star2.SetActive( starsUnlocked > 1 );
		star3.SetActive( starsUnlocked > 2 );
	}
}
