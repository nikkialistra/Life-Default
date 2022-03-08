using System;
using System.Collections.Generic;
using System.Linq;
using Colonists.Colonist;
using UnityEngine;

namespace Colonists.Services
{
    public class ColonistRepository : MonoBehaviour
    {
        private List<ColonistFacade> _colonists = new();

        public event Action<ColonistFacade> Add;
        public event Action<ColonistFacade> Remove;

        private void Start()
        {
            _colonists = FindObjectsOfType<ColonistFacade>().ToList();
        }

        public IEnumerable<ColonistFacade> GetColonists()
        {
            foreach (var colonist in _colonists)
            {
                if (colonist.Alive)
                {
                    yield return colonist;
                }
            }
        }

        public void AddUnit(ColonistFacade colonist)
        {
            _colonists.Add(colonist);
            Add?.Invoke(colonist);
        }

        public void RemoveUnit(ColonistFacade colonist)
        {
            _colonists.Remove(colonist);
            Remove?.Invoke(colonist);
        }
    }
}
