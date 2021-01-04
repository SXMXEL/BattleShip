using UnityEngine;
using UnityEngine.EventSystems;

namespace DragAndDrop
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public abstract class Draggable : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler,
        IDragHandler
    {
        public bool CantDrag;
        [SerializeField] private Canvas _canvas;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!CantDrag)
            {
                Debug.Log("OnEndDrag");
                _canvasGroup.alpha = .6f;
                _canvasGroup.blocksRaycasts = false;
                OnItemBeginDrag();
            }
            else
            {
                eventData.pointerDrag = null;
            }
        }

        protected abstract void OnItemBeginDrag();

        public void OnDrag(PointerEventData eventData)
        {
            if (!CantDrag)
            {
                _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
                OnItemDrag();
            }
            else
            {
                eventData.pointerDrag = null;
            }
        }

        protected abstract void OnItemDrag();

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!CantDrag)
            {
                Debug.Log("OnEndDrag");
                _canvasGroup.alpha = 1;
                _canvasGroup.blocksRaycasts = true;
                OnItemEndDrag(eventData);
            }
            else
            {
                eventData.pointerDrag = null;
            }
        }

        protected abstract void OnItemEndDrag(PointerEventData eventData);

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!CantDrag)
            {
                OnPointerDownOnItem();
            }
            else
            {
                eventData.pointerDrag = null;
            }
        }

        protected abstract void OnPointerDownOnItem();
    }
}