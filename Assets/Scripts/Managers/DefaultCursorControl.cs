using UnityEngine;

public class DefaultCursorControl : MonoBehaviour
{
    public Texture2D cursorTexture = null;
    public Vector2 hotSpot = Vector2.zero;

    private static DefaultCursorControl current = null;

    void Start()
    {
        current = this;
        reset();
    }

    public static void resetCursor()
    {
        current?.reset();
    }

    public void reset()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }

}
