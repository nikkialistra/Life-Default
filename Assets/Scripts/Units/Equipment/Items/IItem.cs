using Units.Enums;

namespace Units.Equipment.Items
{
    public interface IItem
    {
        ItemType ItemType { get; }
        object Content { get; }
    }
}
