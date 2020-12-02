using System;
using System.Linq;
using DG.Tweening;
using DragDrop;
using DragDropFunctions;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elements
{
    public enum ShipType
    {
        Submarine,
        Frigate,
        Schooner,
        Boat
    }


    [RequireComponent(typeof(RectTransform))]
    public class Ship : Draggable
    {
        public bool IsVertical;
        public bool IsSet;
        public Vector3[] ElementsPositions;
        public ShipType ShipType;
        public OwnerType OwnerType;
        public ElementItem[] ShipItems;
        public Vector3 DefaultPosition;
        public RectTransform ShipRectTransform;
        private ElementItem[,] _userGrid;
        private Sprite _currentShipSprite;
        private const float _maxDelay = 1.3f;
        private bool _isTouched;
        private float _touchTime;
        private int _length;
        private Action<Coordinates, Ship> _onShipDragOnGrid;


        public void Init(ElementItem[,] userGrid, Action<Coordinates, Ship> onShipDragOnGrid)
        {
            _onShipDragOnGrid = onShipDragOnGrid;
            _userGrid = userGrid;
            ShipRectTransform = GetComponent<RectTransform>();
            DefaultPosition = ShipRectTransform.position;
        }


        private ElementItem GetNearestElement(Vector3 targetPosition)
        {
            var elements = _userGrid.Cast<ElementItem>().ToList();
            var nearestPossibleDistance =
                Vector2.Distance(_userGrid[0, 0].transform.position, _userGrid[0, 1].transform.position);
            ElementItem nearestElementItem = null;
            float nearestDistance = float.MaxValue;
            foreach (ElementItem elementItem in _userGrid)
            {
                var currentDistance = Vector2.Distance(targetPosition, elementItem.transform.position);
                if (currentDistance < nearestDistance
                    && currentDistance > nearestPossibleDistance)
                {
                    nearestElementItem = elementItem;
                    nearestDistance = currentDistance;
                }
                else if (currentDistance < nearestPossibleDistance)
                {
                    ShipRectTransform.position = DefaultPosition;
                }
            }

            return nearestElementItem;
        }

        

        protected override void OnItemBeginDrag()
        {
            if (ShipItems != null &&
                ShipItems.All(data => data.GridElementType == GridElementType.Ship))
            {
                foreach (var shipItem in ShipItems)
                {
                    shipItem.GridElementType = GridElementType.None;
                }
            }
        }

        protected override void OnItemDrag()
        {
            Debug.Log("Item is dragging");
        }

        protected override void OnItemEndDrag(PointerEventData eventData)
        {
            _onShipDragOnGrid.Invoke(GetNearestElement(eventData.position).Coordinates, this);
        }

        protected override void OnPointerDownOnItem()
        {
            if (!_isTouched)
            {
                _isTouched = true;
                _touchTime = Time.time;
                StartCoroutine(WaitForDoubleTap());
            }
            else
            {
                _isTouched = false;
            }
        }
        
        private System.Collections.IEnumerator WaitForDoubleTap()
        {
            yield return new WaitUntil(() => !_isTouched || Time.time >= _touchTime + _maxDelay);
            if (Time.time >= _touchTime + _maxDelay)
            {
                yield break;
            }

            if (ShipRectTransform.position != DefaultPosition) yield break;
            if (ShipRectTransform.rotation == Quaternion.identity)
            {
                ShipRectTransform.DORotate(new Vector3(0, 0, 90), 1);
                IsVertical = true;
            }
            else
            {
                ShipRectTransform.DORotate(new Vector3(0, 0, 0), 1);
                IsVertical = false;
            }
        }
    }
}