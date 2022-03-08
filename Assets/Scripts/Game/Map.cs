using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Map : MonoBehaviour
    {
        [SerializeField] private bool _loadSavedGraphData;
        
        private AstarPath _astarPath;
        private TextAsset _graphData;

        [Inject]
        public void Construct(AstarPath astarPath, TextAsset graphData)
        {
            _astarPath = astarPath;
            _graphData = graphData;
        }

        public event Action Load;

        private void Start()
        {
            StartCoroutine(WaitAndLoad());
        }

        private IEnumerator WaitAndLoad()
        {
            yield return new WaitForSeconds(.3f);
            OnLoad();
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
