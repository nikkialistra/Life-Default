using System;
using MapGeneration.Data;
using MapGeneration.Generators;
using MapGeneration.Settings;
using UnityEngine;

namespace MapGeneration
{
    public class MapPreview : MonoBehaviour
    {
        [SerializeField] private Renderer _textureRender;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private Material _meshMaterial;

        [SerializeField] private DrawMode _drawMode;

        [SerializeField] private MeshSettings _meshSettings;
        [SerializeField] private HeightMapSettings _heightMapSettings;

        [Range(0, MeshSettings.NumSupportedLODs - 1)]
        [SerializeField] private int _editorPreviewLOD;
        [SerializeField] private bool _autoUpdate;

        private static readonly int HeightMultiplier = Shader.PropertyToID("_Height_Multiplier");

        private enum DrawMode
        {
            NoiseMap,
            Mesh,
            FalloffMap
        };

        public bool AutoUpdate => _autoUpdate;

        public void DrawMapInEditor()
        {
            var heightMap = HeightMapGenerator.GenerateHeightMap(_meshSettings.NumVertsPerLine,
                _meshSettings.NumVertsPerLine, _heightMapSettings, Vector2.zero);

            switch (_drawMode)
            {
                case DrawMode.NoiseMap:
                    DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
                    break;
                case DrawMode.Mesh:
                    DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.Values, _meshSettings, _editorPreviewLOD));
                    break;
                case DrawMode.FalloffMap:
                    DrawTexture(TextureGenerator.TextureFromHeightMap(
                        new HeightMap(FalloffGenerator.GenerateFalloffMap(_meshSettings.NumVertsPerLine), 0, 1)));
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
            _meshFilter.gameObject.SetActive(false);
        }

        private void DrawMesh(MeshData meshData)
        {
            _meshFilter.sharedMesh = meshData.CreateMesh();

            _textureRender.gameObject.SetActive(false);
            _meshFilter.gameObject.SetActive(true);
            _meshMaterial.SetFloat(HeightMultiplier, _heightMapSettings.MaxHeight - _heightMapSettings.MinHeight);
        }


        private void OnValuesUpdated()
        {
            if (!Application.isPlaying)
            {
                DrawMapInEditor();
            }
        }

        private void OnValidate()
        {
            if (_meshSettings != null)
            {
                _meshSettings.OnValuesUpdated -= OnValuesUpdated;
                _meshSettings.OnValuesUpdated += OnValuesUpdated;
            }

            if (_heightMapSettings != null)
            {
                _heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
                _heightMapSettings.OnValuesUpdated += OnValuesUpdated;
            }
        }
    }
}