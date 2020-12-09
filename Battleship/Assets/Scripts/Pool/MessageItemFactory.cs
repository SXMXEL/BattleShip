using System.Collections.Generic;
using System.Linq;
using Elements;
using UnityEngine;

namespace Pool
{
    public class MessageItemFactory : MonoBehaviour
    {
        private readonly List<StepMessageItem> _usedItems = new List<StepMessageItem>();
        private readonly List<StepMessageItem> _unusedItems = new List<StepMessageItem>();
        [SerializeField] private StepMessageItem _prefabItem;

        public StepMessageItem GetItem()
        {
            var currentItem = _unusedItems.FirstOrDefault();
            if (currentItem != null)
            {
                _unusedItems.Remove(currentItem);
            }
            else
            {
                currentItem = Instantiate(_prefabItem, transform);
            }

            currentItem.gameObject.SetActive(true);
            _usedItems.Add(currentItem);
            return currentItem;
        }

        public void ReturnItem(StepMessageItem item)
        {
            _usedItems.Remove(item);
            item.gameObject.SetActive(false);
            item.Dispose();
            _unusedItems.Add(item);
        }
    }
}