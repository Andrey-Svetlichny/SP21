using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using SP21.Animals;

namespace SP21
{
    internal class GameActivity
    {
        GameState _state;
        View _view;
        Mouse _mouse;
        readonly List<Cat> _cats = new List<Cat>();

        public void Show()
        {

            // initialize
            int levelNum = 1;
            _state = new GameState {Level = new Level(levelNum)};
            _view = new View(_state);
            _view.DrawLevel();
            _view.DrawLife();

            // init mouse and cats
            _mouse = new Mouse(_view, _state, _state.Level.InitialMouseCoordinate);
            _cats.AddRange(_state.Level.InitialCatsCoordinate.Select(o => new Cat(_view, _state, o, _mouse)));
            _mouse.Cats = _cats;
            _mouse.Draw();

            while (_state.Life > 0)
            {
                var direction = GetMouseDirectionFromKeyboard();
                if (direction != null)
                {
                    _mouse.WantMove((Coordinate.Direction) direction);
                }
                _mouse.Step();
                CheckCatch();                
                _cats.ForEach(c => c.Step());
                CheckCatch();
                _mouse.Draw();
                

                if (_state.Level.Breadcrumbs == 0)
                {
                    // съедены все хлебные крошки
                    _state.Level = new Level(++levelNum);
                    _view.DrawLevel();
                    _view.DrawLife();
                    // init mouse and cats
                    _mouse.Reset();
                    _cats.ForEach(c => c.Reset());
                }
            }
        }

        /// <summary>
        /// Желаемое направление движение мыши (нажата стрелка на клавиатуре).
        /// </summary>
        private static Coordinate.Direction? GetMouseDirectionFromKeyboard()
        {
            if (Console.KeyAvailable)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.LeftArrow:
                        return Coordinate.Direction.Left;
                    case ConsoleKey.RightArrow:
                        return Coordinate.Direction.Right;
                    case ConsoleKey.UpArrow:
                        return Coordinate.Direction.Up;
                    case ConsoleKey.DownArrow:
                        return Coordinate.Direction.Down;
                }
            }
            return null;
        }


        /// <summary>
        /// Кошки поймали мышь (или наоборот)?
        /// </summary>
        private void CheckCatch()
        {
            // мышь поймала кошку
            var catchedCats = _cats.Where(c => c.Mode == Cat.ModeEnum.Game && c.Coord == _mouse.Coord).ToList();
            foreach (var catchedCat in catchedCats)
            {
                _mouse.EatCat();
                catchedCat.Mode = Cat.ModeEnum.Shadow;
            }

            // кошка моймала мышь
            if (_cats.Exists(c => c.Mode == Cat.ModeEnum.Normal && c.Coord == _mouse.Coord))
            {
                _state.Life--;
                _view.DrawLife();
                _mouse.Die();
                Thread.Sleep(1000);
                _cats.ForEach(c => { c.Hide(); Thread.Sleep(200); });
                _mouse.Reset();
                _cats.ForEach(c => c.Reset());
            }
        }
    }
}