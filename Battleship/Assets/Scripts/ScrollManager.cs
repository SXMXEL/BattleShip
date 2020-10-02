using System;
using Elements;
using UnityEngine;
using UnityEngine.UI;

public class ScrollManager : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollView;
    [SerializeField] private GameObject _scrollContent;
    [SerializeField] private GameObject _scrollItemPrefab;
    private string _logText;

    
    

    public void LogGenerate(ElementItem[,] grid)
    {
        GenerateItem();
        SetLogText(grid);
        _scrollView.verticalNormalizedPosition = 1;
    }

    private void GenerateItem()
    {
        GameObject scrollItemObj = Instantiate(_scrollItemPrefab);
        scrollItemObj.transform.SetParent(_scrollContent.transform, false);
        scrollItemObj.transform.Find("ComputerAttackLogText").gameObject.GetComponent<Text>().text = _logText;
    }

    private void SetLogText(ElementItem[,] grid)
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                switch (grid[i, j].GridElementType)
                {
                    case GridElementType.None:
                        break;
                    case GridElementType.Ship:
                        break;
                    case GridElementType.DestroyedShip:
                        _logText = "Destroyed ship at " + grid[i, j].Coordinates.X.ToString() + " " +
                                   grid[i, j].Coordinates.Y.ToString() + "\n";
                        break;
                    case GridElementType.Miss:
                        _logText = "Missed attack at " + grid[i, j].Coordinates.X.ToString() + " " +
                                   grid[i, j].Coordinates.Y.ToString() + "\n";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}