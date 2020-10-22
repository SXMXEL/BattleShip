using System;
using System.Linq;
using Elements;
using Pool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI
{
    public enum PageState
    {
        StartPage,
        SettingsPage,
        ShipSetPage,
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
        [SerializeField] private StartPage _startPage;
        [SerializeField] private UserShipsSetPanel _userShipsSetPanel;
        [SerializeField] private SettingsPage _settingsPage;
        [SerializeField] private GamePage _gamePage;
        [SerializeField] private SoundManager _soundManager;
        [SerializeField] private MessageItemsController _messageItemsController;
        private DataManager _dataManager;
        private SessionDataManager _sessionDataManager;
        [SerializeField] private ShipsManager _shipsManager;
        [SerializeField] private ElementItem _gridCell;
        [SerializeField] private RectTransform _userGridContainer;
        [SerializeField] private RectTransform _computerGridContainer;
        public const int GridSize = 10;
        private readonly ElementItem[,] _userGridsCells = new ElementItem[GridSize, GridSize];
        private readonly ElementItem[,] _computerGridsCells = new ElementItem[GridSize, GridSize];
        private void Start()
        {
            Init();
        }

        private void Init()
        {
            SetPageState(PageState.StartPage);
            _dataManager = new DataManager();
            _sessionDataManager = new SessionDataManager();
            _soundManager.Init(_dataManager);
            _startPage.Init(()=>SetPageState(PageState.ShipSetPage),
                ()=> SetPageState(PageState.SettingsPage));
            _settingsPage.Init(()=> SetPageState(PageState.StartPage), _dataManager);
            _gamePage.Init(Restart,()=> SetPageState(PageState.GamePage));
            _userShipsSetPanel.Init(ShipsSetPanelQuit,
                () => SetRandomShips(_userGridsCells), _userGridsCells);
            GridCreate(_userGridsCells, null, _userGridContainer, OwnerType.User);
            GridCreate(_computerGridsCells, ElementPressedForAttack, _computerGridContainer, OwnerType.Computer);
            _shipsManager.Init(_userGridsCells);
        }

        private void SetPageState(PageState pageState)
        {
            switch (pageState)
            {
                case PageState.StartPage:
                    _startPage.gameObject.SetActive(true);
                    _userShipsSetPanel.gameObject.SetActive(false);
                    _gamePage.gameObject.SetActive(false);
                    _settingsPage.gameObject.SetActive(false);
                    break;
                case PageState.SettingsPage:
                    _startPage.gameObject.SetActive(false);
                    _userShipsSetPanel.gameObject.SetActive(false);
                    _gamePage.gameObject.SetActive(false);
                    _settingsPage.gameObject.SetActive(true);
                    break;
                case PageState.GamePage:
                    _startPage.gameObject.SetActive(false);
                    _userShipsSetPanel.gameObject.SetActive(false);
                    _gamePage.gameObject.SetActive(true);
                    _settingsPage.gameObject.SetActive(false);
                    break;
                case PageState.ShipSetPage:
                    _startPage.gameObject.SetActive(false);
                    _userShipsSetPanel.gameObject.SetActive(true);
                    _gamePage.gameObject.SetActive(false);
                    _settingsPage.gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void ShipsSetPanelQuit()
        {
            _startPage.gameObject.SetActive(false);
            _userShipsSetPanel.gameObject.SetActive(false);
            _gamePage.gameObject.SetActive(true);
            _settingsPage.gameObject.SetActive(false);
        }

        private void ToStartMenu()
        {
            _startPage.gameObject.SetActive(false);
            _userShipsSetPanel.gameObject.SetActive(false);
            _gamePage.gameObject.SetActive(false);
            _settingsPage.gameObject.SetActive(false);
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
            SetPageState(PageState.StartPage);
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

        private void ElementPressedForAttack(ElementItem elementItem)
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

                _gamePage.UserScoreText.text = "Hit: " + _sessionDataManager.UserHitShipsCount;
                _gamePage.ComputerScoreText.text = "Hit: " + _sessionDataManager.ComputerHitShipsCount;
            }

            _gamePage.WinnerText.text =
                _computerGridsCells.Cast<ElementItem>().Any(element => element.GridElementType == GridElementType.Ship)
                    ? "You LOSE"
                    : "You WIN";

            _gamePage._winnerPanelObject.SetActive(true);
        }
        
        
    }
}