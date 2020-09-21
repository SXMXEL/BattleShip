using System;
using System.Linq;
using Elements;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private ElementItem _gridCell;
    [SerializeField] private UsersElementItem _shipsSetGridCell;
    [SerializeField] private RectTransform _userShipsSetGridContainer;
    [SerializeField] private RectTransform _userGridContainer;
    [SerializeField] private RectTransform _computerGridContainer;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _shipsSetPanelQuitButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _backToStartMenuButton;
    [SerializeField] private AudioSource _explosion;
    [SerializeField] private AudioSource _miss;
    [SerializeField] private Text _computerScoreText;
    [SerializeField] private Text _userScoreText;
    [SerializeField] private Text _winnerText;
    [SerializeField] private GameObject _startMenu;
    [SerializeField] private GameObject _shipsSetPanel;
    [SerializeField] private GameObject _game;
    [SerializeField] private GameObject _winnerPanel;
    private static int _gameGridSize = 10;
    private readonly ElementItem[,] _userGridsCells = new ElementItem[_gameGridSize, _gameGridSize];
    private readonly ElementItem[,] _computerGridsCells = new ElementItem[_gameGridSize, _gameGridSize];
    private readonly UsersElementItem[,] _usersShipsCoordinates = new UsersElementItem[_gameGridSize, _gameGridSize];
    private int _userScore;
    private int _computerScore;
    private const float _delayTime = 0.3f;


    private void Start()
    {
        _startButton.onClick.RemoveAllListeners();
        _startButton.onClick.AddListener(GameStart);
        _shipsSetPanelQuitButton.onClick.RemoveAllListeners();
        _shipsSetPanelQuitButton.onClick.AddListener(ShipsSetPanelQuit);
        _restartButton.onClick.RemoveAllListeners();
        _restartButton.onClick.AddListener(Restart);
        _backToStartMenuButton.onClick.RemoveAllListeners();
        _backToStartMenuButton.onClick.AddListener(BackToStartMenu);
        ShipSetGridCreate(_usersShipsCoordinates, SetShips, _userShipsSetGridContainer);
        GridCreate(_userGridsCells, null, _userGridContainer, OwnerType.User);
        ShipsSet();
        GridCreate(_computerGridsCells, OnElementPressedForAttack, _computerGridContainer, OwnerType.User);
    }

    private void GameStart()
    {
        _startMenu.SetActive(false);
        _shipsSetPanel.SetActive(true);
        _game.SetActive(false);
    }

    private void ShipsSetPanelQuit()
    {
        _startMenu.SetActive(false);
        _shipsSetPanel.SetActive(false);
        _game.SetActive(true);
    }

    private void BackToStartMenu()
    {
        _startMenu.SetActive(true);
        _shipsSetPanel.SetActive(false);
        _game.SetActive(false);
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SetShips(UsersElementItem usersElementItem)
    {
        /*while (_usersShipsCoordinates.Cast<UsersElementItem>()
            .Where(data => data.GridElementType == GridElementType.Ship)
            .ToList().Count < _gameGridSize * _gameGridSize * 0.2f)
        {*/
        usersElementItem.GridElementType = GridElementType.Ship;
        //}
    }


    private static void SetRandomShips(ElementItem[,] grid)
    {
        while (grid.Cast<ElementItem>().Where(data => data.GridElementType == GridElementType.Ship).ToList().Count
               < _gameGridSize * _gameGridSize * 0.2f)
        {
            var randRow = Random.Range(0, _gameGridSize);
            var randColumn = Random.Range(0, _gameGridSize);
            if (grid[randRow, randColumn].GridElementType == GridElementType.None)
            {
                grid[randRow, randColumn].GridElementType = GridElementType.Ship;
            }
        }
    }

    private void ShipSetGridCreate(UsersElementItem[,] usersElementItems,
        Action<UsersElementItem> onElementPressed,
        RectTransform container)
    {
        for (int i = 0; i < _gameGridSize; i++)
        {
            for (int j = 0; j < _gameGridSize; j++)
            {
                var usersElementItem = Instantiate(_shipsSetGridCell, container);
                usersElementItem.Init(new Coordinates(i, j), onElementPressed, GridElementType.None);
                usersElementItems[i, j] = usersElementItem;
            }
        }
    }

    private void GridCreate(ElementItem[,] elementItems,
        Action<ElementItem> onElementPressed,
        RectTransform container,
        OwnerType ownerType)
    {
        for (int i = 0; i < _gameGridSize; i++)
        {
            for (int j = 0; j < _gameGridSize; j++)
            {
                var elementItem = Instantiate(_gridCell, container);
                elementItem.Init(new Coordinates(i, j), onElementPressed,
                    GridElementType.None, ownerType);
                elementItems[i, j] = elementItem;
            }
        }

        if (elementItems == _computerGridsCells)
        {
            SetRandomShips(_computerGridsCells);
        }
        
    }

    private void ShipsSet()
    {
        foreach (var item in _usersShipsCoordinates.Cast<UsersElementItem>()
            .Where(data => data.GridElementType == GridElementType.Ship))
        {
            Debug.Log("found one");
            _userGridsCells[item.Coordinates.X, item.Coordinates.Y].GridElementType = item.GridElementType;
        }
    }


    private void ComputerAttack()
    {
        while (_computerGridsCells.Cast<ElementItem>().Any(data => data.GridElementType == GridElementType.Ship))
        {
            var randRow = Random.Range(0, _gameGridSize);
            var randColumn = Random.Range(0, _gameGridSize);
            var currentElementItem = _userGridsCells[randRow, randColumn];
            switch (currentElementItem.GridElementType)
            {
                case GridElementType.None:
                    currentElementItem.GridElementType = GridElementType.Miss;
                    _miss.PlayDelayed(_delayTime);
                    return;
                case GridElementType.Ship:
                    currentElementItem.GridElementType = GridElementType.DestroyedShip;
                    _explosion.PlayDelayed(_delayTime);
                    _computerScore++;
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
        _userScoreText.text = "Hit: " + _userScore;
        _computerScoreText.text = "Hit: " + _computerScore;
        while (_userGridsCells.Cast<ElementItem>().Any(data => data.GridElementType == GridElementType.Ship)
               && _computerGridsCells.Cast<ElementItem>().Any(data => data.GridElementType == GridElementType.Ship))
        {
            switch (elementItem.GridElementType)
            {
                case GridElementType.None:
                    elementItem.GridElementType = GridElementType.Miss;
                    _miss.PlayDelayed(_delayTime);
                    ComputerAttack();
                    break;
                case GridElementType.Ship:
                    elementItem.GridElementType = GridElementType.DestroyedShip;
                    _explosion.PlayDelayed(_delayTime);
                    _userScore++;
                    break;
                case GridElementType.DestroyedShip:
                    return;
                case GridElementType.Miss:
                    return;
            }
        }

        _winnerText.text =
            _computerGridsCells.Cast<ElementItem>().Any(element => element.GridElementType == GridElementType.Ship)
                ? "You lose"
                : "You win";

        _winnerPanel.SetActive(true);
    }
}