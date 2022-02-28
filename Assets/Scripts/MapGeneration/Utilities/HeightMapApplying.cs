using UnityEditor;
using UnityEngine;

namespace MapGeneration.Utilities
{
	public class HeightMapApplying
	{
		[MenuItem("Terrain/Heightmap From Texture")]
		public static void ApplyHeightmap()
		{
			var heightmap = Selection.activeObject as Texture2D;
			if (heightmap == null)
			{
				EditorUtility.DisplayDialog("No texture selected", "Please select a texture.", "Cancel");
				return;
			}

			Undo.RegisterCompleteObjectUndo(Terrain.activeTerrain.terrainData, "Heightmap From Texture");

			var terrain = Terrain.activeTerrain.terrainData;
			var heightmapResolution = terrain.heightmapResolution;
			
			if (heightmapResolution != heightmap.width || heightmapResolution != heightmap.height)
			{
				EditorUtility.DisplayDialog("Incompatible sized", "Terrain and texture has different sizes", "Cancel");
				return;
			}

			UpdateTerrainHeights(terrain, heightmapResolution, heightmap);
		}

		private static void UpdateTerrainHeights(TerrainData terrain, int heightmapResolution, Texture2D heightmap)
		{
			var heightmapData = terrain.GetHeights(0, 0, heightmapResolution, heightmapResolution);
			var pixels = heightmap.GetPixels();

			for (var y = 0; y < heightmapResolution; y++)
			{
				for (var x = 0; x < heightmapResolution; x++)
				{
					heightmapData[y, x] = pixels[y * heightmapResolution + x].grayscale;
				}
			}

			terrain.SetHeights(0, 0, heightmapData);
		}
	}
}