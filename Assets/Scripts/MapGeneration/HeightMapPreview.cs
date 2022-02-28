using System;
using MapGeneration.Data;
using MapGeneration.Generators;
using MapGeneration.Settings;
using MapGeneration.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MapGeneration
{
    public class HeightMapPreview : MonoBehaviour
    {
        [SerializeField] private Renderer _textureRender;

        [SerializeField] private DrawMode _drawMode;
        
        [InlineEditor(InlineEditorModes.FullEditor)]
        [SerializeField] private HeightMapSettings _heightMapSettings;
        [SerializeField] private bool _autoUpdate;

        private enum DrawMode
        {
            NoiseMap,
            FalloffMap,
            HeightMap
        }

        private void OnValidate()
        {
            if (_autoUpdate)
            {
                _heightMapSettings.Update -= DrawInEditor;
                _heightMapSettings.Update += DrawInEditor;
            }
            else
            {
                _heightMapSettings.Update -= DrawInEditor;
            }
        }
        
        [Button("Export Texture")]
        public void ExportTexture(string name = "HeightMap.png")
        {
            var heightMap = HeightMapGenerator.GenerateHeightMap(_heightMapSettings, Vector2.zero);
            TextureGenerator.ExportHeightMap(heightMap, name);
        }

        [Button("Generate")]
        public void DrawInEditor()
        {
            switch (_drawMode)
            {
                case DrawMode.NoiseMap:
                    DrawNoiseMap();
                    break;
                case DrawMode.FalloffMap:
                    DrawFalloffMap();
                    break;
                case DrawMode.HeightMap:
                    DrawHeightMap();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Button("Apply To Terrain")]
        public void ApplyToTerrain()
        {
            HeightMapApplying.ApplyHeightMapFrom(GetTexture());
        }
        
        [Button("Apply To Terrain From Selection")]
        public void ApplyToTerrainFromSelection()
        {
            HeightMapApplying.ApplyHeightMapFromSelection();
        }

        private Texture2D GetTexture()
        {
            var heightMap = HeightMapGenerator.GenerateHeightMap(_heightMapSettings, Vector2.zero);
            return TextureGenerator.TextureFromHeightMap(heightMap);
        }
        
        private void DrawNoiseMap()
        {
            var noiseMap = HeightMapGenerator.GenerateNoiseMap(_heightMapSettings, Vector2.zero);
            DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }

        private void DrawFalloffMap()
        {
            var falloffMap = FalloffGenerator.GenerateFalloffMap(_heightMapSettings.NoiseSettings.Size);
            DrawTexture(TextureGenerator.TextureFromHeightMap(falloffMap));
        }

        private void DrawHeightMap()
        {
            var heightMap = HeightMapGenerator.GenerateHeightMap(_heightMapSettings, Vector2.zero);
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        }

        private void DrawTexture(Texture2D texture)
        {
            _textureRender.sharedMaterial.mainTexture = texture;
        }
    }
}
