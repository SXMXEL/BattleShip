using UnityEngine;
using UnityEngine.EventSystems;

namespace DragAndDrop
{
    public class OnDrop : MonoBehaviour, IDropHandler
    {
        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            Debug.Log("OnDrop");
            if (eventData.pointerDrag != null)
            {
                eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
                    GetComponent<RectTransform>().anchoredPosition;
            }
        }
    }
}
