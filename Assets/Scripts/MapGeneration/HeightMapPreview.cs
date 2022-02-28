using System;
using MapGeneration.Data;
using MapGeneration.Generators;
using MapGeneration.Settings;
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
        };

        public bool AutoUpdate => _autoUpdate;

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

        public void ExportTexture()
        {
            var heightMap = HeightMapGenerator.GenerateHeightMap(_heightMapSettings, Vector2.zero);
            TextureGenerator.ExportHeightMap(heightMap);
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

        private void OnValuesUpdated()
        {
            if (!Application.isPlaying)
            {
                DrawInEditor();
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {

            if (_heightMapSettings != null)
            {
                _heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
                _heightMapSettings.OnValuesUpdated += OnValuesUpdated;
            }
        }

#endif
    }
}
