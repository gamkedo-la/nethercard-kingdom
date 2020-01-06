/**
 * Description: Hides object in WebGL.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;

public class HideInWebGL : MonoBehaviour
{
	void Start ()
	{
		#if UNITY_WEBGL
		gameObject.SetActive( false );
		#endif
	}
}
