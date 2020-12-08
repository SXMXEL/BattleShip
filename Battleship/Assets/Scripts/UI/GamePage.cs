using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GamePage : MonoBehaviour
    {
        public TextMeshProUGUI ComputerScore, UserScore, WinnerText;
        public GameObject AttackBlock, WinnerPanel, HideGameObjects, ShipsContainer ;
        public Button Confirm, Random;
        [SerializeField] private Button _restartButton, _backToStartMenuButton;


        public void Init(Action OnRestartAction, Action OnSetStartPageState)
        {
            _restartButton.onClick.RemoveAllListeners();
            _restartButton.onClick.AddListener(OnRestartAction.Invoke);
            _backToStartMenuButton.onClick.RemoveAllListeners();
            _backToStartMenuButton.onClick.AddListener(OnSetStartPageState.Invoke);
            Confirm.onClick.RemoveAllListeners();
            Confirm.onClick.AddListener(ConfirmShipsPositions);
        }

        public void ConfirmShipsPositions()
        {
            ShipsContainer.SetActive(false);
            HideGameObjects.SetActive(true);
        }

        

    }
}