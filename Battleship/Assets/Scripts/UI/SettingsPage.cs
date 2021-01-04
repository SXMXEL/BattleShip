using System;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SettingsPage : MonoBehaviour
    {
        [SerializeField] private Button _backToStartMenuButton;
        [SerializeField] private Button _muteButton;
        [SerializeField] private Sprite[] _soundSprites;
        [SerializeField] private Sprite currentSoundButtonIcon;
        private DataManager _dataManager;

   

        public void Init(Action OnSetStartPageState, DataManager dataManager)
        {
            _dataManager = dataManager;
            _backToStartMenuButton.onClick.RemoveAllListeners();
            _backToStartMenuButton.onClick.AddListener(OnSetStartPageState.Invoke);
            _muteButton.onClick.RemoveAllListeners();
            _muteButton.onClick.AddListener(() =>
            {
                MuteFunction();
                ChangeIcon();
            });
        }

        private void MuteFunction()
        {
            _dataManager.UserData.IsMuted = !_dataManager.UserData.IsMuted;
        }

        private void ChangeIcon()
        {
            switch (_dataManager.UserData.IsMuted)
            {
                case false:
                    currentSoundButtonIcon = _soundSprites[0];
                    break;
                case true:
                    currentSoundButtonIcon = _soundSprites[1];
                    break;
            }

            _muteButton.GetComponent<Image>().sprite = currentSoundButtonIcon;
        }
    }
}