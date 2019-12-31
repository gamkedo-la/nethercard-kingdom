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
	[SerializeField] private float lifetime = 1.5f;
	[SerializeField] private float moveSpeed = 2.0f;
	[SerializeField] private float speedShrink = 0.5f;
	[SerializeField] private float speedSideways = 2.0f;
	[SerializeField] private float speedDown = 4.0f;
	[SerializeField] private float varianceMin = 0.5f;
	[SerializeField] private float varianceMax = 1.5f;
	[SerializeField] private AnimationCurve multiplierCurve = new AnimationCurve(new Keyframe(0,1), new Keyframe(1,0));

	private float currentLifetime = 1.0f;
	private float multiplier = 1.0f;
	private float variance = 1.0f;
	private float scale = 1.0f;

	void Start( )
	{
		if ( !text )
			text = GetComponent<TextMeshProUGUI>( );

		Assert.IsNotNull( text, $"Please assign <b>{nameof( text )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		currentLifetime = lifetime;
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

		variance = Random.Range( varianceMin, varianceMax );
	}

	private void Animate( )
	{
		currentLifetime -= Time.deltaTime;
		float progress = currentLifetime / lifetime;
		progress = Mathf.Clamp( progress, 0f, 1f );

		Color c = text.color;
		c.a = progress;
		text.color = c;

		float gravity = speedDown * multiplier * multiplierCurve.Evaluate( progress ) * Time.deltaTime;
		Vector3 moveDown = Vector3.down * -gravity * variance;
		Vector3 moveSideways = transform.right * speedSideways * variance * multiplierCurve.Evaluate( progress );
		float speed = moveSpeed * multiplier * Time.deltaTime;

		transform.position += ( moveSideways + moveDown ) * speed;
		scale -= speedShrink * Time.deltaTime;
		transform.localScale = Vector3.one * scale;

		if ( currentLifetime <= 0 )
			Destroy( gameObject );
	}
}
