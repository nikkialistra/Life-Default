using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aborigines.Services
{
    public class AborigineRepository : MonoBehaviour
    {
        public event Action<Aborigine> Add;
        public event Action<Aborigine> Remove;

        public int Count => _aborigines.Count;

        private readonly List<Aborigine> _aborigines = new();

        public IEnumerable<Aborigine> GetAborigines()
        {
            foreach (var aborigine in _aborigines)
                if (aborigine.Alive)
                    yield return aborigine;
        }

        public void AddAborigine(Aborigine colonist)
        {
            _aborigines.Add(colonist);
            Add?.Invoke(colonist);
        }

        public void RemoveAborigine(Aborigine aborigine)
        {
            _aborigines.Remove(aborigine);
            Remove?.Invoke(aborigine);
        }
    }
}
