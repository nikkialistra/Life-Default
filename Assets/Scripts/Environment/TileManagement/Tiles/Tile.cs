using Pathfinding;

namespace Environment.TileManagement.Tiles
{
    public class Tile
    {
        public Int3 Position;
        
        public float Temperature;
        public int Light;
		public int Beauty;

        public Tile(Int3 position)
        {
            Position = position;
        }
    }
}
