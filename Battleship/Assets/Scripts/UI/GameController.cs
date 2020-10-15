using System;
using System.Collections.Generic;
using System.Linq;
using Elements;
using Pool;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI
{
    public enum GridElementType
    {
        None,
        Ship,
        DestroyedShip,
        Miss
    }

    public class GameController : MonoBehaviour
    {
        [SerializeField] private UserShipsSetPanel _userShipsSetPanel;
        [SerializeField] private SettingsMenu _settingsMenu;
        [SerializeField] private SoundManager _soundManager;
        [SerializeField] private MessageItemsController _messageItemsController;
        private DataManager _dataManager;
        private SessionDataManager _sessionDataManager;
        [SerializeField] private ShipFactory _shipFactory;
        [SerializeField] private ElementItem _gridCell;
        [SerializeField] private RectTransform _userGridContainer;
        [SerializeField] private RectTransform _computerGridContainer;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _backToStartMenuButton;
        [SerializeField] private Button _settingsMenuButton;
        [SerializeField] private Text _computerScoreText, _userScoreText, _winnerText;
        [SerializeField] private GameObject _startMenuObject, _settingsMenuObject, _shipsSetPanelObject, _gamePhaseObject, _winnerPanelObject;
        public const int GridSize = 10;
        private readonly ElementItem[,] _userGridsCells = new ElementItem[GridSize, GridSize];
        private readonly ElementItem[,] _computerGridsCells = new ElementItem[GridSize, GridSize];
        private void Start()
        {
            Init();
        }

        private void Init()
        {
            ToStartMenu();
            _dataManager = new DataManager();
            _sessionDataManager = new SessionDataManager();
            _soundManager.Init(_dataManager);
            _settingsMenu.Init(ToStartMenu, _dataManager);
            _userShipsSetPanel.Init(ShipsSetPanelQuit, () => SetRandomShips(_userGridsCells), _userGridsCells);
            _startButton.onClick.RemoveAllListeners();
            _startButton.onClick.AddListener(GameStart);
            _settingsMenuButton.onClick.RemoveAllListeners();
            _settingsMenuButton.onClick.AddListener(ToSettingsMenu);
            _restartButton.onClick.RemoveAllListeners();
            _restartButton.onClick.AddListener(Restart);
            _backToStartMenuButton.onClick.RemoveAllListeners();
            _backToStartMenuButton.onClick.AddListener(ToStartMenu);
            GridCreate(_userGridsCells, null, _userGridContainer, OwnerType.User);
            GridCreate(_computerGridsCells, OnElementPressedForAttack, _computerGridContainer, OwnerType.Computer);
            
            
            
            _shipFactory.Init(GetNearestElement, _userGridsCells);
        }

        private void GameStart()
        {
            _startMenuObject.SetActive(false);
            _shipsSetPanelObject.SetActive(true);
            _gamePhaseObject.SetActive(false);
            _settingsMenuObject.SetActive(false);
        }

        private void ToSettingsMenu()
        {
            _startMenuObject.SetActive(false);
            _shipsSetPanelObject.SetActive(false);
            _gamePhaseObject.SetActive(false);
            _settingsMenuObject.SetActive(true);
        }

        private void ShipsSetPanelQuit()
        {
            _startMenuObject.SetActive(false);
            _shipsSetPanelObject.SetActive(false);
            _gamePhaseObject.SetActive(true);
            _settingsMenuObject.SetActive(false);
        }

        private void ToStartMenu()
        {
            _startMenuObject.SetActive(true);
            _shipsSetPanelObject.SetActive(false);
            _gamePhaseObject.SetActive(false);
            _settingsMenuObject.SetActive(false);
        }

        private void Restart()
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    _userGridsCells[i, j].GridElementType = GridElementType.None;
                    _computerGridsCells[i, j].GridElementType = GridElementType.None;
                    _userShipsSetPanel._usersShipsCoordinates[i, j].GridElementType = GridElementType.None;
                }
            }

            _sessionDataManager.UserHitShipsCount = 0;
            _sessionDataManager.ComputerHitShipsCount = 0;
            SetRandomShips(_computerGridsCells);
            GameStart();
        }
        

        private void SetRandomShips(ElementItem[,] grid)
        {
            while (grid.Cast<ElementItem>().Where(data => data.GridElementType == GridElementType.Ship).ToList().Count
                   < GridSize * GridSize * 0.2f)
            {
                var randRow = Random.Range(0, GridSize);
                var randColumn = Random.Range(0, GridSize);
                if (grid[randRow, randColumn].GridElementType == GridElementType.None)
                {
                    grid[randRow, randColumn].GridElementType = GridElementType.Ship;
                }
            }
        }


        private void GridCreate(ElementItem[,] elementItems,
            Action<ElementItem> onElementPressed,
            RectTransform container,
            OwnerType ownerType)
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
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


            if (elementItems == _computerGridsCells)
            {
                SetRandomShips(_computerGridsCells);
            }
        }


        private void ComputerAttack()
        {
            while (_computerGridsCells.Cast<ElementItem>().Any(data => data.GridElementType == GridElementType.Ship))
            {
                var randRow = Random.Range(0, GridSize);
                var randColumn = Random.Range(0, GridSize);
                var currentElementItem = _userGridsCells[randRow, randColumn];
                switch (currentElementItem.GridElementType)
                {
                    case GridElementType.None:
                        currentElementItem.GridElementType = GridElementType.Miss;
                        _soundManager.PlaySfx(SfxType.Miss);
                        _messageItemsController.LogGenerate(
                            _userGridsCells[currentElementItem.Coordinates.X, currentElementItem.Coordinates.Y],
                            OwnerType.Computer);
                        return;
                    case GridElementType.Ship:
                        currentElementItem.GridElementType = GridElementType.DestroyedShip;
                        _soundManager.PlaySfx(SfxType.Explosion);
                        _messageItemsController.LogGenerate(
                            _userGridsCells[currentElementItem.Coordinates.X, currentElementItem.Coordinates.Y],
                            OwnerType.Computer);
                        _sessionDataManager.ComputerHitShipsCount++;
                        break;
                    case GridElementType.DestroyedShip:
                        break;
                    case GridElementType.Miss:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void OnElementPressedForAttack(ElementItem elementItem)
        {
            while (_userGridsCells.Cast<ElementItem>().Any(data => data.GridElementType == GridElementType.Ship)
                   && _computerGridsCells.Cast<ElementItem>().Any(data => data.GridElementType == GridElementType.Ship))
            {
                switch (elementItem.GridElementType)
                {
                    case GridElementType.None:
                        elementItem.GridElementType = GridElementType.Miss;
                        _soundManager.PlaySfx(SfxType.Miss);
                        _messageItemsController.LogGenerate(
                            _computerGridsCells[elementItem.Coordinates.X, elementItem.Coordinates.Y], OwnerType.User);
                        ComputerAttack();
                        break;
                    case GridElementType.Ship:
                        elementItem.GridElementType = GridElementType.DestroyedShip;
                        _soundManager.PlaySfx(SfxType.Explosion);
                        _messageItemsController.LogGenerate(
                            _computerGridsCells[elementItem.Coordinates.X, elementItem.Coordinates.Y], OwnerType.User);
                        _sessionDataManager.UserHitShipsCount++;
                        break;
                    case GridElementType.DestroyedShip:
                        return;
                    case GridElementType.Miss:
                        return;
                }

                _userScoreText.text = "Hit: " + _sessionDataManager.UserHitShipsCount;
                _computerScoreText.text = "Hit: " + _sessionDataManager.ComputerHitShipsCount;
            }

            _winnerText.text =
                _computerGridsCells.Cast<ElementItem>().Any(element => element.GridElementType == GridElementType.Ship)
                    ? "You LOSE"
                    : "You WIN";

            _winnerPanelObject.SetActive(true);
        }

        private ElementItem GetNearestElement(Vector2 targetPosition)
        {
            var elements = _userGridsCells.Cast<ElementItem>().ToList();
            var nearestPossibleDistance =
                Vector2.Distance(elements[0].transform.position, elements[1].transform.position);
            ElementItem nearestElementItem = null;
            float nearestDistance = float.MaxValue;
            foreach (ElementItem elementItem in _userGridsCells)
            {
                var currentDistance = Vector2.Distance(targetPosition, elementItem.transform.position);
                if (currentDistance < nearestDistance
                    && currentDistance > nearestPossibleDistance)
                {
                    nearestElementItem = elementItem;
                    nearestDistance = currentDistance;
                }
            }
            return nearestElementItem;
        }

    }
}