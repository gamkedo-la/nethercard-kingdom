using UnityEngine;

public class DefaultCursorControl : MonoBehaviour
{
    public Texture2D cursorTexture = null;
    public Vector2 hotSpot = Vector2.zero;

    private static DefaultCursorControl current = null;

    void Start()
    {
        current = this;
        Reset();
    }

    public static void ResetCursor()
    {
        current?.Reset();
    }

    public void Reset()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }

}
