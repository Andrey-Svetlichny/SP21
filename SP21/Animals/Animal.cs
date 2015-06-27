using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SP21.Animals
{
    abstract class Animal
    {
        protected readonly View View;
        protected readonly GameState State;
        public Coordinate.Point Coord, InitialCoord;
        public Coordinate.Direction? Dir = null;
        protected string Skin;
        protected int StepSleepTime = 15;

        protected Animal(View view, GameState state, Coordinate.Point coord)
        {
            View = view;
            State = state;
            InitialCoord = coord;
            Coord = coord;
        }

        public void Draw()
        {
            View.Draw(Coord.Copy(-1, 0), Skin);
        }

        public virtual void Hide()
        {
            View.Draw(Coord.Copy(-1, 0), Skin.Length);
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
            Thread.Sleep(StepSleepTime);
        }

        public virtual void Reset()
        {
            Hide();
            Coord = InitialCoord;
            Dir = null;
            Draw();
        }

        protected bool CanMove(Coordinate.Direction direction)
        {
            var allowedChars = new[] { ' ', '.', '@' };
            return CharsForward(direction).All(c => allowedChars.Any(allowed => c == allowed));
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