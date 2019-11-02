using UnityEngine;
using UnityEngine.EventSystems;

public class CursorControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture2D cursorTexture = null;
    public Vector2 hotSpot = Vector2.zero;
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Debug.Log("POINTER ENTER");
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        // Debug.Log("POINTER EXIT");
        //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        DefaultCursorControl.resetCursor();
    }

}
