using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlaySound
{
    Explosion,
    Miss,
}

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _explosionAudioSource;
    [SerializeField] private AudioSource _missAudioSource;
    private PlaySound _playSound;

}
