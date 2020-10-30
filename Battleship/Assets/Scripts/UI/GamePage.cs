using System;
using System.Linq;
using Elements;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GamePage : MonoBehaviour
    {
        public Text ComputerScoreText, UserScoreText, WinnerText;
        public GameObject _winnerPanelObject;
        [SerializeField] private Button _restartButton, _backToStartMenuButton, _confirmShipsPositionsButton;
        [SerializeField] private GameObject _computerGameObjects, _shipsContainer;


        public void Init(Action OnRestartAction, Action OnSetPageState)
        {
            _restartButton.onClick.RemoveAllListeners();
            _restartButton.onClick.AddListener(OnRestartAction.Invoke);
            _backToStartMenuButton.onClick.RemoveAllListeners();
            _backToStartMenuButton.onClick.AddListener(OnSetPageState.Invoke);
            _confirmShipsPositionsButton.onClick.RemoveAllListeners();
            _confirmShipsPositionsButton.onClick.AddListener(ConfirmShipsPositions);
        }

        private void ConfirmShipsPositions()
        {
            _shipsContainer.SetActive(false);
            _computerGameObjects.SetActive(true);
        }
    }
}