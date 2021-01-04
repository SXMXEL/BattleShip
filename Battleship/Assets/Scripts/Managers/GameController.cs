using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using DG.Tweening;
using Elements;
using Pool;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class GameController : MonoBehaviour
    {
        private PageManager _pageManager;
        private DataManager _dataManager;
        private SessionDataManager _sessionDataManager;
        [SerializeField] private AttackManager _attackManager;
        [SerializeField] private SoundManager _soundManager;
        [SerializeField] private StartPage _startPage;
        [SerializeField] private SettingsPage _settingsPage;
        [SerializeField] private GamePage _gamePage;
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private ShipsManager _shipsManager;
        [SerializeField] private MessageItemsController _messageItemsController;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _pageManager = new PageManager(_startPage, _gamePage, _settingsPage);
            _dataManager = new DataManager();
            _sessionDataManager = new SessionDataManager();
            _pageManager.SetPageState(PageState.StartPage);
            _soundManager.Init(_dataManager);
            _startPage.Init(() => _pageManager.SetPageState(PageState.GamePage),
                () => _pageManager.SetPageState(PageState.SettingsPage));
            _gamePage.Init(_gridManager.UserGrid,
                _gridManager.ComputerGrid,
                _gridManager.GridSize,
                () => _pageManager.SetPageState(PageState.StartPage),
                _shipsManager,
                _messageItemsController.StepMessageItems,
                _sessionDataManager,
                _pageManager.SetPageState,
                _gridManager.ResetGrid);
            _settingsPage.Init(() => _pageManager.SetPageState(PageState.StartPage), _dataManager);
            _gridManager.Init(_attackManager.ElementPressedForAttack);
            _shipsManager.Init(_gridManager, _soundManager, _gamePage.TryToActivateConfirmButton);
            _attackManager.Init(_soundManager, _messageItemsController, _shipsManager, _sessionDataManager,
                _gridManager, _gamePage);
        }
    }
}