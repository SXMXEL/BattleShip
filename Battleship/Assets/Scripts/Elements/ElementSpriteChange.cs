using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Elements
{
    public class ElementSpriteChange : MonoBehaviour
    {
        [SerializeField] private Image _gfx;
        [SerializeField] private ElementGfx[] _elementGfxes;

        public void ChangeSprite(GridElementType gridElementType)
        {
            _gfx.sprite = _elementGfxes.First(data => data.GridElementType == gridElementType).Gfx;
        }
    }

    [Serializable]
    public class ElementGfx
    {
        public GridElementType GridElementType => gridElementType;

        [FormerlySerializedAs("_gridElement")] [SerializeField]
        private GridElementType gridElementType;

        public Sprite Gfx => _gfx;
        [SerializeField] private Sprite _gfx;

        public ElementGfx(GridElementType gridElementType, Sprite gfx)
        {
            this.gridElementType = gridElementType;
            _gfx = gfx;
        }
    }
}