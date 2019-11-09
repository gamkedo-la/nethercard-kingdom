using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public struct CursorConfig 
{
    public Texture2D texture;
    public Vector2 hotSpot;
}

public class CursorControl : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler
{
    
    public CursorConfig onMouseOver;
    public CursorConfig onMouseDragging;
    
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        Debug.Log("POINTER ENTER ", this);
        if (!pointerEventData.dragging)
        {
            Cursor.SetCursor(onMouseOver.texture, onMouseOver.hotSpot, CursorMode.Auto);
        }
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        Debug.Log("POINTER EXIT ", this);
        if (!pointerEventData.dragging)
        {
            DefaultCursorControl.ResetCursor();
        }
    }
    
    public void OnDraggingBegin()
    {
        Debug.Log("DRAGGING BEGIN ", this);
        Cursor.SetCursor(onMouseDragging.texture, onMouseDragging.hotSpot, CursorMode.Auto);
    }
    public void OnDraggingEnd()
    {
        Debug.Log("DRAGGING END ", this);
        Cursor.SetCursor(onMouseOver.texture, onMouseOver.hotSpot, CursorMode.Auto);
    }

}
