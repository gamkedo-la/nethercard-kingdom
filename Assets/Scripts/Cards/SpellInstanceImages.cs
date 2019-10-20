using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellInstanceImages : MonoBehaviour
{

    [SerializeField] private SpriteRenderer borderImageSprite;
    [SerializeField] private SpriteRenderer fillImageSprite;
    // Start is called before the first frame update
  

    public void UpdateCardDataFromEditor(Sprite borderImage, Sprite fillImage)
    {
        borderImageSprite.sprite = borderImage;
        fillImageSprite.sprite = fillImage;
    }
}
