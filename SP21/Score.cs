using System;

namespace SP21
{
    [Serializable]
    public class Score
    {
        private const int MaxNameLength = 6;
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value.Length <= MaxNameLength ? value : value.Substring(0, MaxNameLength);
            }
        }

        public int Value { get; set; }

    }
}