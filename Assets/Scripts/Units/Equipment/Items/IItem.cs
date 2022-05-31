using Units.Enums;

namespace Units.Equipment.Items
{
    public interface IItem
    {
        ItemType ItemType { get; }
        object Content { get; }
        
        public bool TryGetWeapon(out Weapon weapon)
        {
            if (ItemType == ItemType.Weapon)
            {
                weapon = (Weapon)Content;
                return true;
            }

            weapon = null;
            return false;
        }
        
        public bool TryGetTool(out Tool tool)
        {
            if (ItemType == ItemType.Tool)
            {
                tool = (Tool)Content;
                return true;
            }

            tool = null;
            return false;
        }
        
        public bool TryGetProperty(out Property property)
        {
            if (ItemType == ItemType.Property)
            {
                property = (Property)Content;
                return true;
            }

            property = null;
            return false;
        }
        
        public bool TryGetConsumable(out Consumable consumable)
        {
            if (ItemType == ItemType.Consumable)
            {
                consumable = (Consumable)Content;
                return true;
            }

            consumable = null;
            return false;
        }
    }
}
