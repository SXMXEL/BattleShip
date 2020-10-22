using System.Linq;
using DG.Tweening;
using DragDropFunctions;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elements
{
    public class Ship : Draggable
    {
        public Vector3[] _elementsPositions;
        public ShipType ShipType;
        public ElementItem[] ShipItems;
        private ElementItem[,] _userGrid;
        private Sprite _currentShipSprite;
        private readonly float _maxDelay = 1.3f;
        private bool _isTouched;
        private bool _isVertical;
        private float touchTime;

        public void Init(ElementItem[,] userGrid)
        {
            _userGrid = userGrid;
        }


        private ElementItem GetNearestElement(Vector3 targetPosition)
        {
            var elements = _userGrid.Cast<ElementItem>().ToList();
            var nearestPossibleDistance =
                Vector2.Distance(elements[0].transform.position, elements[1].transform.position);
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
            }

            return nearestElementItem;
        }


        protected override void OnItemDrag(PointerEventData eventData)
        {
            Debug.Log("Item is dragging");
        }

        protected override void OnItemEndDrag(PointerEventData eventData)
        {
            var nearestElementItem = GetNearestElement(eventData.position);
            if (nearestElementItem == null)
            {
                Debug.Log("Nothing found");
                return;
            }

            Debug.Log("Nearest item is " + nearestElementItem.Coordinates);

            var x = nearestElementItem.Coordinates.X;
            var y = nearestElementItem.Coordinates.Y;
            if (_isVertical == false)
            {
                switch (ShipType)
                {
                    case ShipType.Submarine:
                        ShipItems = new[]
                            {nearestElementItem, _userGrid[x, y + 1], _userGrid[x, y + 2], _userGrid[x, y - 1]};
                        break;
                    case ShipType.Frigate:
                        ShipItems = new[] {nearestElementItem, _userGrid[x, y + 1], _userGrid[x, y - 1]};
                        break;
                    case ShipType.Schooner:
                        ShipItems = new[] {nearestElementItem, _userGrid[x, y + 1]};
                        break;
                    case ShipType.Boat:
                        ShipItems = new[] {nearestElementItem};
                        break;
                    default:
                        Debug.Log("Wrong Type");
                        break;
                }
            }
            else if (_isVertical)
            {
                switch (ShipType)
                {
                    case ShipType.Submarine:
                        ShipItems = new[]
                            {nearestElementItem, _userGrid[x + 1, y], _userGrid[x + 2, y], _userGrid[x - 1, y]};
                        break;
                    case ShipType.Frigate:
                        ShipItems = new[] {nearestElementItem, _userGrid[x + 1, y], _userGrid[x - 1, y]};
                        break;
                    case ShipType.Schooner:
                        ShipItems = new[] {nearestElementItem, _userGrid[x + 1, y]};
                        break;
                    case ShipType.Boat:
                        ShipItems = new[] {nearestElementItem};
                        break;
                    default:
                        Debug.Log("Wrong Type");
                        break;
                }
            }

            foreach (var shipItem in ShipItems)
            {
                shipItem.GridElementType = GridElementType.Ship;
            }

            for (int i = 0; i < ShipItems.Length; i++)
            {
                _elementsPositions[i] = ShipItems[i].transform.position;
            }

            var centerOfVectors = CenterOfVectors(_elementsPositions);
            GetComponent<RectTransform>().position = centerOfVectors;
        }

        protected override void OnPointerDownOnItem(PointerEventData eventData)
        {
            if (!_isTouched)
            {
                _isTouched = true;
                touchTime = Time.time;
                StartCoroutine(WaitForDoubleTap());
            }
            else
            {
                _isTouched = false;
            }
        }

        private Vector3 CenterOfVectors(Vector3[] vectors)
        {
            Vector3 sum = new Vector3(0, 0, 0);
            if (vectors == null || vectors.Length == 0)
            {
                return sum;
            }

            foreach (Vector3 vec in vectors)
            {
                sum += vec;
            }

            return sum / vectors.Length;
        }

        System.Collections.IEnumerator WaitForDoubleTap()
        {
            yield return new WaitUntil(() => !_isTouched || Time.time >= touchTime + _maxDelay);
            if (Time.time >= touchTime + _maxDelay)
            {
                yield break;
            }

            if (GetComponent<RectTransform>().rotation == Quaternion.identity)
            {
                GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 90), 1, RotateMode.Fast);
                _isVertical = true;
            }
            else
            {
                GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 0), 1, RotateMode.Fast);
                _isVertical = false;
            }
        }
    }
}