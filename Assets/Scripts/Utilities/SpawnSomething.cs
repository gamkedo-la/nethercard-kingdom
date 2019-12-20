/**
 * Description: Spawns a Game Object on demand.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class SpawnSomething : MonoBehaviour
{
	[SerializeField] private GameObject thingToSpawn = null;

	void Start( )
	{
		Assert.IsNotNull( thingToSpawn, $"Please assign <b>{nameof( thingToSpawn )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	public void Spawn( )
	{
		Spawn( transform.position, Quaternion.identity );
	}

	public void Spawn( Vector2 position )
	{
		Spawn( position, Quaternion.identity );
	}

	public void Spawn( Vector2 position, Quaternion rotation )
	{
		Instantiate( thingToSpawn, position, rotation );
	}
}
