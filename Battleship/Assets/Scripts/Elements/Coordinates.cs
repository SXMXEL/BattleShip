﻿
namespace Elements
{
    public class Coordinates
    {
        public int X { get; }
        public int Y { get; }
    
        public Coordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return base.ToString() + "\nX = " + X + " Y = " + Y;
        }
        public string ToNormalString()
        {
            return (char)(X + 65) + " " + (Y + 1);
            
        }

        public bool IsSameCoordinates(Coordinates coordinates)
        {
            return coordinates.X == X && coordinates.Y == Y;
        }
    }
}
