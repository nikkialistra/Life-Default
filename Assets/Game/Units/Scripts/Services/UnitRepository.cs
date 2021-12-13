using System.Collections.Generic;
using UnityEngine;

namespace Game.Units.Scripts.Services
{
    public class UnitRepository : MonoBehaviour
    {
        private IEnumerable<Unit> _gameObjects;

        public IEnumerable<Unit> GetObjects() => _gameObjects ??= FindObjectsOfType<Unit>();

        public void ResetObjects() => _gameObjects = null;
    }
}