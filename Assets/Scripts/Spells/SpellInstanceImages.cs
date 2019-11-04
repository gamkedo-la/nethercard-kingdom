using UnityEngine;
using UnityEngine.Assertions;

public class SpellInstanceImages : MonoBehaviour
{
    [SerializeField] private SpriteRenderer borderImageSprite = null;
    [SerializeField] private SpriteRenderer fillImageSprite = null;

	void Start( )
	{
		Assert.IsNotNull( borderImageSprite, $"Please assign <b>{nameof( borderImageSprite )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		Assert.IsNotNull( fillImageSprite, $"Please assign <b>{nameof( fillImageSprite )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
	}

	public void UpdateCardDataFromEditor(Sprite borderImage, Sprite fillImage)
    {
        borderImageSprite.sprite = borderImage;
        fillImageSprite.sprite = fillImage;
    }
}
