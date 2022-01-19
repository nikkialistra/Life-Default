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
        private AstarPath _astarPath;
        private TextAsset _graphData;

        public Map(MapGenerator.Factory mapGeneratorFactory, AstarPath astarPath, TextAsset graphData)
        {
            _mapGeneratorFactory = mapGeneratorFactory;
            _astarPath = astarPath;
            _graphData = graphData;
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
            //_astarPath.Scan();
            _astarPath.data.DeserializeGraphs(_graphData.bytes);

            Load?.Invoke();
        }
    }
}
