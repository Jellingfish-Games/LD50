using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableElement : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private bool dragging = false;

    private float reparentingLerp;

    internal Transform originalParent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        reparentingLerp = 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (transform.parent != originalParent)
        {
            transform.SetParent(originalParent);
            reparentingLerp = 0;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void ResetParent()
    {
        transform.SetParent(originalParent);
        reparentingLerp = 0;
    }

    void Start()
    {
        originalParent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            transform.position = Input.mousePosition;
        } else
        {
            reparentingLerp += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, reparentingLerp);
        }
    }
}
