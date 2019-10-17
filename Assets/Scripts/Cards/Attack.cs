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

	[SerializeField] protected float atackDamage = 2f;
	[SerializeField] protected float atackDelay = 1f;
}
