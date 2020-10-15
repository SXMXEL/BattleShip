using System;
using System.Collections.Generic;
using System.Linq;
using DragDropFunctions;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elements
{
    public class ShipFactory : Draggable
    {
        public Sprite[] ShipSprites;
        private Sprite _currentShipSprite;
        private Func<Vector2, ElementItem> _distanceCalculator;
        private ElementItem[,] _userGrid;
        private List<Vector3> _elementsPositionsList = new List<Vector3>();
        private readonly Vector3[] _elementsPositions = new Vector3[4];
        private Ship _ship;
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
            switch (_ship.ShipType)
            {
                case ShipType.Submarine:
                    _ship.ShipItems = new[] {nearestElementItem, _userGrid[x, y + 1], _userGrid[x, y + 2], _userGrid[x, y - 1]};
                    _currentShipSprite = ShipSprites[0];
                    break;
                case ShipType.Frigate:
                    _ship.ShipItems = new[] {nearestElementItem, _userGrid[x, y + 1], _userGrid[x, y - 1]};
                    _currentShipSprite = ShipSprites[1];
                    break;
                case ShipType.Schooner:
                    _ship.ShipItems = new[] {nearestElementItem, _userGrid[x, y + 1]};
                    _currentShipSprite = ShipSprites[2];
                    break;
                case ShipType.Boat:
                    _ship.ShipItems = new[] {nearestElementItem};
                    _currentShipSprite = ShipSprites[3];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            foreach (ElementItem shipItem in _ship.ShipItems)
            {
                shipItem.GridElementType = GridElementType.Ship;
                // _elementPositions.Add(shipItem.transform.position);
            }

            for (int i = 0; i < _ship.ShipItems.Length; i++)
            {
                _elementsPositions[i] = _ship.ShipItems[i].transform.position;
            }
            var centerOfVectors = CenterOfVectors(_elementsPositions);


            GetComponent<RectTransform>().position = centerOfVectors;
        }
        
        private Vector3 CenterOfVectors( Vector3[] vectors )
        {
            Vector3 sum = new Vector3(0,0,0);
            if( vectors == null || vectors.Length == 0 )
            {
                return sum;
            }
 
            foreach( Vector3 vec in vectors )
            {
                sum += vec;
            }
            return sum/vectors.Length;
        }
        
    }
}