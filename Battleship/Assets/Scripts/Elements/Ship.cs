using System;
using DragDropFunctions;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elements
{
    public class Ship : Draggable
    {
        public ShipType ShipType;
        public ElementItem[] ShipItems = null;
        public Sprite ShipSprite;
        private Func<Vector2, ElementItem> _distanceCalculator;
        private ElementItem[,] _userGrid;

        public void Init(Func<Vector2, ElementItem> distanceCalculator, ElementItem[,] userGrid)
        {
            _userGrid = userGrid;
            _distanceCalculator = distanceCalculator;
        }

        protected override void OnItemDrag(PointerEventData eventData)
        {
            Debug.Log("Ship is dragging");
        }

        protected override void OnItemEndDrag(PointerEventData eventData)
        {
            var nearestElementItem = _distanceCalculator.Invoke(eventData.position);
            if (nearestElementItem == null)
            {
                Debug.LogError("Nothing found");
                return;
            }

            Debug.Log("Nearest item is " + nearestElementItem.Coordinates);

            var x = nearestElementItem.Coordinates.X;
            var y = nearestElementItem.Coordinates.Y;
            ShipItems = new[] {nearestElementItem, _userGrid[x, y + 1], _userGrid[x, y + 2], _userGrid[x, y - 1]};
            foreach (ElementItem shipItem in ShipItems)
            {
                shipItem.GridElementType = GridElementType.Ship;
            }
        }
    }
}