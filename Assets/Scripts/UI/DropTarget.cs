using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropTarget : MonoBehaviour, IDropHandler
{
    private DraggableElement occupant;

    void Start()
    {
        occupant = GetComponentInChildren<DraggableElement>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(gameObject);
        if (eventData.pointerDrag.tag == transform.tag)
        {
            if (occupant != null)
            {
                occupant.ResetParent();
            }
            eventData.pointerDrag.transform.SetParent(transform);
            occupant = eventData.pointerDrag.GetComponent<DraggableElement>();
        }
    }
}
