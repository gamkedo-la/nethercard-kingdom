using UnityEngine;
using UnityEngine.UI;

public class SlotControl : MonoBehaviour
{
    [SerializeField] private Image emptySlotFill = null;
    [SerializeField] private Color hoverColor = Color.white;

    private Color currentFillColor = Color.white;

    void Start()
    {

    }

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
