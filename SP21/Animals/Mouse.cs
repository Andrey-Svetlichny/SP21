using System.Collections.Generic;

namespace SP21.Animals
{
    class Mouse : Animal
    {
        private Coordinate.Direction? _nextDirection;
        private const string Skin1 = "<$>", Skin2 = ">$<", SkinDied = "+$+";

        public Mouse(View view, GameState state, Coordinate.Point coord)
            : base(view, state, coord)
        {
            Skin = Skin1;
        }

        public List<Cat> Cats { get; set; }

        public override void Step()
        {
            if (_nextDirection != null && CanMove((Coordinate.Direction)_nextDirection))
            {
                Dir = _nextDirection;
            }
            else if (Dir == null || !CanMove((Coordinate.Direction)Dir))
            {
                return;
            }

            base.Step();
            Skin = Skin == Skin1 ? Skin2 : Skin1;

            // съесть крошки, увеличить счет
            var coords = new[] { Coord.Copy(-1, 0), Coord, Coord.Copy(+1, 0) };
            foreach (var coord in coords)
            {
                switch (State.Level.EatBreadcrumb(coord))
                {
                    case '.': 
                        State.Score++; 
                        break;
                    case '@':
                        State.Score += 5; 
                        Cats.ForEach(c => c.StartOzverinMode());
                        break;
                }
            }
        }

        public void Die()
        {
            Skin = SkinDied;
            Draw();
        }

        public override void Reset()
        {
            Skin = Skin1;
            base.Reset();
        }

        public void WantMove(Coordinate.Direction direction)
        {
            _nextDirection = direction;
        }
    }
}
