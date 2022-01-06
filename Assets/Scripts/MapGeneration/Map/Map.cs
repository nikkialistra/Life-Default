using System;
using MapGeneration.Generators;
using Zenject;

namespace MapGeneration.Map
{
    public class Map : IInitializable, IDisposable
    {
        private readonly MapGenerator.Factory _mapGeneratorFactory;

        private MapGenerator _mapGenerator;

        public Map(MapGenerator.Factory mapGeneratorFactory)
        {
            _mapGeneratorFactory = mapGeneratorFactory;
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
            Load?.Invoke();
        }
    }
}