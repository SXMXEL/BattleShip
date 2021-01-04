using System;
using Elements;
using UnityEngine;

namespace Managers
{
    public class GridManager : MonoBehaviour
    {
        public ElementItem[,] UserGrid => _userGridsCells;
        public ElementItem[,] ComputerGrid => _computerGridsCells;
        public int GridSize => _gridSize;
        [SerializeField] private RectTransform _userGridContainer, _computerGridContainer;
        [SerializeField] private ElementItem _gridCell;
        private const int _gridSize = 10;
        private readonly ElementItem[,] _userGridsCells = new ElementItem[_gridSize, _gridSize];
        private readonly ElementItem[,] _computerGridsCells = new ElementItem[_gridSize, _gridSize];


        public void Init(Action<ElementItem> elementPressedForAttack)
        {
            GridCreate(_userGridsCells, null, _userGridContainer, OwnerType.User);
            GridCreate(_computerGridsCells, elementPressedForAttack, _computerGridContainer, OwnerType.Computer);
        }

        public void ResetGrid()
        {
            for (var i = 0; i < _gridSize; i++)
            {
                for (var j = 0; j < _gridSize; j++)
                {
                    _userGridsCells[i, j].GridElementType = GridElementType.None;
                    _computerGridsCells[i, j].GridElementType = GridElementType.None;
                }
            }
        }

        private void GridCreate(ElementItem[,] elementItems,
            Action<ElementItem> onElementPressed,
            RectTransform container,
            OwnerType ownerType)
        {
            for (var i = 0; i < _gridSize; i++)
            {
                for (var j = 0; j < _gridSize; j++)
                {
                    var elementItem = Instantiate(_gridCell, container);
                    elementItem.Init(
                        new Coordinates(i, j),
                        onElementPressed,
                        GridElementType.None,
                        ownerType);
                    elementItems[i, j] = elementItem;
                }
            }
        }
    }
}