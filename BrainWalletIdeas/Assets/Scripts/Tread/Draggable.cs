using UnityEngine;

public class Draggable : MonoBehaviour
{
    public enum ObjectType { Good, Bad }
    public ObjectType objectType;

    private bool isDragging = false;
    private Vector3 offset;

    public bool IsDragging => isDragging;

    void OnMouseDown()
    {
        isDragging = true;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mousePos.x, mousePos.y, 0);
    }

    void OnMouseDrag()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x, mousePos.y, 0) + offset;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }
}


