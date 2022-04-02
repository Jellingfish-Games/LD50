using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableElement : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    private bool dragging = false;

    private float reparentingLerp;

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }


    // Start is called before the first frame update
    void Start()
    {
        
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
            transform.localPosition = Mathf.Lerp(localPosition, Vector3.zero, reparentingLerp);
        }
    }
}
