using System;
using UnityEngine;

namespace MapGeneration
{
    public class Map
    {
        private readonly AstarPath _astarPath;
        private readonly TextAsset _graphData;

        private readonly bool _loadSavedGraphData;

        public Map(AstarPath astarPath, TextAsset graphData,
            bool loadSavedGraphData)
        {
            _astarPath = astarPath;
            _graphData = graphData;
            _loadSavedGraphData = loadSavedGraphData;
        }

        public event Action Load;

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
