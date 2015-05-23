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
        Mouse _mouse;
        readonly List<Cat> _cats = new List<Cat>();

        public void Show()
        {

            // initilize
            _state = new GameState();
            int levelNum = 1;
            _state.Level = new Level(levelNum);
            _state.Level.Draw();
            DrawLife();

            // init mouse and cats
            _mouse = new Mouse(_state, _state.Level.InitialMouseCoordinate);
            _cats.AddRange(_state.Level.InitialCatsCoordinate.Select(o => new Cat(_state, o, _mouse)));
            _mouse.Cats = _cats;
         
            //_cats.ForEach(c => c.Draw());

            _mouse.Draw();
            //Console.ReadKey();

            while (_state.Life > 0)
            {
                //Mouse.Move()
                //score += Mouse.Eat()
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
                Thread.Sleep(60);

                if (_state.Level.Breadcrumbs == 0)
                {
                    // съедены все хлебные крошки
                    _state.Level = new Level(++levelNum);
                    _state.Level.Draw();
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
            catchedCats.ForEach(c => c.Mode = Cat.ModeEnum.Shadow);

            // кошка моймала мышь
            if (_cats.Exists(c => c.Mode == Cat.ModeEnum.Normal && c.Coord == _mouse.Coord))
            {
                _state.Life--;
                DrawLife();
                _mouse.Die();
                Thread.Sleep(1000);
                _mouse.Reset();
                _cats.ForEach(c => c.Reset());
            }
        }


        private void DrawLife()
        {
            for (int i = 0; i < 10; i++)
            {
                _state.Level.Draw(new Coordinate.Point { X = 79, Y = i }, i < _state.Life - 1 ? "X" : " ");
            }
        }

    }
}