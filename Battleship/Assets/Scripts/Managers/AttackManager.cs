using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using DG.Tweening;
using Elements;
using Pool;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class AttackManager : MonoBehaviour
    {
        private SoundManager _soundManager;
        private MessageItemsController _messageItemsController;
        private ShipsManager _shipsManager;
        private SessionDataManager _sessionDataManager;
        private GamePage _gamePage;
        private GridManager _gridManager;
        private List<Coordinates> _possibleShipCoordinates = new List<Coordinates>();

        private List<Coordinates> _hitedCoordinates = new List<Coordinates>(); //TODO 

        //make computer attack only horizontal or vertical if 2 last ship parts are hited in one direction
        private List<Coordinates> _nonHitCoordinates = new List<Coordinates>();
        private List<Ship> _hitShips;
        private ElementItem[,] _userGridCells;
        private ElementItem[,] _computerGridCells;
        private int _gridSize;


        public void Init(
            SoundManager soundManager,
            MessageItemsController messageItemsController,
            ShipsManager shipsManager,
            SessionDataManager sessionDataManager,
            GridManager gridManager,
            GamePage gamePage)
        {
            _soundManager = soundManager;
            _messageItemsController = messageItemsController;
            _shipsManager = shipsManager;
            _sessionDataManager = sessionDataManager;
            _gridManager = gridManager;
            _gamePage = gamePage;
            _userGridCells = _gridManager.UserGrid;
            _computerGridCells = _gridManager.ComputerGrid;
            _gridSize = _gridManager.GridSize;
        }

        public void ElementPressedForAttack(ElementItem elementItem)
        {
            while (_userGridCells.Cast<ElementItem>().Any(item
                       => item.GridElementType == GridElementType.Ship)
                   && _computerGridCells.Cast<ElementItem>().Any(item
                       => item.GridElementType == GridElementType.Ship))
            {
                switch (elementItem.GridElementType)
                {
                    case GridElementType.None:
                        elementItem.GridElementType = GridElementType.Miss;
                        _soundManager.PlaySfx(SfxType.Miss);
                        _messageItemsController.LogGenerate(
                            _computerGridCells[elementItem.Coordinates.X, elementItem.Coordinates.Y], OwnerType.User);
                        StartCoroutine(ComputerAttack());
                        break;
                    case GridElementType.Ship:
                        elementItem.GridElementType = GridElementType.DestroyedShip;
                        _soundManager.PlaySfx(SfxType.Explosion);
                        _messageItemsController.LogGenerate(
                            _computerGridCells[elementItem.Coordinates.X, elementItem.Coordinates.Y], OwnerType.User);
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
                _computerGridCells.Cast<ElementItem>().Any(item
                    => item.GridElementType == GridElementType.Ship)
                    ? "You LOSE"
                    : "You WIN";

            _gamePage.WinnerPanel.SetActive(true);
        }


        private IEnumerator ComputerAttack()
        {
            _gamePage.AttackBlocker(true);
            var gridCoordinates = (from ElementItem item in _userGridCells
                select item.Coordinates).ToList();


            while (_computerGridCells.Cast<ElementItem>().Any(item
                => item.GridElementType == GridElementType.Ship))
            {
                if (!_possibleShipCoordinates.Any())
                {
                    _nonHitCoordinates = NonHitCoordinatesUpdate();
                    int randRow;
                    int randColumn;
                    while (true)
                    {
                        randRow = Random.Range(0, _gridSize);
                        randColumn = Random.Range(0, _gridSize);
                        var coordinates = new Coordinates(randRow, randColumn);
                        if (_nonHitCoordinates.Any(data
                            => data.X == coordinates.X && data.Y == coordinates.Y))
                        {
                            continue;
                        }

                        break;
                    }

                    var currentElementItem = PossibleLongShipItem() == null
                        ? _userGridCells[randRow, randColumn]
                        : PossibleLongShipItem();

                    var currentX = currentElementItem.Coordinates.X;
                    var currentY = currentElementItem.Coordinates.Y;
                    switch (currentElementItem.GridElementType)
                    {
                        case GridElementType.None:
                            currentElementItem.GridElementType = GridElementType.Miss;
                            OnAttack(currentElementItem, SfxType.Miss, currentX, currentY);
                            _gamePage.AttackBlocker(false);
                            yield break;
                        case GridElementType.Ship:
                            currentElementItem.GridElementType = GridElementType.DestroyedShip;
                            OnAttack(currentElementItem, SfxType.Explosion, currentX, currentY);
                            _sessionDataManager.ComputerHitShipsCount++;
                            _nonHitCoordinates = NonHitCoordinatesUpdate();
                            _possibleShipCoordinates = PossibleShipCoordinates(currentElementItem.Coordinates,
                                    gridCoordinates)
                                .Where(data =>
                                    !_nonHitCoordinates.Any(item =>
                                        data.X == item.X && data.Y == item.Y)).ToList();
                            if (!_possibleShipCoordinates.Any())
                            {
                                Debug.Log("Empty");
                            }

                            _hitedCoordinates.Add(currentElementItem.Coordinates);
                            while (_possibleShipCoordinates.Any())
                            {
                                yield return new WaitForSeconds(1);
                                var index = Random.Range(0, _possibleShipCoordinates.Count);
                                var x = _possibleShipCoordinates[index].X;
                                var y = _possibleShipCoordinates[index].Y;
                                var possibleItem = _userGridCells[x, y];
                                switch (possibleItem.GridElementType)
                                {
                                    case GridElementType.None:
                                        possibleItem.GridElementType = GridElementType.Miss;
                                        OnAttack(possibleItem, SfxType.Miss, x, y);
                                        _possibleShipCoordinates.Remove(_possibleShipCoordinates[index]);
                                        _gamePage.AttackBlocker(false);
                                        yield break;
                                    case GridElementType.Ship:
                                        possibleItem.GridElementType = GridElementType.DestroyedShip;
                                        OnAttack(possibleItem, SfxType.Explosion, x, y);
                                        _sessionDataManager.ComputerHitShipsCount++;
                                        _nonHitCoordinates = NonHitCoordinatesUpdate();
                                        _possibleShipCoordinates = PossibleShipCoordinates(possibleItem.Coordinates,
                                                gridCoordinates)
                                            .Where(data =>
                                                !_nonHitCoordinates.Any(item =>
                                                    data.X == item.X && data.Y == item.Y)).ToList();
                                        if (!_possibleShipCoordinates.Any())
                                        {
                                            Debug.Log("Empty");
                                        }

                                        _hitedCoordinates.Add(possibleItem.Coordinates);
                                        break;
                                    case GridElementType.DestroyedShip:
                                    case GridElementType.Miss:
                                        _possibleShipCoordinates.Remove(_possibleShipCoordinates[index]);
                                        break;
                                    default:
                                        yield break;
                                }
                            }

                            break;
                        case GridElementType.DestroyedShip:
                        case GridElementType.Miss:
                            break;
                        default:
                            yield break;
                    }
                }

                while (_possibleShipCoordinates.Any())
                {
                    yield return new WaitForSeconds(1);
                    var index = Random.Range(0, _possibleShipCoordinates.Count);
                    var x = _possibleShipCoordinates[index].X;
                    var y = _possibleShipCoordinates[index].Y;
                    var possibleItem = _userGridCells[x, y];
                    switch (possibleItem.GridElementType)
                    {
                        case GridElementType.None:
                            possibleItem.GridElementType = GridElementType.Miss;
                            OnAttack(possibleItem, SfxType.Miss, x, y);
                            _possibleShipCoordinates.Remove(_possibleShipCoordinates[index]);
                            _gamePage.AttackBlocker(false);
                            yield break;
                        case GridElementType.Ship:
                            possibleItem.GridElementType = GridElementType.DestroyedShip;
                            OnAttack(possibleItem, SfxType.Explosion, x, y);
                            _sessionDataManager.ComputerHitShipsCount++;
                            _nonHitCoordinates = NonHitCoordinatesUpdate();
                            _possibleShipCoordinates = PossibleShipCoordinates(possibleItem.Coordinates,
                                    gridCoordinates)
                                .Where(data =>
                                    !_nonHitCoordinates.Any(item =>
                                        data.X == item.X && data.Y == item.Y)).ToList();
                            _hitedCoordinates.Add(possibleItem.Coordinates);
                            break;
                        case GridElementType.DestroyedShip:
                        case GridElementType.Miss:
                            _possibleShipCoordinates.Remove(_possibleShipCoordinates[index]);
                            break;
                        default:
                            yield break;
                    }
                }
            }
        }
        
        private ElementItem PossibleLongShipItem()
        {
            if (_shipsManager.UserShips.FirstOrDefault(ship
                => ship.ShipItems.Count(item => item.GridElementType == GridElementType.DestroyedShip) >= 2
                   && ship.ShipItems.Any(item => item.GridElementType == GridElementType.Ship)) == null) return null;
            {
                var longShip = _shipsManager.UserShips.FirstOrDefault(ship
                    => ship.ShipItems.Count(item => item.GridElementType == GridElementType.DestroyedShip) >= 2
                       && ship.ShipItems.Any(item => item.GridElementType == GridElementType.Ship));
                if (!(longShip is null))
                    return longShip.ShipItems.FirstOrDefault((item =>
                        item.GridElementType != GridElementType.DestroyedShip));
                else
                {
                    return null;
                }
            }
        }

        private List<Ship> HitShipsCheck()
        {
            return _shipsManager.UserShips.Where(ship =>
                ship.ShipItems.All(item => item.GridElementType == GridElementType.DestroyedShip)).ToList();
        }

        private List<Coordinates> NonHitCoordinatesUpdate()
        {
            _hitShips = HitShipsCheck();
            var nonHitCoordinates = new List<Coordinates>();

            foreach (var ship in _hitShips)
            {
                foreach (var item in ship.AccessZone)
                {
                    nonHitCoordinates.Add(item);
                }
            }

            return nonHitCoordinates;
        }

        private void OnAttack(ElementItem AttackedItem, SfxType sfxType, int x, int y)
        {
            _soundManager.PlaySfx(sfxType);
            _messageItemsController.LogGenerate(
                _userGridCells[x, y],
                OwnerType.Computer);
            var itemTransform = AttackedItem.gameObject.transform;
            itemTransform.localScale = Vector3.zero;
            itemTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            itemTransform.DOShakePosition(3f, 2.5f);
        }

        private static List<Coordinates> PossibleShipCoordinates(Coordinates targetCoordinates,
            List<Coordinates> gridCoordinates)
        {
            var x = targetCoordinates.X;
            var y = targetCoordinates.Y;
            var possibleCoordinates = new List<Coordinates>
            {
                new Coordinates(x, y + 1),
                new Coordinates(x, y - 1),
                new Coordinates(x + 1, y),
                new Coordinates(x - 1, y)
            };

            return possibleCoordinates.Where(shipCoordinates
                => gridCoordinates.Any(data
                    => data.X == shipCoordinates.X && data.Y == shipCoordinates.Y)).ToList();
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