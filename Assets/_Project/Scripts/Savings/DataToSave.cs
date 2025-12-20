using System;

namespace Asteroids
{
    [Serializable]
    public class DataToSave
    {
        public int _bestScore;
        public string _saveDate;
        public bool _hasAdBlock;

        public DataToSave()
        {
            _bestScore = 0;
            _saveDate = "";
            _hasAdBlock = false;
        }
    }
}
