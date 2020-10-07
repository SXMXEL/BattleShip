using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Elements
{
    public enum OwnerType
    {
        User,
        Computer,
    }

    public class ElementItem : MonoBehaviour
    {
        [SerializeField] private Button _innerButton;
        [SerializeField] private ElementSpriteChange _elementSpriteChange;
        [SerializeField] private Transform _elementItemTransform;
        public Coordinates Coordinates { private set; get; }
        private GridElementType _gridElementTypeType;
        private OwnerType _ownerType;

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

        public void ElementItemShake ()
        {
            _elementItemTransform.DOShakePosition(3f, 2.5f);
        }
    }
}