/**
 * Description: Utility functions.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections;
using UnityEngine;

public class Utilities
{
	public static IEnumerator ChangeOverTime( float timeLength, System.Action<float> methodWithParameter )
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
	}
}
