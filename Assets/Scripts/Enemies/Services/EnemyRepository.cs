using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies.Services
{
    public class EnemyRepository : MonoBehaviour
    {
        private readonly List<Enemy> _enemies = new();

        public event Action<Enemy> Add;
        public event Action<Enemy> Remove;

        public int Count => _enemies.Count;

        public IEnumerable<Enemy> GetEnemies()
        {
            foreach (var enemy in _enemies)
            {
                if (enemy.Alive)
                {
                    yield return enemy;
                }
            }
        }

        public void AddEnemy(Enemy colonist)
        {
            _enemies.Add(colonist);
            Add?.Invoke(colonist);
        }

        public void RemoveEnemy(Enemy enemy)
        {
            _enemies.Remove(enemy);
            Remove?.Invoke(enemy);
        }
    }
}
