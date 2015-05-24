using System;

namespace SP21
{
    public class View
    {
        private readonly GameState _state;

        public View(GameState state)
        {
            _state = state;
        }

        public void DrawLevel()
        {
            for (int y = 0; y < Coordinate.MaxY; y++)
                for (int x = 0; x < Coordinate.MaxX; x++)
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(_state.Level[new Coordinate.Point(x, y)]);
                }
        }

        public void Draw(Coordinate.Point point, string s)
        {
            Console.SetCursorPosition(point.X, point.Y);
            Console.Write(s);
        }

        public void Draw(Coordinate.Point point, int length)
        {
            Console.SetCursorPosition(point.X, point.Y);
            Console.Write(string.Concat(_state.Level.Get(point, length)));
        }

        public void DrawLife()
        {
            for (int i = 0; i < 10; i++)
            {
                Draw(new Coordinate.Point { X = 79, Y = i }, i < _state.Life - 1 ? "X" : " ");
            }
        }

        public void DrawScore()
        {
            Draw(new Coordinate.Point { X = 36, Y = 18 }, string.Format("{0,6}", _state.Score));
        }
    }
}
