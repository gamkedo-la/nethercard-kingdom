/**
 * Description: Plays a sound on instantiation or scene load.
 * Authors: Kornel
 * Copyright: © 2018-2019 Kornel. All rights reserved. For license see: 'LICENSE' file.
 **/

using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent( typeof( AudioSource ) )]
public class PlaySound : MonoBehaviour
{
	[Header("External objects")]
	[SerializeField] private AudioClip[] sounds = null;

	[Header("Sound parameters")]
	[SerializeField] private float minPitch = 0.8f;
	[SerializeField] private float maxPitch = 1.2f;
	[SerializeField] private float minVolume = 0.9f;
	[SerializeField] private float maxVolume = 1.0f;

	[Header("Activation")]
	[SerializeField] private bool playOnStart = true;
	[SerializeField] private bool limitFequencyOfPlays = false;
	[SerializeField, Tooltip("Time to wait before allowing another play")] private float timeBetweenPlays = 1f;

	private AudioSource audioSource = null;
	private float currentLimitBetweenSounds = 0f;

	void Start( )
	{
		audioSource = GetComponent<AudioSource>( );

		Assert.IsNotNull( audioSource, $"Please assign <b>{nameof( audioSource )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( sounds, $"Please assign <b>{nameof( sounds )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		//Assert.AreNotEqual( sounds.Length, 0, $"Please assign <b>{nameof( sounds )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		if ( sounds.Length == 0 )
			Debug.LogWarning( "No sound added to " + transform.parent.name );
		else if ( playOnStart )
			Play( );
	}

	void Update( )
	{
		if ( limitFequencyOfPlays )
			currentLimitBetweenSounds -= Time.deltaTime;
	}

	/// <summary>
	/// Play the assigned sound(s).
	/// </summary>
	public void Play( )
	{
		Play( Random.Range( minPitch, maxPitch ) );
	}

	/// <summary>
	/// Play the assigned sound(s). Useful for playing sounds with ever raising pitch.
	/// </summary>
	/// <param name="pitch">Desired pitch.</param>
	public void Play( float pitch )
	{
		if ( currentLimitBetweenSounds > 0 )
			return;

		if ( limitFequencyOfPlays )
			currentLimitBetweenSounds = timeBetweenPlays;

		audioSource.clip = sounds[Random.Range( 0, sounds.Length )];
		audioSource.pitch = pitch;
		audioSource.volume = Random.Range( minVolume, maxVolume );
		audioSource.Play( );
	}
}
