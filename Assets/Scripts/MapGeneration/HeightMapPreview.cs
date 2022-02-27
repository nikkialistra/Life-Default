using System;
using MapGeneration.Data;
using MapGeneration.Generators;
using MapGeneration.Settings;
using UnityEngine;

namespace MapGeneration
{
    public class HeightMapPreview : MonoBehaviour
    {
        [SerializeField] private Renderer _textureRender;

        [SerializeField] private DrawMode _drawMode;
        
        [SerializeField] private HeightMapSettings _heightMapSettings;
        [SerializeField] private bool _autoUpdate;

        private static readonly int HeightMultiplier = Shader.PropertyToID("_Height_Multiplier");

        private enum DrawMode
        {
            NoiseMap,
            FalloffMap,
            HeightMap
        };

        public bool AutoUpdate => _autoUpdate;

        public void DrawMapInEditor()
        {
            var heightMap = HeightMapGenerator.GenerateHeightMap(_heightMapSettings, Vector2.zero);

            switch (_drawMode)
            {
                case DrawMode.NoiseMap:
                    DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
                    break;
                case DrawMode.FalloffMap:
                    DrawTexture(TextureGenerator.TextureFromHeightMap(
                        new HeightMap(FalloffGenerator.GenerateFalloffMap(_heightMapSettings.NoiseSettings.Size), 0, 1)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawTexture(Texture2D texture)
        {
            _textureRender.sharedMaterial.mainTexture = texture;
            _textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

            _textureRender.gameObject.SetActive(true);
        }
        
        private void OnValuesUpdated()
        {
            if (!Application.isPlaying)
            {
                DrawMapInEditor();
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
