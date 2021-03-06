﻿using System;
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Elements
{
    public enum OwnerType
    {
        User,
        Computer
    }


    public class ElementItem : MonoBehaviour, IDropHandler
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
                () =>
                {
                    onPressed?.Invoke(this);
                    ElementItemShake();
                });
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
            if (_ownerType == OwnerType.Computer)
            {
                _elementItemTransform.localScale = Vector3.zero;
                _elementItemTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
                _elementItemTransform.DOShakePosition(3f, 2.5f);
            }
        }
    }
}