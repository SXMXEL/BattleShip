using Elements;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DragDropFunctions
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
