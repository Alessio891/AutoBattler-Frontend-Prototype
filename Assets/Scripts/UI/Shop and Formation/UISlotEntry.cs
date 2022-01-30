using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UISlotEntry : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Image ItemIcon;
    public Button DisposeButton;


    GameObject draggedObj;
    bool Dragging = false;

    public virtual void Start()
    { }

    public virtual void Empty()
    {
        ItemIcon.sprite = null;
        ItemIcon.enabled = false;
        DisposeButton.gameObject.SetActive(false);
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (ItemIcon.enabled && ItemIcon.sprite != null)
        {
            Dragging = true;
            draggedObj = GameObject.Instantiate(ItemIcon.gameObject);
            Transform mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").transform;
            draggedObj.transform.SetParent(mainCanvas);
            draggedObj.transform.localScale = Vector3.one;
            draggedObj.transform.position = eventData.position;
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (Dragging)
            draggedObj.transform.position = eventData.position;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (Dragging)
        {
            Destroy(draggedObj);
            Dragging = false;
        }
    }
}
