using UnityEngine;

public class DropZone : MonoBehaviour
{
    public Draggable.ObjectType acceptedType; // Good or Bad

    private void OnTriggerStay2D(Collider2D other)
    {
        Draggable draggable = other.GetComponent<Draggable>();
        if (draggable != null && draggable.IsDragging) // only while dragging
        {
            if (draggable.objectType == acceptedType)
            {
                Debug.Log("Correct object dragged into " + gameObject.name);
                Destroy(other.gameObject); // or score points
            }
            else
            {
                Debug.Log("Wrong object dragged into " + gameObject.name);
                // Optionally: feedback like shaking, red flash, etc.
            }
        }
    }
}

