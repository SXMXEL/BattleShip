using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GamePage : MonoBehaviour
    {
        public Text ComputerScoreText, UserScoreText, WinnerText;
        public GameObject WinnerPanelObject;
        [SerializeField] private Button _restartButton, _backToStartMenuButton, _confirmShipsPositionsButton;
        [SerializeField] private GameObject _hideGameObjects, _shipsContainer;


        public void Init(Action OnRestartAction, Action OnSetStartPageState, Action Confirm)
        {
            _restartButton.onClick.RemoveAllListeners();
            _restartButton.onClick.AddListener(OnRestartAction.Invoke);
            _backToStartMenuButton.onClick.RemoveAllListeners();
            _backToStartMenuButton.onClick.AddListener(OnSetStartPageState.Invoke);
            _confirmShipsPositionsButton.onClick.RemoveAllListeners();
            _confirmShipsPositionsButton.onClick.AddListener(ConfirmShipsPositions);
            _confirmShipsPositionsButton.onClick.AddListener(Confirm.Invoke);
        }

        private void ConfirmShipsPositions()
        {
            _shipsContainer.SetActive(false);
            _hideGameObjects.SetActive(true);
        }
    }
}