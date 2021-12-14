using System;
using System.Collections.Generic;
using Game.Units;

namespace Kernel.Saving
{
    [Serializable]
    public class SaveData
    {
        private static SaveData _current;
        
        private SaveData() { }

        public static SaveData Current
        {
            get
            {
                return _current ??= new SaveData();
            }
            set
            {
                _current = value;
            }
        }

        public IEnumerable<UnitData> Units = new List<UnitData>();
    }
}