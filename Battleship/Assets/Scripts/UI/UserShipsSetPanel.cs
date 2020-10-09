﻿using System;
using System.Linq;
using Elements;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UserShipsSetPanel : MonoBehaviour
    {
        [SerializeField] private SoundManager _soundManager;
        [SerializeField] private Button _shipsSetPanelQuitButton;
        [SerializeField] private Button _randomShipSetButton;
        [SerializeField] private UsersElementItem _shipsSetGridCell;
        [SerializeField] private RectTransform _userShipsSetGridContainer;
        private static GameController _gameController;
        private const int GridSize = GameController.GridSize;
        public readonly UsersElementItem[,] _usersShipsCoordinates = new UsersElementItem[GridSize, GridSize];


        public void Init(Action onShipsSetPanelQuit, Action onSetRandomShipsAction, ElementItem[,] _userGrid)
        {
            _shipsSetPanelQuitButton.interactable = false;
            _shipsSetPanelQuitButton.onClick.RemoveAllListeners();
            _shipsSetPanelQuitButton.onClick.AddListener(() =>
            {
                ShipsCoordinatesSwap(_userGrid);
                onShipsSetPanelQuit.Invoke();
            });
            _randomShipSetButton.onClick.RemoveAllListeners();
            _randomShipSetButton.onClick.AddListener(() => 
            {
                onSetRandomShipsAction.Invoke();
                onShipsSetPanelQuit.Invoke();
            });
            ShipSetGridCreate(_usersShipsCoordinates, SetShips, _userShipsSetGridContainer);
        }

        private void ShipsCoordinatesSwap(ElementItem[,] _userGrid)
        {
            foreach (var item in _usersShipsCoordinates.Cast<UsersElementItem>()
                .Where(data => data.GridElementType == GridElementType.Ship))
            {
                _userGrid[item.Coordinates.X, item.Coordinates.Y].GridElementType = item.GridElementType;
            }
        }

        private void SetShips(UsersElementItem usersElementItem)
        {
            if (_usersShipsCoordinates.Cast<UsersElementItem>()
                    .Where(data => data.GridElementType == GridElementType.Ship)
                    .ToList().Count
                < GridSize * GridSize * 0.2f)
            {
                usersElementItem.GridElementType = GridElementType.Ship;
                _soundManager.PlaySfx(SfxType.ShipPlaceSound);
            }

            if (_usersShipsCoordinates.Cast<UsersElementItem>().Where(data => data.GridElementType == GridElementType.Ship)
                .ToList().Count > 19)
            {
                _shipsSetPanelQuitButton.interactable = true;
            }
        }


        private void ShipSetGridCreate(UsersElementItem[,] usersElementItems,
            Action<UsersElementItem> onElementPressed,
            RectTransform container
        )
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    var usersElementItem = Instantiate(_shipsSetGridCell, container);
                    usersElementItem.Init(
                        new Coordinates(i, j),
                        onElementPressed,
                        GridElementType.None);
                    usersElementItems[i, j] = usersElementItem;
                }
            }
        }
    }
}