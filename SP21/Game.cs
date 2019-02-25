using System;
using System.Collections.Generic;
using System.Linq;
using SP21.Animals;

namespace SP21
{
    public class Game
    {
        private static Random _random;
        readonly Level _level;
        readonly ConsoleView _view;
        readonly ScoreTable _scoreTable;
        List<Cat> _cats;
        Mouse _mouse;
        int _score;
        int _breadcrumbs;
        int _catsEatenDuringCurrentOzverin;
        int _bonus;

        /// <summary>
        /// Осталось времени текущему озверину.
        /// </summary>
        int _ozverinRemains;

        /// <summary>
        /// Осталось времени до показа/исчезновения бонуса.
        /// </summary>
        int _bonusRemains;

        public Game()
        {
            _random = new Random();
            _level = new Level();
            _view = new ConsoleView();
            _scoreTable = new ScoreTable();
            _scoreTable.Load();
        }

        public void Run()
        {
            _view.DrawScoreTable(_scoreTable);
            if (!ConsoleView.UserWantsToPlay(true))
            {
                return;
            }

            while (true)
            {
                _level.LevelNum = 1;
                _level.Lives = 3;
                _breadcrumbs = _level.Breadcrumbs;
                _score = 0;
                _ozverinRemains = 0;
                InitBonus();
                InitAnimals();
                _view.DrawLevel(_level);
                foreach (var cat in _cats)
                {
                    _view.Draw(cat, _level);
                }
                _view.Draw(_mouse, _level);

                while (_level.Lives > 0)
                {
                    GameStep();
                }

                var position = _scoreTable.Add(_score);
                _view.DrawScoreTable(_scoreTable);
                if (position != -1)
                {
                    _scoreTable.Scores[position].Name = ConsoleView.EnterName(position);
                    _scoreTable.Save();
                }
                if (!ConsoleView.UserWantsToPlay(false))
                    break;
            }
        }

        /// <summary>
        /// Сбросить бонус и завести таймер для показа бонуса
        /// </summary>
        private void InitBonus()
        {
            _bonus = 0;
            _bonusRemains = 500 + _random.Next(200);
        }

        private void GameStep()
        {
            if (_bonus == 0 && --_bonusRemains == 0)
            {
                // время показать бонус
                _bonus = 15 * (_score / 500 + 1);
                _bonusRemains = 120;
                _level.SetBonus(_bonus);
                _view.DrawBonus(_level);
            }

            if (_bonus > 0 && --_bonusRemains == 0)
            {
                // кончилось время показа бонуса
                // на этом уровне бонусов больше не будет (_bonusRemains < 0)
                _bonus = 0;
                _level.SetBonus(_bonus);
                _view.DrawBonus(_level);
            }

            if (_ozverinRemains > 0 && --_ozverinRemains == 0)
            {
                // кончилось время работы озверина
                foreach (var cat in _cats)
                {
                    if (cat.Mode == Cat.ModeEnum.Prey)
                        cat.Mode = Cat.ModeEnum.Normal;
                }
            }

            // ход кошек
            if (_ozverinRemains == 0 || _ozverinRemains % Cat.PreyModeSkipStep != 0)
            {
                foreach (var cat in _cats)
                {
                    cat.Step(_mouse.Coord);
                    _view.Draw(cat, _level);
                    CheckCatch(cat, _mouse);
                }
            }

            // ход мыши
            var direction = ConsoleView.GetMouseDirectionFromKeyboard();
            if (direction != null)
            {
                _mouse.WantMove(direction.Value);
            }
            _mouse.Step();
            _view.Draw(_mouse, _level);
            var item = _level.Take(
                _mouse.Dir == Coordinate.Direction.Left ? _mouse.Coord - 1 :
                _mouse.Dir == Coordinate.Direction.Right ? _mouse.Coord + 1 : _mouse.Coord);
            switch (item)
            {
                case '*':
                    IncreaseScore(_bonus);
                    InitBonus();
                    _level.SetBonus(_bonus);
                    break;
                case '@':
                    IncreaseScore(5);
                    _view.DrawScore(_score);
                    _ozverinRemains = OzverinTime(_score);
                    foreach (var cat in _cats)
                    {
                        if (cat.Mode == Cat.ModeEnum.Shadow || _level.IsHome(cat.Coord))
                            continue;
                        if (_score < 2900 || _random.Next(8) > 0)
                            cat.Mode = Cat.ModeEnum.Prey;
                    }
                    _catsEatenDuringCurrentOzverin = 0;
                    break;
                case '.':
                    IncreaseScore(1);
                    _view.DrawScore(_score);
                    if (--_breadcrumbs == 0)
                    {
                        _level.LevelNum++;
                        _breadcrumbs = _level.Breadcrumbs;
                        _view.DrawLevel(_level);
                        _view.DrawScore(_score);
                        InitAnimals();
                    }
                    break;
            }
            _cats.ForEach(cat => CheckCatch(cat, _mouse));
        }

        /// <summary>
        /// Время действия озверина
        /// </summary>
        private int OzverinTime(int score)
        {
            if (score <  500) return 180;
            if (score < 1000) return 144;
            if (score < 1900) return 120;
            if (score < 2000) return 108;
            if (score < 2500) return 96;
            if (score < 2800) return 84;
            if (score < 2900) return 72;
            return 54;
        }

        /// <summary>
        /// Кошка поймала мышь (или озверевшая мышь поймала кошку)?
        /// </summary>
        private void CheckCatch(Cat cat, Mouse mouse1)
        {
            if (cat.Coord != mouse1.Coord)
            {
                return;
            }

            // кошка поймала мышь 
            if (cat.Mode == Cat.ModeEnum.Normal)
            {
                _level.Lives--;
                InitBonus();
                _level.SetBonus(_bonus );
                _view.MouseDieAnimation(_level, _cats, _mouse);
                _view.DrawLevel(_level);
                InitAnimals();
            }

            // мышь поймала кошку
            if (cat.Mode == Cat.ModeEnum.Prey)
            {
                IncreaseScore(++_catsEatenDuringCurrentOzverin * 20);
                cat.Mode = Cat.ModeEnum.Shadow;
            }
        }

        public void IncreaseScore(int scoreDiff)
        {
            var newScore = _score + scoreDiff;
            if (newScore / 500 > _score / 500)
            {
                _view.DrawNewLife(++_level.Lives);
            }
            _score = newScore;
            _view.DrawScore(_score);
        }


        private void InitAnimals()
        {
            if (_cats == null)
            {
                _cats = _level.InitialCatsCoordinateDirection.Select(o => new Cat()).ToList();
                _mouse = new Mouse();
            }

            for (int i = 0; i < _level.InitialCatsCoordinateDirection.Length; i++)
            {
                _cats[i].Level = _level;
                _cats[i].Coord = _level.InitialCatsCoordinateDirection[i].Item1;
                _cats[i].Dir = _level.InitialCatsCoordinateDirection[i].Item2;
                _cats[i].Mode = Cat.ModeEnum.Normal;
            }
            _mouse.Level = _level;
            _mouse.Coord = _level.InitialMouseCoordinate;
        }
    }
}
