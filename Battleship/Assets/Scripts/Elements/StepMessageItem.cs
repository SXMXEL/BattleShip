using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Elements
{
    public class StepMessageItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _stepText;
    
        public void Init(StepMessageData stepMessageData)
        {
            _stepText.text = stepMessageData.MessageText;
            switch (stepMessageData.OwnerType)
            {
                case OwnerType.User:
                    _stepText.alignment = TextAlignmentOptions.MidlineLeft;
                    break;
                case OwnerType.Computer:
                    _stepText.alignment = TextAlignmentOptions.MidlineRight;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Dispose()
        {
        }
    }

    public class StepMessageData
    {
        public readonly string MessageText;
        public readonly OwnerType OwnerType;

        public StepMessageData(string messageText, OwnerType ownerType)
        {
            MessageText = messageText;
            OwnerType = ownerType;
        }
    }
}