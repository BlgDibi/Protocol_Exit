using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture2D interactCursorTexture;
    public Vector2 hotSpot = new Vector2(16, 16);

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(interactCursorTexture, hotSpot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}