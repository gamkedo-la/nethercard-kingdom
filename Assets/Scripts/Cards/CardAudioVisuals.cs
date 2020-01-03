/**
 * Description: Audio-visual functions of the card.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CardAudioVisuals : MonoBehaviour
{
	public CardSelectionMode SelectionMode { get; set; }
	public Sprite CardFill { get { return cardImageFill.sprite; } }
	public Sprite CardBorder { get { return cardImageBorder.sprite; } }
	public bool Revealing { get; private set; } = false;
	public Vector2 Position { get; set; }

	[Header("External Objects")]
	[SerializeField] private PlaySound playSound = null;
	[SerializeField] private PlaySound backSound = null;
	[SerializeField] private PlaySound overSound = null;
	[SerializeField] private Animator animator = null;

	[Header("Card Elements")]
	[SerializeField] private Canvas frontCanvas = null;
	[SerializeField] private Canvas backCanvas = null;
	[SerializeField] private CanvasGroup canvasGroup = null;
	[SerializeField] private CanvasGroup liveImage = null;
	[SerializeField] private GameObject statisticsPanel = null;
	[SerializeField] private TextMeshProUGUI manaCostLabel = null;
	[SerializeField] private TextMeshProUGUI nameLabel = null;
	[SerializeField] private TextMeshProUGUI attackLabel = null;
	[SerializeField] private TextMeshProUGUI hpLabel = null;
	[SerializeField] private TextMeshProUGUI speedLabel = null;
	[SerializeField] private TextMeshProUGUI abilityLabel = null;
	[SerializeField] private TextMeshProUGUI flavorLabel = null;
	[SerializeField] private Image cardImageFill = null;
	[SerializeField] private Image cardImageBorder = null;
	[SerializeField] private Image shockwaveSprite = null;
	[SerializeField] private GameObject level2Marks = null;
	[SerializeField] private GameObject level3Marks = null;
	[SerializeField] private GameObject stack1 = null;
	[SerializeField] private GameObject stack2 = null;
	[SerializeField] private GameObject classMelee = null;
	[SerializeField] private GameObject classRanged = null;
	[SerializeField] private GameObject classSupport = null;
	[SerializeField] private GameObject classSpell = null;

	[Header("Parameters")]
	[SerializeField] private float overInBuilderScale = 1.2f;
	[SerializeField] private float overInHandScale = 1.4f;
	[SerializeField] private float alphaOnDraggedFromHand = 0.8f;
	[SerializeField] private float alphaOnDraggedInDeckBuilder = 0.8f;
	[SerializeField] private float alphaOnCanNotBePlayed = 0.5f;
	[SerializeField] private float liveImageScaleMod = 1.0f;

	private bool showPreview = false;
	private bool canBePlayed = true;
	private float cardAlpha = 1.0f;
	private float sizeIncrease = 0f;
	private Vector3 cardScale = Vector3.one;

	void Start( )
	{
		Assert.IsNotNull( playSound, $"Please assign <b>{nameof( playSound )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( backSound, $"Please assign <b>{nameof( backSound )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( overSound, $"Please assign <b>{nameof( overSound )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		Assert.IsNotNull( frontCanvas, $"Please assign <b>{nameof( frontCanvas )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( backCanvas, $"Please assign <b>{nameof( backCanvas )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( canvasGroup, $"Please assign <b>{nameof( canvasGroup )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( liveImage, $"Please assign <b>{nameof( liveImage )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( animator, $"Please assign <b>{nameof( animator )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		Assert.IsNotNull( statisticsPanel, $"Please assign <b>{nameof( statisticsPanel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( manaCostLabel, $"Please assign <b>{nameof( manaCostLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( nameLabel, $"Please assign <b>{nameof( nameLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( attackLabel, $"Please assign <b>{nameof( attackLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( hpLabel, $"Please assign <b>{nameof( hpLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( speedLabel, $"Please assign <b>{nameof( speedLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( abilityLabel, $"Please assign <b>{nameof( abilityLabel )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( cardImageFill, $"Please assign <b>{nameof( cardImageFill )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( cardImageBorder, $"Please assign <b>{nameof( cardImageBorder )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( shockwaveSprite, $"Please assign <b>{nameof( shockwaveSprite )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( level2Marks, $"Please assign <b>{nameof( level2Marks )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( level3Marks, $"Please assign <b>{nameof( level3Marks )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( stack1, $"Please assign <b>{nameof( stack1 )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( stack2, $"Please assign <b>{nameof( stack2 )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( classMelee, $"Please assign <b>{nameof( classMelee )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( classRanged, $"Please assign <b>{nameof( classRanged )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( classSupport, $"Please assign <b>{nameof( classSupport )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( classSpell, $"Please assign <b>{nameof( classSpell )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	void Update( )
	{
		transform.localScale = Vector3.Lerp( transform.localScale, cardScale, 0.25f );
		canvasGroup.alpha = Mathf.Lerp( canvasGroup.alpha, cardAlpha, 0.15f );

		// Show and drag preview
		if ( showPreview )
		{
			liveImage.alpha = Mathf.Lerp( liveImage.alpha, 0.5f, 0.15f );
			liveImage.transform.rotation = Quaternion.identity;
			liveImage.transform.position = Input.mousePosition;
		}
		// Return preview to card position
		else if (!showPreview && Vector2.Distance( liveImage.transform.localPosition, Vector2.zero) > 1 )
		{
			liveImage.transform.localPosition = Vector2.Lerp( liveImage.transform.localPosition, Vector2.zero, 0.1f );
			liveImage.transform.rotation = Quaternion.identity;
			liveImage.alpha = Mathf.Lerp( liveImage.alpha, 0f, 0.1f );
		}
	}

	public void DoCardReveal( )
	{
		Revealing = true;
		animator.SetTrigger( "Reveal" );
	}

	public void CardRevealDone( )
	{
		Revealing = false;
	}

	public void OnWarning( )
	{
		animator.SetFloat( "StartPosition", Random.Range( 0f, 1f ) );
		animator.SetBool( "Shake", true );

		shockwaveSprite.gameObject.SetActive( true );
		shockwaveSprite.transform.localScale = Vector3.one;

		Color c = shockwaveSprite.color;
		c.a = 1;
		shockwaveSprite.color = c;

		sizeIncrease = 0.2f;
		float duration = 0.2f;
		StartCoroutine( Utilities.ChangeOverTime( duration, BorderEffect ) );

		float warningDuration = 0.3f;
		Invoke( nameof( OffWarning ), warningDuration );
	}

	public void OnInformation( )
	{
		shockwaveSprite.gameObject.SetActive( true );
		shockwaveSprite.transform.localScale = Vector3.one;

		Color c = shockwaveSprite.color;
		c.a = 1;
		shockwaveSprite.color = c;

		sizeIncrease = 0.2f;
		float duration = 0.5f;
		StartCoroutine( Utilities.ChangeOverTime( duration, BorderEffect ) );
	}

	public void OffWarning( ) => animator.SetBool( "Shake", false );

	public void CanBePlayed( bool canBePlayed )
	{
		this.canBePlayed = canBePlayed;
		cardAlpha = canBePlayed ? 1.0f : alphaOnCanNotBePlayed;
	}

	public void ShowPreview( bool show )
	{
		showPreview = show;
		liveImage.transform.rotation = Quaternion.identity;
		liveImage.transform.localScale = ( Vector3.one / overInHandScale ) * liveImageScaleMod;
	}

	public void HighlightCardInDeck( )
	{
		Enlarge( overInBuilderScale );
	}

	public void HighlightCardInHand( )
	{
		Enlarge( overInHandScale );
	}

	public void DraggedFromHand( )
	{
		cardAlpha = alphaOnDraggedFromHand;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.interactable = false;

		playSound.Play( );
	}

	public void SetDisabled( )
	{
		canBePlayed = false;
		cardAlpha = alphaOnCanNotBePlayed;
		canvasGroup.alpha = alphaOnCanNotBePlayed;

		canvasGroup.blocksRaycasts = false;
		canvasGroup.interactable = false;
	}

	public void NormalCard( )
	{
		cardScale = Vector3.one;
		cardAlpha = canBePlayed ? 1.0f : alphaOnCanNotBePlayed;

		frontCanvas.overrideSorting = false;
		frontCanvas.sortingOrder = 0;
		canvasGroup.blocksRaycasts = true;
		canvasGroup.interactable = true;
	}

	public void DraggedCard( bool backMove )
	{
		cardAlpha = alphaOnDraggedInDeckBuilder;

		frontCanvas.overrideSorting = true;
		frontCanvas.sortingOrder = 10200;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.interactable = false;

		if ( backMove )
			Back( );
		else
			playSound.Play( );
	}

	public void Back( )
	{
		backSound.Play( );
	}

	public void SetStack( int amount )
	{
		stack1.SetActive( amount >= 2 );
		stack2.SetActive( amount >= 3 );
	}

	public void UpdateCardStatsFromEditor( Sprite borderSprite, Sprite fillSprite )
	{
		cardImageBorder.sprite = borderSprite;
		cardImageFill.sprite = fillSprite;
	}

	public void PopulateCardInfo( CardType type, GameObject toSummon, int useCost, string displayName, string abilityText, string flavorText, CardLevel level, CardClass cardClass )
	{
		var specificCulture = System.Globalization.CultureInfo.GetCultureInfo( "en-US" );

		if ( type == CardType.Unit )
		{
			statisticsPanel.SetActive( true );

			Unit u = toSummon.GetComponent<Unit>( );
			attackLabel.text = u.DPS.ToString( "0.0", specificCulture );
			hpLabel.text = u.HP.MaxHP.ToString( );
			speedLabel.text = u.MoveSpeed.ToString( "0.0", specificCulture );
		}
		else
		{
			statisticsPanel.SetActive( false );
		}

		manaCostLabel.text = useCost.ToString( );
		nameLabel.text = displayName;
		abilityLabel.text = abilityText;
		flavorLabel.text = flavorText;

		level2Marks.SetActive( false );
		level3Marks.SetActive( false );

		if ( level == CardLevel.Level2 )
			level2Marks.SetActive( true );
		else if ( level == CardLevel.Level3 )
			level3Marks.SetActive( true );

		classMelee.SetActive( false );
		classRanged.SetActive( false );
		classSupport.SetActive( false );
		classSpell.SetActive( false );

		if ( cardClass == CardClass.Melee )
			classMelee.SetActive( true );
		else if ( cardClass == CardClass.Ranged )
			classRanged.SetActive( true );
		else if ( cardClass == CardClass.Support )
			classSupport.SetActive( true );
		else
			classSpell.SetActive( true );
	}

	public void EndSummoning( ) => canvasGroup.alpha = 1f;

	public void CancelSummoning( )
	{
		canvasGroup.alpha = 1f;
		backSound.Play( );
	}

	private void Enlarge( float scale )
	{
		cardScale = Vector3.one * scale;

		frontCanvas.overrideSorting = true;
		frontCanvas.sortingOrder = 10100;

		overSound.Play( );
	}

	private void BorderEffect( float progress )
	{
		shockwaveSprite.transform.localScale = Vector3.one * ( 1.0f + sizeIncrease * progress );
		Color c = shockwaveSprite.color;
		c.a = 1 - progress;
		shockwaveSprite.color = c;
	}
}
