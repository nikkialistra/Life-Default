using System;
using System.Collections.Generic;
using Game.Units;
using Game.Units.Unit;

namespace Kernel.Saving
{
    [Serializable]
    public class SaveData
    {
        public IList<UnitData> Units = new List<UnitData>();
    }
}