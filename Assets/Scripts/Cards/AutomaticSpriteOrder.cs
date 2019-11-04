/**
 * Description: Sets sprite order based on Y position.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
public class AutomaticSpriteOrder : MonoBehaviour
{
	private SortingGroup sorting = null;

	void Start ()
	{
		sorting = GetComponent<SortingGroup>( );
		Assert.IsNotNull( sorting, $"Please assign <b>{nameof( sorting )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	void FixedUpdate( ) => sorting.sortingOrder = -(int)( transform.position.y * 100 );
}
