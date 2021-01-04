using System;
using DG.Tweening;
using UnityEngine;

namespace Commands
{
    public class DelayWrappedCommand
    {
        private readonly Action _callback;
        private readonly float _delay;
        private Sequence _sequence;

        public bool IsRunning => _sequence.IsPlaying();

        public DelayWrappedCommand(Action callback, float delay)
        {
            _callback = callback;
            if (delay < 0)
            {
                delay = 0;
            }

            _delay = delay;

            _sequence = DOTween.Sequence();
        }

        public void Started()
        {
            if (IsRunning)
            {
                Debug.LogWarning("Command is already running.");
                return;
            }

            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(DOTween.To(() => 0, value => { }, 0, _delay));
            _sequence.OnComplete(() => { _callback?.Invoke(); });
            _sequence.Play();
        }

        public void Cancel()
        {
            if (!IsRunning)
            {
                Debug.LogWarning("Command is already completed.");
                return;
            }
            _sequence?.Kill();
        }
    }
}