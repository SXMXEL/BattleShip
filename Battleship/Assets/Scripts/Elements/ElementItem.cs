using System;
using DG.Tweening;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elements
{
    public enum OwnerType
    {
        User,
        Computer,
    }

    public enum ShipType
    {
        Submarine,
        Frigate,
        Schooner,
        Boat,
    }

    public class ElementItem : MonoBehaviour, IDropHandler
    {
        [SerializeField] private Button _innerButton;
        [SerializeField] private ElementSpriteChange _elementSpriteChange;
        [SerializeField] private Transform _elementItemTransform;
        public Coordinates Coordinates { private set; get; }
        private GridElementType _gridElementTypeType;
        private OwnerType _ownerType;

        public ShipType ShipType
        {
            set => _shipType = value;
            get => _shipType;
        }

        private ShipType _shipType;


        public GridElementType GridElementType
        {
            set
            {
                _gridElementTypeType = value;
                var currentGridElementType = _gridElementTypeType;
                if (_ownerType == OwnerType.Computer)
                {
                    switch (currentGridElementType)
                    {
                        case GridElementType.None:
                            break;
                        case GridElementType.Ship:
                            currentGridElementType = GridElementType.None;
                            break;
                        case GridElementType.DestroyedShip:
                            break;
                        case GridElementType.Miss:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                _elementSpriteChange.ChangeSprite(currentGridElementType);
            }
            get => _gridElementTypeType;
        }

        public void Init(
            Coordinates coordinates,
            Action<ElementItem> onPressed,
            GridElementType gridElementType,
            OwnerType ownerType)
        {
            Coordinates = coordinates;
            _innerButton.onClick.RemoveAllListeners();
            _innerButton.onClick.AddListener(
                () => { onPressed?.Invoke(this); });
            _innerButton.onClick.AddListener(ElementItemShake);
            GridElementType = gridElementType;
            _ownerType = ownerType;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
                    GetComponent<RectTransform>().anchoredPosition;
            }
        }

        private void ElementItemShake()
        {
            _elementItemTransform.DOShakePosition(3f, 2.5f);
        }
    }
}