using System;
using System.Collections.Generic;
using UnityEngine;

namespace Colonists.Services
{
    public class ColonistRepository : MonoBehaviour
    {
        private readonly List<Colonist> _colonists = new();

        public event Action<Colonist> Add;
        public event Action<Colonist> Remove;

        public IEnumerable<Colonist> GetColonists()
        {
            foreach (var colonist in _colonists)
            {
                if (colonist.Alive)
                {
                    yield return colonist;
                }
            }
        }

        public void AddColonist(Colonist colonist)
        {
            _colonists.Add(colonist);
            Add?.Invoke(colonist);
        }

        public void RemoveColonist(Colonist colonist)
        {
            _colonists.Remove(colonist);
            Remove?.Invoke(colonist);
        }
    }
}
