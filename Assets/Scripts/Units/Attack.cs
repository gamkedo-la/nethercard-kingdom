/**
 * Description: Base class for attacks.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class Attack : MonoBehaviour
{
	public float DPS { get { return atackDamage / atackDelay; } }
	public bool Frozen { get; set; } = false;

	[SerializeField] protected Animator animator = null;
	[SerializeField] protected float atackDamage = 2f;
	[SerializeField] protected float atackDelay = 1f;

	virtual protected void Start( )
	{
		Assert.IsNotNull( animator, $"Please assign <b>{nameof( animator )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}
}
