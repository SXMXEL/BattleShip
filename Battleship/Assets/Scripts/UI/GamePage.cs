using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Elements;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GamePage : MonoBehaviour
    {
        public GameObject WinnerPanel, HideGameObjects, ShipsContainer;
        public TextMeshProUGUI ComputerScore, UserScore, WinnerText;
        private ShipsManager _shipsManager;
        [SerializeField] private GameObject _attackBlock;
        [SerializeField] private Button _restartButton, _backToStartMenuButton, _confirm, _random;

        private Action<Ship[]> _dragBlock;
        private Action<Ship[]> _resetShips;


        public void Init(
            ElementItem[,] userGrid,
            ElementItem[,] computerGrid,
            int gridSize,
            Action onSetStartPageState,
            ShipsManager shipsManager,
            List<StepMessageItem> stepMessageItems,
            SessionDataManager sessionDataManager,
            Action<PageState> setGamePage,
            Action resetGrid)
        {
            FreshStart();
            _shipsManager = shipsManager;
            _dragBlock = _shipsManager.DragBlock;
            _resetShips = _shipsManager.ResetShips;
            _restartButton.onClick.RemoveAllListeners();
            _restartButton.onClick.AddListener(() =>
                Restart(
                    stepMessageItems,
                    sessionDataManager,
                    setGamePage,
                    resetGrid));
            _backToStartMenuButton.onClick.RemoveAllListeners();
            _backToStartMenuButton.onClick.AddListener(onSetStartPageState.Invoke);
            _confirm.onClick.RemoveAllListeners();
            _confirm.onClick.AddListener(() =>
            {
                ConfirmShipsPositions();
                _shipsManager.SetRandomShips(gridSize, _shipsManager.ComputerShips, computerGrid);
            });
            _random.onClick.RemoveAllListeners();
            _random.onClick.AddListener(() =>
            {
                _shipsManager.SetRandomShipsForAll();
                ShipsContainer.SetActive(false);
                HideGameObjects.SetActive(true);
            });
            TryToActivateConfirmButton();
        }

        public void TryToActivateConfirmButton()
        {
            if (_shipsManager.UserShips.All(ship => ship.IsSet))
            {
                _confirm.interactable = true;
                _confirm.onClick.AddListener(() => _dragBlock.Invoke(_shipsManager.UserShips));
            }
            else
            {
                _confirm.interactable = false;
            }
        }

        public void AttackBlocker(bool block)
        {
            _attackBlock.SetActive(block);
        }

        private void ConfirmShipsPositions()
        {
            ShipsContainer.SetActive(false);
            HideGameObjects.SetActive(true);
        }


        private void Restart(
            List<StepMessageItem> stepMessageItems,
            SessionDataManager sessionDataManager,
            Action<PageState> pageState,
            Action resetGrid)
        {
            _resetShips.Invoke(_shipsManager.UserShips);
            _resetShips.Invoke(_shipsManager.ComputerShips);
            resetGrid.Invoke();
            foreach (var item in stepMessageItems)
            {
                item.gameObject.SetActive(false);
                item.Dispose();
            }

            FreshStart();
            AttackBlocker(false);
            sessionDataManager.UserHitShipsCount = 0;
            sessionDataManager.ComputerHitShipsCount = 0;
            _confirm.interactable = false;
            pageState(PageState.GamePage);
        }


        private void FreshStart()
        {
            WinnerPanel.SetActive(false);
            HideGameObjects.SetActive(false);
            ShipsContainer.SetActive(true);
        }
    }
}