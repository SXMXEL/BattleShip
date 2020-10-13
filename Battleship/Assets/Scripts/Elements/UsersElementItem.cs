using System;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Elements
{
    public class UsersElementItem : MonoBehaviour
    {
        public Coordinates Coordinates { private set; get; }
        [SerializeField] private Button _coordinatesButton;
        [SerializeField] private ElementSpriteChange _elementSpriteChange;
        [SerializeField] private RectTransform _userElementItemRectTransform;
        private GridElementType _gridElementTypeType;
        


        public GridElementType GridElementType
        {
            set
            {
                _gridElementTypeType = value;

                _elementSpriteChange.ChangeSprite(_gridElementTypeType);
            }
            get => _gridElementTypeType;
        }

        public void Init(
            Coordinates coordinates,
            Action<UsersElementItem> OnPressed,
            GridElementType gridElementType)
        {
            Coordinates = coordinates;
            _coordinatesButton.onClick.RemoveAllListeners();
            _coordinatesButton.onClick.AddListener(() => { OnPressed?.Invoke(this); });
            _coordinatesButton.onClick.AddListener(UserElementItemScale);
            GridElementType = gridElementType;
        }

        private void UserElementItemScale()
        {
            _userElementItemRectTransform.localScale = Vector3.zero;
            _userElementItemRectTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }
    }
}