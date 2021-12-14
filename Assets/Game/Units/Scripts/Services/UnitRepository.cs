using System.Collections.Generic;
using UnityEngine;

namespace Game.Units.Services
{
    public class UnitRepository : MonoBehaviour
    {
        private IEnumerable<UnitFacade> _gameObjects;

        private void Start()
        {
            _gameObjects = FindObjectsOfType<UnitFacade>();
        }

        public IEnumerable<UnitFacade> GetObjects() => _gameObjects ??= FindObjectsOfType<UnitFacade>();

        public void ResetObjects()
        {
            _gameObjects = null;
        }
    }
}