using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotControl : MonoBehaviour
{
    [SerializeField] private Image emptySlotFill;
    [SerializeField] private Color hoverColor;

    private Color currentFillColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        emptySlotFill.color = Color.Lerp(emptySlotFill.color, currentFillColor, 0.25f);
    }

    public void OnHoverEnter()
    {
        currentFillColor = hoverColor;
    }

    public void OnHoverExit()
    {
        currentFillColor = Color.white;
    }

    public void OnClick()
    {
    }

    public void OnRelease()
    {
        DeckBuilder.Instance.otherSlot = transform.parent.gameObject;
        DeckBuilder.Instance.MoveSlot();
    }
}
