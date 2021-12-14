using System;
using System.Collections.Generic;
using Game.Units;

namespace Kernel.Saving
{
    [Serializable]
    public class SaveData
    {
        public IEnumerable<UnitData> Units = new List<UnitData>();
    }
}