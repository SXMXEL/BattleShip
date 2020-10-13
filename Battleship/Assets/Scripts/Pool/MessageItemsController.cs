using System;
using System.Collections.Generic;
using System.Linq;
using Elements;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Pool
{
    public class MessageItemsController : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollView;
        [SerializeField] private GameObject _scrollContent;
        [SerializeField] private MessageItemFactory _messageItemFactory;
        private List<StepMessageItem> _stepMessageItems = new List<StepMessageItem>();

        public void LogGenerate(ElementItem gridElement, OwnerType ownerType)
        {
            GenerateItem(new StepMessageData(GetMessageText(gridElement), ownerType));
            _scrollView.verticalNormalizedPosition = 0;
        }

        private void GenerateItem(StepMessageData stepMessageData)
        {
            var messageItem = _messageItemFactory.GetItem();
            messageItem.transform.SetParent(_scrollContent.transform);
            messageItem.Init(stepMessageData);
            _stepMessageItems.Add(messageItem);
            if (_stepMessageItems.Count > 20)
            {
                var firstItem = _stepMessageItems.First();
                _stepMessageItems.Remove(firstItem);
                _messageItemFactory.ReturnItem(firstItem);
            }
        }

        private string GetMessageText(ElementItem gridElement)
        {
            var tempText = string.Empty;

            switch (gridElement.GridElementType)
            {
                case GridElementType.None:
                    break;
                case GridElementType.Ship:
                    break;
                case GridElementType.DestroyedShip:
                    tempText = "Destroyed ship at  " + (gridElement.Coordinates.ToNormalString());
                    break;
                case GridElementType.Miss:
                    tempText = "Missed attack at  " + (gridElement.Coordinates.ToNormalString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return tempText;
        }
    }
}