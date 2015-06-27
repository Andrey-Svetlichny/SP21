using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SP21.Animals
{
    class Mouse : Animal
    {
        private Coordinate.Direction? _nextDirection;
        private const string Skin1 = "<$>", Skin2 = ">$<", SkinDied1 = "$+$", SkinDied2 = "+$+";

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

            if (Dir != null && !CanMove((Coordinate.Direction)Dir))
            {
                Dir = null;
            }

            if (Dir != null)
            {
                Skin = Skin == Skin1 ? Skin2 : Skin1;

                // съесть крошки, увеличить счет
                var coords = new[] { Coord.Copy(-1, 0), Coord, Coord.Copy(+1, 0) };
                var eatedChar = coords.Select(c => State.Level.EatBreadcrumb(c)).SingleOrDefault(c => c != ' ');
                switch (eatedChar)
                {
                    case '.':
                        View.IncreaseScore(1);
                        break;
                    case '@':
                        View.IncreaseScore(5);
                        _catsEatenDuringCurrentOzverin = 0;
                        Cats.ForEach(c => c.StartOzverinMode());
                        break;
                }
            }

            base.Step();
        }

        private int _catsEatenDuringCurrentOzverin;

        public void EatCat()
        {
            View.IncreaseScore(++_catsEatenDuringCurrentOzverin * 20);
        }

        public void Die()
        {
            for (int i = 0; i < 4; i++)
            {
                Skin = SkinDied1;
                Draw();
                Thread.Sleep(100);
                Skin = SkinDied2;
                Draw();
                Thread.Sleep(100);
            }
        }

        public override void Reset()
        {
            base.Reset();
            Skin = Skin1;
            _nextDirection = null;
        }

        public void WantMove(Coordinate.Direction direction)
        {
            _nextDirection = direction;
        }

    }
}
