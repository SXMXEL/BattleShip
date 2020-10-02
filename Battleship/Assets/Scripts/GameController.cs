using System;
using System.Linq;
using Elements;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


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
    [SerializeField] private ScrollManager _scrollManager;
    private DataManager _dataManager;
    private SessionDataManager _sessionDataManager;
    [SerializeField] private ElementItem _gridCell;
    [SerializeField] private RectTransform _userGridContainer;
    [SerializeField] private RectTransform _computerGridContainer;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _backToStartMenuButton;
    [SerializeField] private Button _settingsMenuButton;
    [SerializeField] private Text _computerScoreText;
    [SerializeField] private Text _userScoreText;
    [SerializeField] private Text _winnerText;
    [SerializeField] private GameObject _startMenuObject;
    [SerializeField] private GameObject _settingsMenuObject;
    [SerializeField] private GameObject _shipsSetPanelObject;
    [SerializeField] private GameObject _gamePhaseObject;
    [SerializeField] private GameObject _winnerPanelObject;
    public const int GridSize = 10;
    private readonly ElementItem[,] _userGridsCells = new ElementItem[GridSize, GridSize];
    private readonly ElementItem[,] _computerGridsCells = new ElementItem[GridSize, GridSize];


    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _dataManager = new DataManager();
        _sessionDataManager = new SessionDataManager();
        _soundManager.Init(_dataManager);
        _settingsMenu.Init(BackToStartMenu, _dataManager);
        _userShipsSetPanel.Init(ShipsSetPanelQuit, _userGridsCells);
        _startButton.onClick.RemoveAllListeners();
        _startButton.onClick.AddListener(GameStart);
        _settingsMenuButton.onClick.RemoveAllListeners();
        _settingsMenuButton.onClick.AddListener(ToSettingsMenu);
        _restartButton.onClick.RemoveAllListeners();
        _restartButton.onClick.AddListener(Restart);
        _backToStartMenuButton.onClick.RemoveAllListeners();
        _backToStartMenuButton.onClick.AddListener(BackToStartMenu);
        GridCreate(_userGridsCells, null, _userGridContainer, OwnerType.User);
        GridCreate(_computerGridsCells, OnElementPressedForAttack, _computerGridContainer, OwnerType.Computer);
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

    private void BackToStartMenu()
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


    private static void SetRandomShips(ElementItem[,] grid)
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
                    _scrollManager.LogGenerate(_userGridsCells);
                    return;
                case GridElementType.Ship:
                    currentElementItem.GridElementType = GridElementType.DestroyedShip;
                    _soundManager.PlaySfx(SfxType.Explosion);
                    _scrollManager.LogGenerate(_userGridsCells);
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
                    _scrollManager.LogGenerate(_computerGridsCells);
                    ComputerAttack();
                    break;
                case GridElementType.Ship:
                    elementItem.GridElementType = GridElementType.DestroyedShip;
                    _soundManager.PlaySfx(SfxType.Explosion);
                    _scrollManager.LogGenerate(_computerGridsCells);
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
}