/**
 * Description: Changes scale over time.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ScaleChanger : MonoBehaviour
{
	[Header("External objects")]
	[SerializeField, Tooltip("If empty will use self")] private Transform target = null;
	[SerializeField] private Behaviour[] toEnableOnDone = null;

	[Header("Scale")]
	[SerializeField, Tooltip("Use below values, otherwise uses current object scale")] private bool useRandomStartScale = true;
	[SerializeField] private float startScaleMin = 0.0f;
	[SerializeField] private float startScaleMax = 0.0f;
	[SerializeField, Tooltip("Use below values, otherwise uses current object scale")] private bool useRandomEndScale = false;
	[SerializeField] private float endScaleMin = 0.6f;
	[SerializeField] private float endScaleMax = 1.2f;

	[Header("Time")]
	[SerializeField] private float startDelayMin = 0.0f;
	[SerializeField] private float startDelayMax = 0.5f;
	[SerializeField] private float durationTimeMin = 0.1f;
	[SerializeField] private float durationTimeMax = 0.2f;

	[Header("Other")]
	[SerializeField] private bool runOnStart = true;

	[Header("Events")]
	[SerializeField] private UnityEvent onDone = null;

	private Vector3 startScale, endScale;
	private float startDelay = 0;

	void Start( )
	{
		if ( !target )
			target = transform;

		if ( useRandomStartScale )
			startScale = Vector3.one * Random.Range( startScaleMin, startScaleMax );
		else
			startScale = transform.localScale;

		if ( useRandomEndScale )
			endScale = Vector3.one * Random.Range( endScaleMin, endScaleMax );
		else
			endScale = transform.localScale;

		if ( runOnStart )
			StartScale(  );
	}

	public void StartScale( )
	{
		StartScale( startDelayMin, startDelayMax );
	}

	public void StartScale( float min, float max )
	{
		startDelay = Random.Range( min, max );
		StartCoroutine( Scale( ) );
	}

	private IEnumerator Scale( )
	{
		float currentTime = 0;
		float durationTime = Random.Range( durationTimeMin, durationTimeMax );
		transform.localScale = startScale;

		yield return new WaitForSeconds( startDelay );

		float progress = 0;
		do
		{
			currentTime += Time.deltaTime;
			currentTime = currentTime < durationTime ? currentTime : durationTime;
			progress = currentTime / durationTime;

			target.localScale = Vector3.Lerp( startScale, endScale, progress );

			yield return null;
		}
		while ( progress < 1 );

		// And we are done
		Done( );
	}

	private void Done( )
	{
		foreach ( var item in toEnableOnDone )
			item.enabled = true;

		onDone.Invoke( );
	}
}
