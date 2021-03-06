﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StartPage : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _settingsMenuButton;
        public void Init(Action OnSetGamePageState, Action OnSetSettingsPageState)
        {
            _startButton.onClick.RemoveAllListeners();
            _startButton.onClick.AddListener(OnSetGamePageState.Invoke);
            _settingsMenuButton.onClick.RemoveAllListeners();
            _settingsMenuButton.onClick.AddListener(OnSetSettingsPageState.Invoke);
        }
    }
}
