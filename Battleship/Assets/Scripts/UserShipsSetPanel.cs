using System;
using System.Linq;
using Elements;
using UnityEngine;
using UnityEngine.UI;

public class UserShipsSetPanel : MonoBehaviour
{
    [SerializeField] private Button _shipsSetPanelQuitButton;
    [SerializeField] private UsersElementItem _shipsSetGridCell;
    [SerializeField] private RectTransform _userShipsSetGridContainer;
    private static GameController _gameController;
    private const int GridSize = GameController.GridSize;
    private readonly UsersElementItem[,] _usersShipsCoordinates = new UsersElementItem[GridSize, GridSize];

    public void Init(Action onShipsSetPanelQuit, ElementItem[,] _userGrid)
    {
        _shipsSetPanelQuitButton.onClick.RemoveAllListeners();
        _shipsSetPanelQuitButton.onClick.AddListener(() => ShipsSet(_userGrid));
        _shipsSetPanelQuitButton.onClick.AddListener(onShipsSetPanelQuit.Invoke);
        ShipSetGridCreate(_usersShipsCoordinates, SetShips, _userShipsSetGridContainer);
    }

    private void ShipsSet(ElementItem[,] _userGrid)
    {
        foreach (var item in _usersShipsCoordinates.Cast<UsersElementItem>()
            .Where(data => data.GridElementType == GridElementType.Ship))
        {
            _userGrid[item.Coordinates.X, item.Coordinates.Y].GridElementType = item.GridElementType;
        }
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
    
    private void ShipSetGridCreate(UsersElementItem[,] usersElementItems,
        Action<UsersElementItem> onElementPressed,
        RectTransform container)
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