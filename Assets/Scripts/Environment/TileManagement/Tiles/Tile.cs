using UnityEngine;

namespace Environment.TileManagement.Tiles
{
    public class Tile
    {
        public readonly Vector2Int Position;
        
        public float Temperature;
        public int Light;
		public int Beauty;

        public Tile(Vector2Int position)
        {
            Position = position;
        }
    }
}
