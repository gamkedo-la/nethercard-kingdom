/**
 * Description: Audio-visual functions of the card.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardAudioVisuals : MonoBehaviour
{
	public CardSelectionMode SelectionMode { get; set; }
	//public string Name { get { return displayName; } }
	//public int Cost { get { return useCost; } }
	//public Card LowerLevelVersion { get { return lowerLevelVersion; } }
	//public Card HigherLevelVersion { get { return higherLevelVersion; } }
	//public CardLevel Level { get { return level; } }
	//public CardType CardType { get { return type; } }
	//public GameObject ToSummon { get { return toSummon; } }
	public Sprite CardFill { get { return cardImageFill.sprite; } }
	public Sprite CardBorder { get { return cardImageBorder.sprite; } }
	public string Ability { get { return abilityLabel.text; } }
	public string Flavor { get { return flavorLabel.text; } }
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
	[SerializeField] private Image cardEffectBorder = null;
	[SerializeField] private GameObject level2Marks = null;
	[SerializeField] private GameObject level3Marks = null;

	[Header("Parameters")]
	[SerializeField] private Vector3 overInBuilderScale = Vector3.one * 1.07f;
	[SerializeField] private Vector3 overInHandScale = Vector3.one * 1.07f;
	[SerializeField] private float dragAlpha = 0.9f;

	private Card hoverCard = null;
	private Card draggedCard = null;

	private bool dragging = false;
	private bool over = false;
	private float alpha = 1.0f;

	private float lerpBackTimer = 0f;
	private float sizeIncrease = 0f;
	private bool lerpBack = false;
	private Vector2 mouseOffset = Vector3.one;
	private Vector2 mousePosOld = Vector3.one;

	private Vector3 scaleToLerp = Vector3.one;
	private Vector3 overScale = Vector3.one;
	private Vector3 defaultScale = Vector3.one;
	private Vector3 previousPosition = Vector3.zero;
	private bool selected = false;
	private bool canBeUnselected = false;

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
		Assert.IsNotNull( cardEffectBorder, $"Please assign <b>{nameof( cardEffectBorder )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( level2Marks, $"Please assign <b>{nameof( level2Marks )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( level3Marks, $"Please assign <b>{nameof( level3Marks )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		overScale = overInBuilderScale; // TODO: Get info from CardNew and set the correct scale

		PopulateCardInfo( );
	}

	void Update( )
	{
		transform.localScale = Vector3.Lerp( transform.localScale, scaleToLerp, 0.25f );
		canvasGroup.alpha = Mathf.Lerp( canvasGroup.alpha, alpha, 0.15f );
		//transform.position = Vector2.Lerp( transform.position, Position, 0.15f );

		if ( dragging && SelectionMode == CardSelectionMode.InHand )
		{
			/*transform.position = Vector2.Lerp( transform.position, Input.mousePosition, 0.25f );
			canvasGroup.alpha = Mathf.Lerp( canvasGroup.alpha, 0.0f, 0.15f );
			liveImage.alpha = Mathf.Lerp( liveImage.alpha, 0.5f, 0.15f );
			transform.localScale = Vector3.one;*/
		}
		else if ( dragging && ( SelectionMode == CardSelectionMode.InCollection || SelectionMode == CardSelectionMode.InDeck ) )
		{
			Vector2 mouseNewPos = (Vector2)Input.mousePosition + mouseOffset;
			//Vector2 moveOffset = mousePosOld - mouseNewPos;
			//mousePosOld = mouseNewPos;

			//Vector2 newPos = transform.position - (Vector3)moveOffset;
			//Debug.Log( newPos );
			mouseNewPos += new Vector2( 0.0f, -Screen.height * 0.1f );

			//transform.position = Vector2.Lerp( transform.position, mouseNewPos, 0.25f );
			//transform.position = newPos;
			//transform.position = mouseNewPos;
			//Position = mouseNewPos;
		}

		/*}
		else
		{
			if ( selectionMode == CardSelectionMode.InHand )
			{
				if ( SummoningManager.Instance.EnoughMana( useCost ) )
					canvasGroup.alpha = 1f;
				else
					canvasGroup.alpha = 0.9f;

				liveImage.alpha = Mathf.Lerp( liveImage.alpha, 0f, 0.25f );
			}

			if ( lerpBackTimer <= 0f || !lerpBack )
				transform.localScale = Vector3.Lerp( transform.localScale, scaleToLerp, 0.25f );
		}*/

		lerpBackTimer -= Time.deltaTime;
	}

	public void DoCardReveal( )
	{
		Revealing = true;
		//animator.enabled = true;
		animator.SetTrigger( "Reveal" );
	}

	public void CardRevealDone( )
	{
		Revealing = false;
		//animator.enabled = false;
	}

	public void OnWarning( )
	{
		animator.SetFloat( "StartPosition", Random.Range( 0f, 1f ) );
		animator.SetBool( "Shake", true );

		cardEffectBorder.transform.localScale = Vector3.one;

		Color c = cardEffectBorder.color;
		c.a = 1;
		cardEffectBorder.color = c;

		sizeIncrease = 0.2f;
		float duration = 0.2f;
		StartCoroutine( Utilities.ChangeOverTime( duration, BorderEffect ) );

		float warningDuration = 0.3f;
		Invoke( nameof( OffWarning ), warningDuration );
	}

	public void OnInformation( )
	{
		cardEffectBorder.transform.localScale = Vector3.one;

		Color c = cardEffectBorder.color;
		c.a = 1;
		cardEffectBorder.color = c;

		sizeIncrease = 0.2f;
		float duration = 0.5f;
		StartCoroutine( Utilities.ChangeOverTime( duration, BorderEffect ) );
	}

	private void BorderEffect( float progress )
	{
		cardEffectBorder.transform.localScale = Vector3.one * ( 1.0f + sizeIncrease * progress);
		Color c = cardEffectBorder.color;
		c.a = 1 - progress;
		cardEffectBorder.color = c;
	}

	public void OffWarning( )
	{
		animator.SetBool( "Shake", false );
		//animator.enabled = false;
	}

	public void OnOverEnter( )
	{
		//over = true;
		//ShowBigger( );

		/*if ( selectionMode == CardSelectionMode.InHand )
		{
			scaleToLerp = Vector3.one * 1.3f;
			lerpBack = false;
			lerpBackTimer = 0.1f;
			frontCanvas.overrideSorting = true;
			frontCanvas.sortingOrder = 1100;

			if ( hoverCard == null )
			{
				overSound.Play( );
				hoverCard = this;
			}
		}
		else if ( selectionMode == CardSelectionMode.InCollection )
		{
			lerpBack = false;
			lerpBackTimer = 0.1f;
			scaleToLerp = defaultScale * 1.1f;

			if ( DeckBuilder.Instance.IsDeckCardSelected( ) )
			{
				DeckBuilder.Instance.CheckCollectionCardSelection( this );

				scaleToLerp = defaultScale * 1.5f;
			}
		}
		else if ( selectionMode == CardSelectionMode.InDeck )
		{
			lerpBack = false;
			lerpBackTimer = 0.1f;
			scaleToLerp = defaultScale * 1.1f;

			if ( DeckBuilder.Instance.IsCollectionCardSelected( ) )
			{
				DeckBuilder.Instance.CheckDeckCardSelection( this );

				scaleToLerp = defaultScale * 1.5f;
			}
		}*/
	}

	public void HighlightCard( )
	{
		defaultScale = overScale;
		scaleToLerp = defaultScale;

		frontCanvas.overrideSorting = true;
		frontCanvas.sortingOrder = 10100;
	}

	public void NormalCard( )
	{
		defaultScale = Vector3.one;
		scaleToLerp = defaultScale;

		frontCanvas.overrideSorting = false;
		frontCanvas.sortingOrder = 0;
		canvasGroup.blocksRaycasts = true;
		canvasGroup.interactable = true;
	}

	public void DraggedCard( )
	{
		frontCanvas.overrideSorting = true;
		frontCanvas.sortingOrder = 10200;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.interactable = false;
	}

	public void OnOverExit( )
	{
		//over = false;
		//ShowNormal( );

		/*if ( draggedCard == this )
			return;

		if ( selectionMode == CardSelectionMode.InHand )
		{
			scaleToLerp = Vector3.one;
			lerpBack = true;

			frontCanvas.overrideSorting = false;
			frontCanvas.sortingOrder = 0;

			if ( hoverCard == this )
				hoverCard = null;
		}
		else if ( selectionMode == CardSelectionMode.InCollection || selectionMode == CardSelectionMode.InDeck )
		{
			scaleToLerp = defaultScale;
			lerpBack = true;

			DeckBuilder.Instance.CompareAndRemoveSelection( this );
		}*/
	}

	public void OnBeginDrag( )
	{
		/*dragging = true;
		alpha = dragAlpha;
		//canvasGroup.blocksRaycasts = false;

		frontCanvas.overrideSorting = true;
		frontCanvas.sortingOrder = 10200;

		mouseOffset = transform.position - Input.mousePosition;
		mousePosOld = Input.mousePosition;*/
		/*onStartedDrag?.Invoke( );

		if ( selectionMode == CardSelectionMode.InHand )
			StartSummoning( );
		else if ( selectionMode == CardSelectionMode.InCollection || selectionMode == CardSelectionMode.InDeck )
			StartDraggingInDeckBuilding( );*/
	}

	public void OnEndDrag( )
	{
		/*dragging = false;
		alpha = 1.0f;

		ShowNormal( );*/
		//canvasGroup.blocksRaycasts = true;

		/*onEndedDrag?.Invoke( );

		scaleToLerp = defaultScale;
		lerpBack = true;

		if ( selectionMode == CardSelectionMode.InHand )
			EndSummoning( );
		else if ( selectionMode == CardSelectionMode.InCollection || selectionMode == CardSelectionMode.InDeck )
			EndDraggingInDeckBuilding( );*/
	}

	public void OnCliked( )
	{
		/*if ( selectionMode == CardSelectionMode.InHand )
		{
			if ( selected )
			{
				selected = false;
				EndSummoning( );
			}
			else
			{
				selected = true;
				StartSummoning( );
			}
		}
		else if ( selectionMode == CardSelectionMode.InCollection || selectionMode == CardSelectionMode.InDeck )
		{
			if ( !selected && !draggedCard )
			{
				selected = true;
				Invoke( nameof( CanBeUnselected ), 0.01f );
				StartDraggingInDeckBuilding( );
			}
		}*/
	}

	public void OnReleased( )
	{
		/*if ( selectionMode == CardSelectionMode.InHand )
		{ }
		else if ( selectionMode == CardSelectionMode.InCollection || selectionMode == CardSelectionMode.InDeck )
		{ }*/
	}

	public void UpdateCardStatsFromEditor( CardType cardType, CardLevel cardLevel, string name, int cost,
		string ability, string flavor, Sprite borderSprite, Sprite fillSprite, GameObject instanceToSummon )
	{
		/*type = cardType;
		level = cardLevel;
		displayName = name;
		useCost = cost;
		abilityText = ability;
		flavorText = flavor;
		cardImageBorder.sprite = borderSprite;
		cardImageFill.sprite = fillSprite;
		toSummon = instanceToSummon;*/
	}

	[ContextMenu( "Update Card Info" )]
	public void PopulateCardInfo( )
	{
		/*var specificCulture = System.Globalization.CultureInfo.GetCultureInfo( "en-US" );

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
			level3Marks.SetActive( true );*/
	}

	private void ShowNormal( )
	{
		if ( over )
			return;

		defaultScale = Vector3.one;
		scaleToLerp = defaultScale;

		if ( !dragging )
		{
			frontCanvas.overrideSorting = false;
			frontCanvas.sortingOrder = 0;
		}
	}

	private void ShowBigger( )
	{
		defaultScale = overScale;
		scaleToLerp = defaultScale;

		frontCanvas.overrideSorting = true;
		frontCanvas.sortingOrder = 10100;
	}

	private void StartSummoning( )
	{
		//if ( !SummoningManager.Instance.EnoughMana( useCost ) )
			//return;

		//draggedCard = this;
		OnOverEnter( );

		//SummoningManager.Instance.Summoning( Camera.main.ScreenToWorldPoint( Input.mousePosition ), type, true );
		playSound.Play( );
	}

	private void EndSummoning( )
	{
		if ( draggedCard != this )
			return;

		canvasGroup.alpha = 1f;
		draggedCard = null;
		OnOverExit( );

		//bool canSummon = SummoningManager.Instance.Summoning( Vector2.zero, type, false );

		/*if ( canSummon )
		{
			GameObject instance = Instantiate( toSummon, (Vector2)Camera.main.ScreenToWorldPoint( Input.mousePosition ), Quaternion.identity );
			if ( type == CardType.DirectDefensiveSpell || type == CardType.DirectOffensiveSpell || type == CardType.AoeSpell )
				instance.GetComponent<Spell>( ).SetTarget( SummoningManager.Instance.LastTarget );

			SummoningManager.Instance.RemoveMana( useCost );
			Destroy( gameObject );
		}
		else
			backSound.Play( );*/
	}

	private void StartDraggingInDeckBuilding( )
	{
		/*draggedCard = this;

		DeckBuilder.Instance.CheckCollectionCardSelection( this );
		DeckBuilder.Instance.CheckDeckCardSelection( this );

		previousPosition = transform.position;

		scaleToLerp = defaultScale * 1.25f;

		frontCanvas.overrideSorting = true;
		frontCanvas.sortingOrder = 999999;*/
	}

	private void EndDraggingInDeckBuilding( )
	{
		transform.position = previousPosition;
		canvasGroup.alpha = 1.0f;

		frontCanvas.overrideSorting = false;
		frontCanvas.sortingOrder = 100000;

		draggedCard = null;
		//DeckBuilder.Instance.MoveSlot( );
	}
}
