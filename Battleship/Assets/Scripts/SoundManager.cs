using System;
using System.Linq;
using UnityEngine;

public enum SfxType
{
    Explosion,
    Miss
}

public class SoundManager : MonoBehaviour
{
    [SerializeField] private SfxItem[] _audioSources;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private float _delayTime = 0.3f;
    
    public void PlaySfx(SfxType sfxType)
    {
        _audio.clip = _audioSources.First(data => data.SfxType == sfxType).AudioClip;
        _audio.PlayDelayed(_delayTime);
    }
}

[Serializable]
public class SfxItem
{
    public SfxType SfxType => _sfxType;
    [SerializeField]
    private SfxType _sfxType;
    public AudioClip AudioClip => _audio;
    [SerializeField]private AudioClip _audio;

    public SfxItem(SfxType sfxType, AudioClip audioClip)
    {
        _sfxType = sfxType;
        _audio = audioClip;
    }
}
