using System;

namespace SnakeGame
{
    public class Snake : IComparable<Snake>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Snake() : this(0, 0) { }

        public Snake(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int CompareTo(Snake square)
        {
            int result = 0;

            int x = this.X.CompareTo(square.X);
            int y = this.Y.CompareTo(square.Y);

            if (!(x == 0 && y == 0))
            {
                result = -1;
            }

            return result;
        }

        public void LoadBait(int width, int height)
        {
            Random random = new Random();

            X = random.Next(0, width);
            Y = random.Next(0, height);
        }
    }
}
