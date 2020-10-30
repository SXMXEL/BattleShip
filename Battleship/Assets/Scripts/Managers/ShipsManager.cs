using UnityEngine;

namespace Elements
{
    
    public class ShipsManager : MonoBehaviour
    {
        public Ship[] Ships;
        private const float _doubleClickTime = 0.2f;
        private float _lastClickTime;
        public void Init(ElementItem[,] userGrid)
        {
            foreach (var ship in Ships)
            {
                ship.Init(userGrid);
            }
        }
    }
}
