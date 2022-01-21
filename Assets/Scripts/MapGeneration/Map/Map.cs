using System;
using MapGeneration.Generators;
using UnityEngine;
using Zenject;

namespace MapGeneration.Map
{
    public class Map : IInitializable, IDisposable
    {
        private readonly MapGenerator.Factory _mapGeneratorFactory;

        private MapGenerator _mapGenerator;
        private readonly AstarPath _astarPath;
        private readonly TextAsset _graphData;

        private readonly bool _loadSavedGraphData;

        public Map(MapGenerator.Factory mapGeneratorFactory, AstarPath astarPath, TextAsset graphData,
            bool loadSavedGraphData)
        {
            _mapGeneratorFactory = mapGeneratorFactory;
            _astarPath = astarPath;
            _graphData = graphData;
            _loadSavedGraphData = loadSavedGraphData;
        }

        public event Action Load;

        public void Initialize()
        {
            _mapGenerator = _mapGeneratorFactory.Create();

            _mapGenerator.Load += OnLoad;
        }

        public void Dispose()
        {
            _mapGenerator.Load -= OnLoad;
        }

        private void OnLoad()
        {
            if (_loadSavedGraphData)
            {
                _astarPath.data.DeserializeGraphs(_graphData.bytes);
            }
            else
            {
                AstarPath.active.Scan();
            }

            Load?.Invoke();
        }
    }
}
