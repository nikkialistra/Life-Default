using Units.Enums;

namespace Units.Equipment.Items
{
    public interface IItem
    {
        ItemType ItemType { get; }
        object Content { get; }
        
        public bool TryGetWeapon(out Weapon instrument)
        {
            if (ItemType == ItemType.Weapon)
            {
                instrument = (Weapon)Content;
                return true;
            }

            instrument = null;
            return false;
        }
        
        public bool TryGetInstrument(out Instrument instrument)
        {
            if (ItemType == ItemType.Instrument)
            {
                instrument = (Instrument)Content;
                return true;
            }

            instrument = null;
            return false;
        }
        
        public bool TryGetProperty(out Property instrument)
        {
            if (ItemType == ItemType.Property)
            {
                instrument = (Property)Content;
                return true;
            }

            instrument = null;
            return false;
        }
        
        public bool TryGetConsumable(out Consumable instrument)
        {
            if (ItemType == ItemType.Consumable)
            {
                instrument = (Consumable)Content;
                return true;
            }

            instrument = null;
            return false;
        }
    }
}
