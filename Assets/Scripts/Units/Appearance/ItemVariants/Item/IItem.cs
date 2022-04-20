namespace Units.Appearance.ItemVariants.Item
{
    public interface IItem<T>
    {
        public T Value { get; }
        public int Chance { get; }
        public float RelativeChance { get; set; }
    }
}
