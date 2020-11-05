using System;
using Elements;
using UnityEngine;

namespace Managers
{
    public class ShipsManager : MonoBehaviour
    {
        public Ship[] Ships;

        public void Init(ElementItem[,] userGrid, bool confirm)
        {
            foreach (var ship in Ships)
            {
                ship.Init(userGrid);
                if (confirm)
                {
                    ship.CantDrag = true;
                }
            }
        }
    }
}