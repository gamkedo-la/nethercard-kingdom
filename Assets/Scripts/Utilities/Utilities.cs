/**
 * Description: Utility methods.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections;
using UnityEngine;

public class Utilities
{
	public static IEnumerator ChangeOverTime( float timeLength, System.Action<float> methodWithParameter )
	{
		return ChangeOverTime( timeLength, methodWithParameter, null );
	}

	public static IEnumerator ChangeOverTime( float timeLength, System.Action<float> methodWithParameter, System.Action onDone )
	{
		float currentTime = 0;
		float durationTime = timeLength;

		float progress;
		do
		{
			currentTime += Time.deltaTime;
			currentTime = currentTime < durationTime ? currentTime : durationTime;

			progress = currentTime / durationTime;
			progress = progress < 1.0f ? progress : 1.0f;

			methodWithParameter( progress );

			yield return null;
		}
		while ( progress < 1 );

		onDone?.Invoke( );
	}

	static public LineRenderer DrawDebugLine( Vector3 start, Vector3 end, Color color, float startWidth = 0.1f, float endWidth = 0.1f )
	{
		GameObject line = new GameObject( "Debug Line" );
		line.transform.position = start;

		LineRenderer lr = line.AddComponent<LineRenderer>( );
		lr.material = new Material( Shader.Find( "Sprites/Default" ) );
		lr.startColor = color;
		lr.endColor = color;

		lr.startWidth = startWidth;
		lr.endWidth = endWidth;

		lr.SetPosition( 0, start );
		lr.SetPosition( 1, end );

		return lr;
	}
}
