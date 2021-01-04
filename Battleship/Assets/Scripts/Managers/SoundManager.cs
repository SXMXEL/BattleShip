using System;
using System.Linq;
using Data;
using UnityEngine;

namespace Managers
{
    public enum SfxType
    {
        Explosion,
        Miss,
        ShipPlaceSound
    }

    public class SoundManager : MonoBehaviour
    {
        private DataManager _dataManager;
        [SerializeField] private SfxItem[] _audioSources;
        [SerializeField] private AudioSource _audio;
        [SerializeField] private float _delayTime = 0.3f;

        public void Init(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public void PlaySfx(SfxType sfxType)
        {
            if (_dataManager.UserData.IsMuted == false)
            {
                _audio.clip = _audioSources.First(data => data.SfxType == sfxType).AudioClip;
                _audio.PlayDelayed(_delayTime);
            }

            if (_dataManager.UserData.IsMuted)
            {
                Debug.Log("Mute on");
            }
        }
    }

    [Serializable]
    public class SfxItem
    {
        public SfxType SfxType => _sfxType;
        [SerializeField] private SfxType _sfxType;
        public AudioClip AudioClip => _audioClip;
        [SerializeField] private AudioClip _audioClip;

        public SfxItem(SfxType sfxType, AudioClip audioClip)
        {
            _sfxType = sfxType;
            _audioClip = audioClip;
        }
    }
}