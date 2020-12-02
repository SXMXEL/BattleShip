using System;
using System.Collections.Generic;
using System.Linq;
using Elements;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Pool
{
    public class MessageItemsController : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GameObject _scrollContent;
        [SerializeField] private MessageItemFactory _messageItemFactory;
        public List<StepMessageItem> StepMessageItems = new List<StepMessageItem>();

        public void LogGenerate(ElementItem gridElement, OwnerType ownerType)
        {
            GenerateItem(new StepMessageData(GetMessageText(gridElement), ownerType));
            _scrollRect.verticalNormalizedPosition = -0.2f;
            
        }

        private void GenerateItem(StepMessageData stepMessageData)
        {
            var messageItem = _messageItemFactory.GetItem();
            messageItem.transform.SetParent(_scrollContent.transform);
            messageItem.Init(stepMessageData);
            StepMessageItems.Add(messageItem);
            if (StepMessageItems.Count > 20)
            {
                var firstItem = StepMessageItems.First();
                StepMessageItems.Remove(firstItem);
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