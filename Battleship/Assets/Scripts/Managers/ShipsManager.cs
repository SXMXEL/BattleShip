using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Elements;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Managers
{
    public class ShipsManager : MonoBehaviour
    {
        public Ship[] UserShips;
        public Ship[] ComputerShips;
        private ElementItem[,] _userGrid;
        private GamePage _gamePage;
        private SoundManager _soundManager;
        


        public void Init(ElementItem[,] userGrid, ElementItem[,] computerGrid, GamePage gamePage,
            int gridSize, SoundManager soundManager)
        {
            _userGrid = userGrid;
            _gamePage = gamePage;
            _soundManager = soundManager;
            foreach (var ship in UserShips)
            {
                ship.Init(userGrid, ShipDragOnGrid);
            }

            _gamePage.Random.onClick.RemoveAllListeners();
            _gamePage.Random.onClick.AddListener(() =>
            {
                SetRandomShips(gridSize, ComputerShips, computerGrid);
                SetRandomShips(gridSize, UserShips, _userGrid);
                _gamePage.ConfirmShipsPositions();
            });
            _gamePage.Confirm.onClick.AddListener(
                () => SetRandomShips(gridSize, ComputerShips, computerGrid));
            TryToActivateConfirmButton();
        }

        private void TryToActivateConfirmButton()
        {
            if (UserShips.All(ship => ship.IsSet))
            {
                _gamePage.Confirm.interactable = true;
                _gamePage.Confirm.onClick.AddListener(() => DragBlock(UserShips));
            }
            else
            {
                _gamePage.Confirm.interactable = false;
            }
        }

        private static void DragBlock(Ship[] ships)
        {
            foreach (Ship ship in ships)
            {
                ship.CantDrag = true;
            }
        }

        private void SetRandomShips(int gridSize, Ship[] ships, ElementItem[,] grid)
        {
            foreach (var elementItem in grid)
            {
                elementItem.GridElementType = GridElementType.None;
            }

            var gridCoordinatesList =
                (from ElementItem elementItem in grid select elementItem.Coordinates).ToList();
            for (int i = 0; i < 3; i++)
            {
                while (true)
                {
                    var randomShip = ships[Random.Range(0, ships.Length)];
                    if (randomShip.IsVertical != true && randomShip.ElementsPositions.Length > 1)
                    {
                        randomShip.IsVertical = true;
                        randomShip.ShipRectTransform.DORotate(new Vector3(0, 0, 90), 1);
                    }
                    else
                    {
                        continue;
                    }

                    break;
                }
            }

            foreach (var ship in ships)
            {
                while (true)
                {
                    var randomRow = Random.Range(0, gridSize);
                    var randomColumn = Random.Range(0, gridSize);
                    var randomCoordinate = new Coordinates(randomRow, randomColumn);
                    var coordinates = GetShipCoordinates(ship.ShipType, randomCoordinate, ship.IsVertical);
                    var shipCoordinatesList
                        = coordinates.Item1;
                    var shipItemList = new List<ElementItem>();
                    var validShipCoordinatesList = shipCoordinatesList.Where(shipCoordinate
                        => gridCoordinatesList.Any(data
                            => data.X == shipCoordinate.X && data.Y == shipCoordinate.Y)).ToList();

                    if (validShipCoordinatesList.Count == ship.ElementsPositions.Length)
                    {
                        for (int i = 0; i < ship.ElementsPositions.Length; i++)
                        {
                            shipItemList.Add(grid[validShipCoordinatesList[i].X, validShipCoordinatesList[i].Y]);
                        }

                        ship.ShipItems = shipItemList.ToArray();

                        if (ship.ShipItems.All(data => data.GridElementType == GridElementType.None))
                        {
                            foreach (var shipItem in ship.ShipItems)
                            {
                                shipItem.GridElementType = GridElementType.Ship;
                            }

                            ship.ElementsPositions =
                                (from ElementItem elementItem in ship.ShipItems select elementItem.transform.position)
                                .ToArray();
                            Debug.Log(ship.ElementsPositions.FirstOrDefault().ToString());
                            var centerOfVectors = CenterOfVectors(ship.ElementsPositions);
                            ship.ShipRectTransform.position = centerOfVectors;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    break;
                }
            }

            DragBlock(ships);
        }


        private Tuple<List<Coordinates>, List<Coordinates>> GetShipCoordinates(ShipType shipType, Coordinates targetCoordinates,
            bool isVertical)
        {
            int length;
            var coordinates = new List<Coordinates>();
            var accessZone = new List<Coordinates>();
            switch (shipType)
            {
                case ShipType.Submarine:
                    length = 4;
                    break;
                case ShipType.Frigate:
                    length = 3;
                    break;
                case ShipType.Schooner:
                    length = 2;
                    break;
                case ShipType.Boat:
                    length = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shipType), shipType, null);
            }

            if (length >= 2)
            {
                if (!isVertical)
                {
                    targetCoordinates = new Coordinates(targetCoordinates.X, targetCoordinates.Y - 1);
                    for (var i = 0; i < length; i++)
                    {
                        coordinates.Add(new Coordinates(targetCoordinates.X, targetCoordinates.Y + i));
                        accessZone.Add(new Coordinates(targetCoordinates.X - 1, targetCoordinates.Y + i));
                        accessZone.Add(new Coordinates(targetCoordinates.X + 1, targetCoordinates.Y + i));
                        
                    }

                    for (int i = -1; i < 2; i++)
                    {
                        accessZone.Add(new Coordinates(targetCoordinates.X + i, targetCoordinates.Y - 1));
                        accessZone.Add(new Coordinates(targetCoordinates.X + i, targetCoordinates.Y + length));
                    }
                }
                else
                {
                    targetCoordinates = new Coordinates(targetCoordinates.X - 1, targetCoordinates.Y);
                    for (var i = 0; i < length; i++)
                    {
                        coordinates.Add(new Coordinates(targetCoordinates.X + i, targetCoordinates.Y));
                        
                        accessZone.Add(new Coordinates(targetCoordinates.X + i, targetCoordinates.Y - 1));
                        accessZone.Add(new Coordinates(targetCoordinates.X + i, targetCoordinates.Y + 1));
                    }
                    
                    for (int i = -1; i < 2; i++)
                    {
                        accessZone.Add(new Coordinates(targetCoordinates.X - 1, targetCoordinates.Y + i));
                        accessZone.Add(new Coordinates(targetCoordinates.X + length, targetCoordinates.Y + i));
                    }
                }
            }
            else
            {
                coordinates.Add(targetCoordinates);
            }

            return new Tuple<List<Coordinates>, List<Coordinates>>(coordinates, accessZone);
        }

        private Tuple<List<Coordinates>, List<Coordinates>> GetShipItemList(ElementItem[,] grid, ShipType shipType,
            Coordinates mainShipItemCoordinates, bool isVertical)
        {
            var coordinates = GetShipCoordinates(shipType, mainShipItemCoordinates, isVertical);
            var gridCoordinates = 
                (from ElementItem elementItem in grid select elementItem.Coordinates).ToList();
            var shipCoordinates = coordinates.Item1;
            var validShipCoordinates = shipCoordinates.Where(shipCoordinate
                => gridCoordinates.Any(data
                    => data.X == shipCoordinate.X && data.Y == shipCoordinate.Y)).ToList();
            return new Tuple<List<Coordinates>, List<Coordinates>>(validShipCoordinates, coordinates.Item2);
        }

        private void ShipDragOnGrid(Coordinates nearestItemCoordinates, Ship ship)
        {
            if (ship.OwnerType != OwnerType.User) return;
            var coordinates = 
                GetShipItemList(_userGrid, ship.ShipType, nearestItemCoordinates, ship.IsVertical);
            var gridCoordinates = 
                (from ElementItem elementItem in _userGrid select elementItem.Coordinates).ToList();
            var validShipCoordinates =
                coordinates.Item1;
            var shipItems = new List<ElementItem>();
            if (validShipCoordinates.Count == ship.ElementsPositions.Length)
            {

                var accessCoordinates = coordinates.Item2.Where(shipCoordinate
                    => gridCoordinates.Any(data
                        => data.X == shipCoordinate.X && data.Y == shipCoordinate.Y)).ToList();
                var accessItems = accessCoordinates.Select(t => _userGrid[t.X, t.Y]).ToArray();

                for (var i = 0; i < ship.ElementsPositions.Length; i++)
                {
                    shipItems.Add(_userGrid[validShipCoordinates[i].X,
                        validShipCoordinates[i].Y]);
                }

                Debug.Log(shipItems.First());
                ship.ShipItems = shipItems.ToArray();
                if (ship.ShipItems.All(data => data.GridElementType == GridElementType.None)
                && accessItems.All(item => item.GridElementType == GridElementType.None))
                {
                    ship.ElementsPositions =
                        (from ElementItem elementItem in ship.ShipItems select elementItem.transform.position)
                        .ToArray();
                    var centerOfVectors = CenterOfVectors(ship.ElementsPositions);

                    foreach (var shipItem in ship.ShipItems)
                    {
                        shipItem.GridElementType = GridElementType.Ship;
                    }

                    ship.ShipRectTransform.position = centerOfVectors;
                    ship.IsSet = true;
                    _soundManager.PlaySfx(SfxType.ShipPlaceSound);
                    TryToActivateConfirmButton();
                }
                else
                {
                    Debug.Log("Can't set ship");
                    ship.ShipItems = null;
                    ship.IsSet = false;
                    ship.ShipRectTransform.position = ship.DefaultPosition;
                }

                Debug.Log("Nearest item is " + nearestItemCoordinates);
            }
            else
            {
                ship.IsSet = false;
                ship.ShipRectTransform.position = ship.DefaultPosition;
            }
        }

        private Vector3 CenterOfVectors(Vector3[] vectors)
        {
            var sum = new Vector3(0, 0, 0);
            if (vectors == null || vectors.Length == 0)
            {
                return sum;
            }

            foreach (var vec in vectors)
            {
                sum += vec;
            }

            return sum / vectors.Length;
        }
    }
}