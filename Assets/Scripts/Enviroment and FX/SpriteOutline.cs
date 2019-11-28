/**
 * Description: A simple outline effect for sprites.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;

public class SpriteOutline : MonoBehaviour
{
	[SerializeField] private float alphaMin = 0.5f;
	[SerializeField] private float alphaMax = 1.0f;
	[SerializeField] private float alphaChange = 0.2f;

	private SpriteRenderer sprite = null;
	private float alpha;
	private Color color;

	void Start ()
	{
		sprite = GetComponent<SpriteRenderer>( );
		Assert.IsNotNull( sprite, $"Please assign <b>{nameof( sprite )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		alpha = alphaMin;
		color = sprite.color;
	}

	void Update ()
	{
		if ( alphaChange > 0 )
		{
			alpha += alphaChange * Time.deltaTime;
			if ( alpha >= alphaMax )
			{
				alpha = alphaMax;
				alphaChange = -alphaChange;
			}
			color.a = alpha;
		}
		else
		{
			alpha += alphaChange * Time.deltaTime;
			if ( alpha <= alphaMin )
			{
				alpha = alphaMin;
				alphaChange = -alphaChange;
			}
			color.a = alpha;
		}

		sprite.color = color;
	}
}
