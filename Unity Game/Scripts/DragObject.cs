using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragObject : MonoBehaviour, IDragHandler,IEndDragHandler //allow an object to be dragged and dropped inside a unity scene
{



    internal bool isCorrectObject;
    internal Vector3 defaultPosition;

    private void Awake() {
        defaultPosition = transform.localPosition;
    }

    //when object is being dragged position correctly
    void IDragHandler.OnDrag(PointerEventData eventData) {
       transform.position = eventData.position;
    }

    //when drag ended check if the correct object is in the basket
    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {

        float distance = Vector3.Distance(transform.position, GameManager.Instance.DragBucket.transform.position);

        if (distance < 50) {

            if (isCorrectObject) {
                transform.position = GameManager.Instance.DragBucket.transform.position;
                GameManager.Instance.IsCorrectDrag(true);
            } else {
                transform.localPosition = defaultPosition;
                GameManager.Instance.IsCorrectDrag(false);
            }
           
        } else {
            transform.localPosition = defaultPosition;
        }
    }
}

