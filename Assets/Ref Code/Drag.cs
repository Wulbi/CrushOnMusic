using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.UI;
using UnityEngine.EventSystems;


public class Drag : MonoBehaviour
{
    public Canvas canvas;

     Vector2 startPos;

    private void Start()
    {
        startPos = gameObject.transform.position;
    }

    public void Drop()
    {
        gameObject.transform.position = startPos;
    }

    public void DragHandler(BaseEventData data)
    {
        PointerEventData pointer = (PointerEventData)data;

        Vector2 position;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, pointer.position, canvas.worldCamera, out position);
        transform.position = canvas.transform.TransformPoint(position);

    }


}


//This source code is originally bought from www.anysourcecode.com
// Visit www.anysourcecode.com
//
//Contact us at:
//
//Email : hello@anysourcecode.com
//Facebook: https://www.facebook.com/anysourcecode
//Twitter: https://x.com/anysourcecode
//Instagram: https://www.instagram.com/anysourcecode
//Youtube: http://www.youtube.com/@anysourcecode
//LinkedIn: www.linkedin.com/anysourcecode
//Pinterest: https://www.pinterest.com/anysourcecode/
