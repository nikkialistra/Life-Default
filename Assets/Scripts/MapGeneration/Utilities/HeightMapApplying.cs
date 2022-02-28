using UnityEditor;
using UnityEngine;

namespace MapGeneration.Utilities
{
	public class HeightMapApplying
	{
		public static void ApplyHeightMapFrom(Texture2D texture)
		{
			UpdateTerrain(texture);
		}

		public static void ApplyHeightMapFromSelection()
		{
			var texture = Selection.activeObject as Texture2D;
			if (texture == null)
			{
				EditorUtility.DisplayDialog("No texture selected", "Please select a texture.", "Cancel");
				return;
			}

			UpdateTerrain(texture);
		}

		private static void UpdateTerrain(Texture2D texture)
		{
			Undo.RegisterCompleteObjectUndo(Terrain.activeTerrain.terrainData, "Heightmap From Texture");

			var terrain = Terrain.activeTerrain.terrainData;
			var heightmapResolution = terrain.heightmapResolution;

			if (heightmapResolution != texture.width || heightmapResolution != texture.height)
			{
				EditorUtility.DisplayDialog("Incompatible sized", "Terrain and texture has different sizes", "Cancel");
				return;
			}

			UpdateTerrainHeights(terrain, heightmapResolution, texture);
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