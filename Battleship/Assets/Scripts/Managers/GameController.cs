using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Elements;
using Pool;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public enum PageState
    {
        StartPage,
        SettingsPage,
        GamePage
    }

    public enum GridElementType
    {
        None,
        Ship,
        DestroyedShip,
        Miss
    }

    public class GameController : MonoBehaviour
    {
        private DataManager _dataManager;
        private SessionDataManager _sessionDataManager;
        [SerializeField] private SoundManager _soundManager;
        [SerializeField] private StartPage _startPage;
        [SerializeField] private SettingsPage _settingsPage;
        [SerializeField] private GamePage _gamePage;
        [SerializeField] private ShipsManager _shipsManager;
        [SerializeField] private MessageItemsController _messageItemsController;
        [SerializeField] private ElementItem _gridCell;
        [SerializeField] private RectTransform _userGridContainer, _computerGridContainer;
        private const int _gridSize = 10;
        private readonly ElementItem[,] _userGridsCells = new ElementItem[_gridSize, _gridSize];
        private readonly ElementItem[,] _computerGridsCells = new ElementItem[_gridSize, _gridSize];
        private List<Coordinates> _possibleShipCoordinates = new List<Coordinates>();


        private void Start()
        {
            Init();
        }

        private void Init()
        {
            SetPageState(PageState.StartPage);
            FreshStart();
            _dataManager = new DataManager();
            _sessionDataManager = new SessionDataManager();
            _soundManager.Init(_dataManager);
            GridCreate(_userGridsCells, null, _userGridContainer, OwnerType.User);
            GridCreate(_computerGridsCells, ElementPressedForAttack, _computerGridContainer, OwnerType.Computer);
            _startPage.Init(() => SetPageState(PageState.GamePage),
                () => SetPageState(PageState.SettingsPage));
            _gamePage.Init(Restart, () => SetPageState(PageState.StartPage));
            _settingsPage.Init(() => SetPageState(PageState.StartPage), _dataManager);
            _shipsManager.Init(_userGridsCells, _computerGridsCells, _gamePage, _gridSize, _soundManager);
        }

        private void SetPageState(PageState pageState)
        {
            _startPage.gameObject.SetActive(pageState == PageState.StartPage);
            _settingsPage.gameObject.SetActive(pageState == PageState.SettingsPage);
            _gamePage.gameObject.SetActive(pageState == PageState.GamePage);
        }


        private void Restart()
        {
            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    _userGridsCells[i, j].GridElementType = GridElementType.None;
                    _computerGridsCells[i, j].GridElementType = GridElementType.None;
                }
            }

            foreach (var item in _messageItemsController.StepMessageItems)
            {
                item.gameObject.SetActive(false);
                item.Dispose();
            }
            
            FreshStart();
            AttackBlocker(false);
            ResetShips(_shipsManager.UserShips);
            ResetShips(_shipsManager.ComputerShips);
            _sessionDataManager.UserHitShipsCount = 0;
            _sessionDataManager.ComputerHitShipsCount = 0;
            _gamePage.Confirm.interactable = false;
            SetPageState(PageState.GamePage);
        }

        private static void ResetShips(Ship[] ships)
        {
            foreach (var ship in ships)
            {
                ship.IsVertical = false;
                ship.CantDrag = false;
                ship.IsSet = false;
                if (ship.OwnerType == OwnerType.User)
                {
                    ship.ShipRectTransform.position = ship.DefaultPosition;
                }
                else
                {
                    ship.gameObject.SetActive(false);
                }

                ship.ShipRectTransform.DORotate(new Vector3(0, 0, 0), 1);
            }
        }

        private void FreshStart()
        {
            _gamePage.WinnerPanel.SetActive(false);
            _gamePage.HideGameObjects.SetActive(false);
            _gamePage.ShipsContainer.SetActive(true);
        }


        private void GridCreate(ElementItem[,] elementItems,
            Action<ElementItem> onElementPressed,
            RectTransform container,
            OwnerType ownerType)
        {
            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
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

        private void AttackBlocker(bool block)
        {
            _gamePage.AttackBlock.SetActive(block);
        }

        private IEnumerator ComputerAttack()
        {
            AttackBlocker(true);
            var gridCoordinates = (from ElementItem item in _userGridsCells
                select item.Coordinates).ToList();
            while (_computerGridsCells.Cast<ElementItem>().Any(item
                => item.GridElementType == GridElementType.Ship))
            {
                var randRow = Random.Range(0, _gridSize);
                var randColumn = Random.Range(0, _gridSize);
                var currentElementItem = _userGridsCells[randRow, randColumn];
                if (!_possibleShipCoordinates.Any())
                {
                    switch (currentElementItem.GridElementType)
                    {
                        case GridElementType.None:
                            currentElementItem.GridElementType = GridElementType.Miss;
                            _soundManager.PlaySfx(SfxType.Miss);
                            _messageItemsController.LogGenerate(
                                _userGridsCells[randRow, randColumn],
                                OwnerType.Computer);
                            AttackBlocker(false);
                            yield break;
                        case GridElementType.Ship:
                            currentElementItem.GridElementType = GridElementType.DestroyedShip;
                            _soundManager.PlaySfx(SfxType.Explosion);
                            _messageItemsController.LogGenerate(
                                _userGridsCells[randRow, randColumn],
                                OwnerType.Computer);
                            _sessionDataManager.ComputerHitShipsCount++;
                            _possibleShipCoordinates = PossibleShipCoordinates(currentElementItem.Coordinates,
                                gridCoordinates);
                            while (_possibleShipCoordinates.Any())
                            {
                                yield return new WaitForSeconds(1);
                                int index = Random.Range(0, _possibleShipCoordinates.Count);
                                var x = _possibleShipCoordinates[index].X;
                                var y = _possibleShipCoordinates[index].Y;
                                var possibleItem = _userGridsCells[x, y];
                                switch (possibleItem.GridElementType)
                                {
                                    case GridElementType.None:
                                        possibleItem.GridElementType = GridElementType.Miss;
                                        _soundManager.PlaySfx(SfxType.Miss);
                                        _messageItemsController.LogGenerate(
                                            _userGridsCells[x, y],
                                            OwnerType.Computer);
                                        _possibleShipCoordinates.Remove(_possibleShipCoordinates[index]);
                                        AttackBlocker(false);
                                        yield break;
                                    case GridElementType.Ship:
                                        possibleItem.GridElementType = GridElementType.DestroyedShip;
                                        _soundManager.PlaySfx(SfxType.Explosion);
                                        _messageItemsController.LogGenerate(
                                            _userGridsCells[x, y],
                                            OwnerType.Computer);
                                        _sessionDataManager.ComputerHitShipsCount++;
                                        _possibleShipCoordinates = PossibleShipCoordinates(possibleItem.Coordinates,
                                            gridCoordinates);
                                        continue;
                                    case GridElementType.DestroyedShip:
                                    case GridElementType.Miss:
                                        _possibleShipCoordinates.Remove(_possibleShipCoordinates[index]);
                                        continue;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }

                            yield break;
                        case GridElementType.DestroyedShip:
                        case GridElementType.Miss:
                            continue;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    while (_possibleShipCoordinates.Any())
                    {
                        yield return new WaitForSeconds(1);
                        var index = Random.Range(0, _possibleShipCoordinates.Count);
                        var x = _possibleShipCoordinates[index].X;
                        var y = _possibleShipCoordinates[index].Y;
                        var possibleItem = _userGridsCells[x, y];
                        switch (possibleItem.GridElementType)
                        {
                            case GridElementType.None:
                                possibleItem.GridElementType = GridElementType.Miss;
                                _soundManager.PlaySfx(SfxType.Miss);
                                _messageItemsController.LogGenerate(
                                    _userGridsCells[x, y],
                                    OwnerType.Computer);
                                _possibleShipCoordinates.Remove(_possibleShipCoordinates[index]);
                                AttackBlocker(false);
                                yield break;
                            case GridElementType.Ship:
                                possibleItem.GridElementType = GridElementType.DestroyedShip;
                                _soundManager.PlaySfx(SfxType.Explosion);
                                _messageItemsController.LogGenerate(
                                    _userGridsCells[x, y],
                                    OwnerType.Computer);
                                _sessionDataManager.ComputerHitShipsCount++;
                                _possibleShipCoordinates = PossibleShipCoordinates(possibleItem.Coordinates,
                                    gridCoordinates);
                                continue;
                            case GridElementType.DestroyedShip:
                            case GridElementType.Miss:
                                _possibleShipCoordinates.Remove(_possibleShipCoordinates[index]);
                                continue;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }

        private static List<Coordinates> PossibleShipCoordinates(Coordinates targetCoordinates,
            List<Coordinates> gridCoordinates)
        {
            var x = targetCoordinates.X;
            var y = targetCoordinates.Y;
            var possibleCoordinates = new List<Coordinates>();
            for (int i = -1; i < 2 && i != 0; i++)
            {
                possibleCoordinates.Add(new Coordinates(x, y + i));
                possibleCoordinates.Add(new Coordinates(x + i, y));
            }

            return possibleCoordinates.Where(shipCoordinates
                => gridCoordinates.Any(data
                    => data.X == shipCoordinates.X && data.Y == shipCoordinates.Y)).ToList();
        }

        

        private void ElementPressedForAttack(ElementItem elementItem)
        {
            while (_userGridsCells.Cast<ElementItem>().Any(item
                       => item.GridElementType == GridElementType.Ship)
                   && _computerGridsCells.Cast<ElementItem>().Any(item
                       => item.GridElementType == GridElementType.Ship))
            {
                switch (elementItem.GridElementType)
                {
                    case GridElementType.None:
                        elementItem.GridElementType = GridElementType.Miss;
                        _soundManager.PlaySfx(SfxType.Miss);
                        _messageItemsController.LogGenerate(
                            _computerGridsCells[elementItem.Coordinates.X, elementItem.Coordinates.Y], OwnerType.User);
                        StartCoroutine(ComputerAttack());
                        break;
                    case GridElementType.Ship:
                        elementItem.GridElementType = GridElementType.DestroyedShip;
                        _soundManager.PlaySfx(SfxType.Explosion);
                        _messageItemsController.LogGenerate(
                            _computerGridsCells[elementItem.Coordinates.X, elementItem.Coordinates.Y], OwnerType.User);
                        _sessionDataManager.UserHitShipsCount++;
                        break;
                    case GridElementType.DestroyedShip:
                    case GridElementType.Miss:
                        return;
                }

                TryActivateShip(_shipsManager.ComputerShips);

                _gamePage.UserScore.text = "Hit: " + _sessionDataManager.UserHitShipsCount;
                _gamePage.ComputerScore.text = "Hit: " + _sessionDataManager.ComputerHitShipsCount;
            }

            _gamePage.WinnerText.text =
                _computerGridsCells.Cast<ElementItem>().Any(item
                    => item.GridElementType == GridElementType.Ship)
                    ? "You LOSE"
                    : "You WIN";

            _gamePage.WinnerPanel.SetActive(true);
        }

        private static void TryActivateShip(Ship[] ships)
        {
            foreach (var ship in ships)
            {
                ship.gameObject.SetActive(ship.ShipItems.All(item =>
                    item.GridElementType == GridElementType.DestroyedShip));
            }
        }
    }
}