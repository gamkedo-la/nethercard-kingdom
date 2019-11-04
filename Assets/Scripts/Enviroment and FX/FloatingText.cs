/**
 * Description: Controls the movement and opacity of the floating text.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

/**
 * Use example:
 * ------------
 * GameObject go = Instantiate( floatingText, transform.position, Quaternion.identity );
 * FloatingText ft = go.GetComponent<FloatingText>( );
 * ft.SetPrameters( textToShow, 1.0f, 1.0f, Color.white );
 **/

using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class FloatingText : MonoBehaviour
{
	[Header("External objects")]
	[SerializeField] private TextMeshProUGUI text = null;

	[Header("Parameters")]
	[SerializeField] private float speedMove = 2.0f;
	[SerializeField] private float speedFade = 0.5f;
	[SerializeField] private float speedShrink = 0.5f;
	[SerializeField] private float speedSideways = 2.0f;
	[SerializeField] private float speedDown = 4.0f;
	[SerializeField] private float varianceMin = 0.5f;
	[SerializeField] private float varianceMax = 1.5f;

	private float multiplier = 1.0f;
	private float gravity = 0.0f;
	private float sideways = 0.0f;
	private float variance = 1.0f;
	private float scale = 1.0f;

	void Start( )
	{
		if ( !text )
			text = GetComponent<TextMeshProUGUI>( );

		Assert.IsNotNull( text, $"Please assign <b>{nameof( text )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	private void Update( )
	{
		Animate( );
	}

	public void SetPrameters( string message, bool directionLeft, float scale = 1.0f, float speedMultiplier = 1.0f, Color tintColor = default )
	{
		if ( tintColor == default )
			tintColor = Color.white;

		multiplier = speedMultiplier;
		text.text = message;
		text.color *= tintColor;

		speedSideways *= directionLeft ? 1 : -1;

		transform.localScale = Vector3.one * scale;

		sideways = speedSideways;// * Random.Range( 0, 2 ) > 0 ? 1 : -1;
		variance = Random.Range( varianceMin, varianceMax );
	}

	private void Animate( )
	{
		gravity -= speedDown * multiplier * Time.deltaTime;
		Vector3 moveUp = Vector3.up * 4;
		Vector3 moveDown = Vector3.down * -gravity * variance;
		Vector3 moveSideways = transform.right * sideways * variance;
		float speed = speedMove * multiplier * Time.deltaTime;

		transform.position += ( moveUp + moveSideways + moveDown ) * speed;
		scale -= speedShrink * Time.deltaTime;
		transform.localScale = Vector3.one * scale;

		Color c = text.color;
		c.a -= speedFade * multiplier * Time.deltaTime;
		c.a = Mathf.Clamp( c.a, 0f, 1f );
		text.color = c;

		if ( c.a <= 0 )
			Destroy( gameObject );
	}
}
