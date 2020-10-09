using System;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Elements
{
    public class ElementSpriteChange : MonoBehaviour
    {
        [SerializeField] private Image _gfx;
        [SerializeField] private ElementGfx[] _elementGfxes;

        public void ChangeSprite(GridElementType gridElementType, ShipType shipType)
        {
            _gfx.sprite = _elementGfxes.First(data => data.GridElementType == gridElementType && data.ShipType == shipType).Gfx;
        }
    }

    [Serializable]
    public class ElementGfx
    {
        public GridElementType GridElementType => _gridElementType;

        [SerializeField]
        private GridElementType _gridElementType;

        public ShipType ShipType => _shipType;

        [SerializeField] private ShipType _shipType;
        public Sprite Gfx => _gfx;
        [SerializeField] private Sprite _gfx;

        public ElementGfx(GridElementType gridElementType, Sprite gfx, ShipType shipType)
        {
            _gridElementType = gridElementType;
            _gfx = gfx;
            _shipType = shipType;
        }
    }
}