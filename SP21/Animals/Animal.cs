using System.Collections.Generic;
using System.Linq;

namespace SP21.Animals
{
    abstract class Animal
    {
        protected GameState State;
        public Coordinate.Point Coord, InitialCoord;
        public Coordinate.Direction? Dir = null;
        protected string Skin;

        protected Animal(GameState state, Coordinate.Point coord)
        {
            State = state;
            InitialCoord = coord;
            Coord = coord;
        }

        public void Draw()
        {
            State.Level.Draw(Coord.Copy(-1, 0), Skin);
        }

        public virtual void Hide()
        {
            State.Level.Draw(Coord.Copy(-1, 0), 3);
        }

        private void Move()
        {
            Coord.Move(Dir);
        }

        public virtual void Step()
        {
            Hide();
            Move();
            Draw();
        }

        public virtual void Reset()
        {
            Hide();
            Coord = InitialCoord;
            Dir = null;
            Draw();
        }

        protected IEnumerable<char> CharsForward(Coordinate.Direction direction)
        {
            switch (direction)
            {
                case Coordinate.Direction.Left:
                    return State.Level.Get(Coord.Copy(-2, 0), 1);
                case Coordinate.Direction.Right:
                    return State.Level.Get(Coord.Copy(2, 0), 1);
                case Coordinate.Direction.Up:
                    return State.Level.Get(Coord.Copy(-1, -1), 3);
                case Coordinate.Direction.Down:
                    return State.Level.Get(Coord.Copy(-1, 1), 3);
                default:
                    return new char[] { };
            }
        }
    }
}