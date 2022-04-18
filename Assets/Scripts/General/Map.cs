using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace General
{
    public class Map : MonoBehaviour
    {
        private AstarPath _astarPath;

        [Inject]
        public void Construct(AstarPath astarPath)
        {
            _astarPath = astarPath;
        }

        public event Action Load;

        private void Start()
        {
            StartCoroutine(GetAstarGraph());
        }

        private IEnumerator GetAstarGraph()
        {
            if (_astarPath.data.cacheStartup)
            {
                // Skip one frame for monobehaviour start methods to be invoked
                yield return null;
            }
            else
            {
                yield return _astarPath.ScanAsync();
            }

            Load?.Invoke();
        }
    }
}
