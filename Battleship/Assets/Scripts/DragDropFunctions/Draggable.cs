using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elements;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DragDropFunctions
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public abstract class Draggable : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler,
        IDragHandler
    {
        [SerializeField] private Canvas _canvas;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private ShipType _shipType;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("OnEndDrag");
            _canvasGroup.alpha = .6f;
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("OnDrag");
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
            OnItemDrag(eventData);
        }

        protected abstract void OnItemDrag(PointerEventData eventData);

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("OnEndDrag");
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            OnItemEndDrag(eventData);
        }

        protected abstract void OnItemEndDrag(PointerEventData eventData);

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("OnPointerDown");
        }
    }
}