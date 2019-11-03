/**
 * Description: Adds health functionality to a GameObject.
 * Authors: Kornel
 * Copyright: © 2018-2019 Kornel. All rights reserved. For license see: 'LICENSE' file.
 **/

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
	public float CurrentHP { get; private set; }
	public float MaxHP { get { return maxHP; } }

	[Header("External/optional objects")]
	[SerializeField, Tooltip("An optional Slider that acts as an HP Bar.")] private Slider hpBar = null;
	[SerializeField, Tooltip("An optional Slider that acts as an under HP Bar for health shrink effect.")] private Slider hpBarUnder = null;
	[SerializeField, Tooltip( "An optional label to display health." )] private TextMeshProUGUI label = null;
	[SerializeField] private GameObject floatingTextDamage = null;
	[SerializeField] private GameObject floatingTextHeal = null;

	[Header("Tweakable")]
	[SerializeField] private float maxHP = 10;
	[SerializeField] private bool destroyOnNoHP = true;
	[SerializeField] private bool delayNextDamage = false;
	[SerializeField] private float nextDamageDelay = 1f;
	[SerializeField] private bool hideHpBar = false;
	[SerializeField] private bool scaleOnDamage = true;
	[SerializeField] private Vector3 scaleFactor = Vector3.one * 2f;
	[SerializeField] private float scaleTime = 0.3f;
	[SerializeField] private float underChangeTime = 1f;
	[SerializeField] private bool floatingTextLeftDir = false;

	[Header("Events")]
	[SerializeField] private UnityEvent onHealthChange = null;
	[SerializeField] private UnityEvent onDamage = null;
	[SerializeField] private UnityEvent onHeal = null;
	[SerializeField] private UnityEvent onDeath = null;

	private bool canBeDamaged = true;
	private float oldUnderHPValue = 0;
	private Vector3 hpBarOldScale;
	private Vector3 hpBarUnderOldScale;

	void Start( )
	{
		CurrentHP = maxHP;

		if ( hpBar )
		{
			hpBar.maxValue = maxHP;
			hpBar.value = maxHP;
			if ( hideHpBar )
				hpBar.gameObject.SetActive( false );

			hpBarOldScale = hpBar.transform.localScale;
		}

		if ( hpBarUnder )
		{
			hpBarUnder.maxValue = maxHP;
			hpBarUnder.value = maxHP;
			if ( hideHpBar )
				hpBarUnder.gameObject.SetActive( false );

			hpBarUnderOldScale = hpBarUnder.transform.localScale;
		}

		if ( label )
			label.text = $"{CurrentHP}/{maxHP}";
	}

	/// <summary>
	/// Applies damage to current HP. Respects HP restrictions and fires events if necessary.
	/// </summary>
	/// <param name="damage">Amount of taken damage.</param>
	public void DoDamage( float damage, Vector2 contact, bool special = false )
	{
		if ( !canBeDamaged )
			return;

		if ( !special )
			onDamage.Invoke( );

		ChangeHP( -damage );

		if ( hpBar && !hpBar.gameObject.activeSelf && hideHpBar )
		{
			hpBar.gameObject.SetActive( true );

			if ( hpBarUnder )
				hpBarUnder.gameObject.SetActive( true );
		}

		if ( floatingTextDamage )
		{
			GameObject go = Instantiate( floatingTextDamage, contact, Quaternion.identity );
			FloatingText ft = go.GetComponent<FloatingText>( );
			ft.SetPrameters( damage.ToString( ), floatingTextLeftDir, 1.0f, 1.0f, Color.white );
		}

		if ( delayNextDamage )
		{
			canBeDamaged = false;
			Invoke( nameof( CanBeDamaged ), nextDamageDelay );
		}
	}

	/// <summary>
	/// Adds health. Respects HP restrictions and fires events if necessary.
	/// </summary>
	/// <param name="change">Amount of HP change. Negative values for damage, positive for healing.</param>
	public void Heal( float heal )
	{
		onHeal.Invoke( );

		if ( floatingTextHeal )
		{
			float healed = heal;
			if ( CurrentHP + heal > maxHP )
				healed = maxHP - CurrentHP;

			//if ( healed > 0 )
			//{
				GameObject go = Instantiate( floatingTextHeal, transform.position, Quaternion.identity );
				FloatingText ft = go.GetComponent<FloatingText>( );
				ft.SetPrameters( $"+{healed}", floatingTextLeftDir, 1.0f, 1.0f, Color.white );
			//}
		}

		ChangeHP( heal );
	}

	/// <summary>
	/// Changes current HP. Respects HP restrictions and fires events if necessary.
	/// </summary>
	/// <param name="change">Amount of HP change. Negative values for damage, positive for healing.</param>
	private void ChangeHP( float change )
	{
		oldUnderHPValue = CurrentHP;
		CurrentHP += change;
		CurrentHP = CurrentHP > maxHP ? maxHP : CurrentHP;
		CurrentHP = CurrentHP < 0 ? 0 : CurrentHP;

		onHealthChange.Invoke( );

		if ( label )
			label.text = $"{CurrentHP}/{maxHP}";

		if ( hpBar )
			hpBar.value = CurrentHP;

		StopAllCoroutines( );
		if ( hpBarUnder && change < 0 )
			StartCoroutine( Utilities.ChangeOverTime( underChangeTime, OnUnderHPBarValueChange ) );

		if ( scaleOnDamage && change < 0 )
		{
			if ( hpBar )
				hpBar.transform.localScale = new Vector3( hpBarOldScale.x * scaleFactor.x, hpBarOldScale.y * scaleFactor.y, hpBarOldScale.z * scaleFactor.z );

			if ( hpBarUnder )
				hpBarUnder.transform.localScale = new Vector3( hpBarUnderOldScale.x * scaleFactor.x, hpBarUnderOldScale.y * scaleFactor.y, hpBarUnderOldScale.z * scaleFactor.z );

			StartCoroutine( Utilities.ChangeOverTime( scaleTime, OnScaleValueChange ) );
		}

		if ( CurrentHP <= 0 )
		{
			DestroyMe( );
		}
		else if ( CurrentHP == MaxHP && hideHpBar )
		{
			if ( hpBar )
				hpBar.gameObject.SetActive( false );

			if ( hpBarUnder )
				hpBarUnder.gameObject.SetActive( false );
		}
		else if ( CurrentHP > 0 && hideHpBar )
		{
			if ( hpBar )
				hpBar.gameObject.SetActive( true );

			if ( hpBarUnder )
				hpBarUnder.gameObject.SetActive( true );
		}
	}

	private void OnUnderHPBarValueChange( float percent )
	{
		hpBarUnder.value = CurrentHP + ( oldUnderHPValue - CurrentHP ) * ( 1 - percent );
	}

	private void OnScaleValueChange( float percent )
	{
		if ( hpBar )
			hpBar.transform.localScale = new Vector3
			(
				hpBarOldScale.x * ( 1 + ( scaleFactor.x - 1 ) * ( 1 - percent ) ),
				hpBarOldScale.y * ( 1 + ( scaleFactor.y - 1 ) * ( 1 - percent ) ),
				hpBarOldScale.z * ( 1 + ( scaleFactor.z - 1 ) * ( 1 - percent ) )
			);

		if ( hpBarUnder )
			hpBarUnder.transform.localScale = new Vector3
			(
				hpBarUnderOldScale.x * ( 1 + ( scaleFactor.x - 1 ) * ( 1 - percent ) ),
				hpBarUnderOldScale.y * ( 1 + ( scaleFactor.y - 1 ) * ( 1 - percent ) ),
				hpBarUnderOldScale.z * ( 1 + ( scaleFactor.z - 1 ) * ( 1 - percent ) )
			);
	}

	private void DestroyMe( )
	{
		onDeath.Invoke( );

		if ( !destroyOnNoHP )
			return;

		Destroy( gameObject );
	}

	private void CanBeDamaged( )
	{
		canBeDamaged = true;
	}
}
