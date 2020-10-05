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


    public void LogGenerate(ElementItem[,] grid, OwnerType ownerType)
    {
        SetLogText(grid);
        GenerateItem(ownerType);
        _scrollView.verticalNormalizedPosition = 0;
    }

    private void GenerateItem(OwnerType ownerType)
    {
        GameObject scrollItemObj = Instantiate(_scrollItemPrefab);
        scrollItemObj.transform.SetParent(_scrollContent.transform, false);
        scrollItemObj.transform.Find("AttackLogText").gameObject.GetComponent<Text>().text = _logText;
        if (ownerType == OwnerType.User)
        {
            scrollItemObj.transform.Find("AttackLogText").gameObject.GetComponent<Text>().alignment =
                TextAnchor.MiddleRight;
        }
        else if(ownerType == OwnerType.Computer)
        {
            scrollItemObj.transform.Find("AttackLogText").gameObject.GetComponent<Text>().alignment =
                TextAnchor.MiddleLeft;
        }
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
                        _logText = "Destroyed ship at " + (grid[i, j].Coordinates.X + 1).ToString() + " " +
                                   (grid[i, j].Coordinates.Y + 1).ToString();
                        break;
                    case GridElementType.Miss:
                        _logText = "Missed attack at " + (grid[i, j].Coordinates.X + 1).ToString() + " " +
                                   (grid[i, j].Coordinates.Y + 1).ToString();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}