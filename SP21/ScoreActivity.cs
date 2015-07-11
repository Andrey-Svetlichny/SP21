using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace SP21
{
    internal class ScoreActivity
    {

        private const string ScoreFile = "SP21.dat";

        private const int RowsCount = 20;

        private bool _isFirstRun;

        public ScoreActivity()
        {
            _isFirstRun = true;
            Load();
        }

        public int NoNameScore { get; set; }

        public List<Score> Scores { get; set; }

        /// <summary>
        /// Показывает таблицу рекордов.
        /// </summary>
        /// <returns>Начинать игру.</returns>
        public bool Show(int score)
        {
            var newScore = new Score { Value = score };
            if (score > 0)
            {
                Scores.Add(newScore);
                Scores = Scores.OrderByDescending(o => o.Value).ToList();
                Scores.RemoveAt(Scores.Count - 1);
            }
            var position = Scores.IndexOf(newScore);

            Console.Clear();
            Console.WriteLine("      PAC-HALL");
            Console.WriteLine("--------------------");

            for (int i = 0; i < Scores.Count; i++)
            {
                bool isLastRow = i + 1 == Scores.Count;
                Console.WriteLine(".{0,2}.{1,-9} {2,6}",
                    isLastRow ? "NN" : string.Format("{0:00}", i + 1),
                    isLastRow ? "*********" : Scores[i].Name,
                    Scores[i].Value);
            }

            if (position >= 0)
            {
                Console.SetCursorPosition(36, 11);
                Console.WriteLine("ENTER YOU NAME !");
                Console.SetCursorPosition(4, position + 2);
                Console.CursorVisible = true;
                var name = (Console.ReadLine() ?? "");
                newScore.Name = name.Length > 6 ? name.Substring(0, 6) : name;
                Console.CursorVisible = false;
                Save();
            }
            else
            {
                NoNameScore = score;
            }


            Console.SetCursorPosition(36, 11);
            Console.WriteLine(_isFirstRun ? "BEGIN ?" : "ONCE MORE ?     ");
            _isFirstRun = false;
            while (true)
            {
                var keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.N)
                {
                    return false;
                }

                if (keyInfo.Key == ConsoleKey.Y)
                {
                    return true;
                }
            }
        }

        private void Load()
        {
            try
            {
                using (var fs = new FileStream(ScoreFile, FileMode.Open))
                {
                    Scores = (List<Score>)new BinaryFormatter().Deserialize(fs);
                }
            }
            catch (Exception)
            {
                Scores = new List<Score>();
                for (int i = Scores.Count; i < RowsCount; i++)
                {
                    Scores.Add(new Score());
                }
            }
            // NoName score
            Scores.Add(new Score());
        }

        private void Save()
        {
            using (var fs = new FileStream(ScoreFile, FileMode.Create))
            {
                new BinaryFormatter().Serialize(fs, Scores.Take(RowsCount).ToList());
            }
        }

        [Serializable]
        public class Score
        {

            public String Name { get; set; } 
            public int Value { get; set; }

        }
    }
}