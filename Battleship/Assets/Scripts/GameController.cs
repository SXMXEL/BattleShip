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
    [SerializeField] private UserShipsSetPanel _userShipsSetPanel;
    [SerializeField] private SoundManager _soundManager;
    [SerializeField] private UserData _userData;
    [SerializeField] private ElementItem _gridCell;
    [SerializeField] private RectTransform _userGridContainer;
    [SerializeField] private RectTransform _computerGridContainer;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _backToStartMenuButton;
    [SerializeField] private Text _computerScoreText;
    [SerializeField] private Text _userScoreText;
    [SerializeField] private Text _winnerText;
    [SerializeField] private GameObject _startMenu;
    [SerializeField] private GameObject _shipsSetPanel;
    [SerializeField] private GameObject _game;
    [SerializeField] private GameObject _winnerPanel;
    private SfxType _sfxType;
    public const int GridSize = 10;
    private readonly ElementItem[,] _userGridsCells = new ElementItem[GridSize, GridSize];
    private readonly ElementItem[,] _computerGridsCells = new ElementItem[GridSize, GridSize];
    private int _userScore;
    private int _computerScore;


    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _startButton.onClick.RemoveAllListeners();
        _startButton.onClick.AddListener(GameStart);
        _restartButton.onClick.RemoveAllListeners();
        _restartButton.onClick.AddListener(Restart);
        _backToStartMenuButton.onClick.RemoveAllListeners();
        _backToStartMenuButton.onClick.AddListener(BackToStartMenu);
        _userShipsSetPanel.Init(ShipsSetPanelQuit, _userGridsCells);
        GridCreate(_userGridsCells, null, _userGridContainer, OwnerType.User);
        GridCreate(_computerGridsCells, OnElementPressedForAttack, _computerGridContainer, OwnerType.Computer);
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
                    GridElementType.None, ownerType);
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
                    _soundManager.PlaySfx(_sfxType);
                    return;
                case GridElementType.Ship:
                    currentElementItem.GridElementType = GridElementType.DestroyedShip;
                    _soundManager.PlaySfx(_sfxType);
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
        while (_userGridsCells.Cast<ElementItem>().Any(data => data.GridElementType == GridElementType.Ship)
               && _computerGridsCells.Cast<ElementItem>().Any(data => data.GridElementType == GridElementType.Ship))
        {
            switch (elementItem.GridElementType)
            {
                case GridElementType.None:
                    elementItem.GridElementType = GridElementType.Miss;
                    _soundManager.PlaySfx(_sfxType);
                    ComputerAttack();
                    break;
                case GridElementType.Ship:
                    elementItem.GridElementType = GridElementType.DestroyedShip;
                    _soundManager.PlaySfx(_sfxType);
                    _userScore++;
                    break;
                case GridElementType.DestroyedShip:
                    return;
                case GridElementType.Miss:
                    return;
            }

            _userScoreText.text = "Hit: " + _userScore;
            _computerScoreText.text = "Hit: " + _computerScore;
        }

        _winnerText.text =
            _computerGridsCells.Cast<ElementItem>().Any(element => element.GridElementType == GridElementType.Ship)
                ? "You LOSE"
                : "You WIN";

        _winnerPanel.SetActive(true);
    }
}