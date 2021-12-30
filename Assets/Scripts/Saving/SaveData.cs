using System;
using System.Collections.Generic;
using Units.Unit;

namespace Saving
{
    [Serializable]
    public class SaveData
    {
        public IList<UnitData> Units = new List<UnitData>();
    }
}