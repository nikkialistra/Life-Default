using MapGeneration.Generators;
using Zenject;

namespace MapGeneration
{
    public class Map : IInitializable
    {
        private readonly MapGenerator.Factory _mapGeneratorFactory;

        public Map(MapGenerator.Factory mapGeneratorFactory)
        {
            _mapGeneratorFactory = mapGeneratorFactory;
        }

        public void Initialize()
        {
            var map = _mapGeneratorFactory.Create();
        }
    }
}