using System.Collections.Generic;
using System.Linq;
using Enemies;
using UI.Game.GameLook.Components.Info;

namespace Selecting.Selected
{
    public class SelectedEnemies
    {
        private readonly InfoPanelView _infoPanelView;

        public SelectedEnemies(InfoPanelView infoPanelView)
        {
            _infoPanelView = infoPanelView;
        }

        public List<Enemy> Enemies { get; private set; } = new();
        public int Count => Enemies.Count;

        public void Set(List<Enemy> enemies)
        {
            UnsubscribeFromEnemies();

            Enemies = enemies.ToList();
            UpdateSelectionStatuses();

            _infoPanelView.SetEnemies(Enemies);

            SubscribeToEnemies();
        }

        public void Add(List<Enemy> enemies)
        {
            UnsubscribeFromEnemies();

            Enemies = Enemies.Union(enemies).ToList();
            UpdateSelectionStatuses();

            _infoPanelView.SetEnemies(Enemies);

            SubscribeToEnemies();
        }

        public void Set(Enemy enemy)
        {
            UnsubscribeFromEnemies();

            Enemies = new List<Enemy> { enemy };
            UpdateSelectionStatuses();
            _infoPanelView.SetEnemy(enemy);

            SubscribeToEnemies();
        }

        public void Add(Enemy enemy)
        {
            if (!Enemies.Contains(enemy))
            {
                Enemies.Add(enemy);
                enemy.EnemyDying += RemoveFromSelected;
            }
            else
            {
                enemy.Deselect();
                Enemies.Remove(enemy);
                enemy.EnemyDying -= RemoveFromSelected;
            }

            UpdateSelectionStatuses();
            _infoPanelView.SetEnemies(Enemies);
        }

        public void Deselect()
        {
            UnsubscribeFromEnemies();

            foreach (var enemy in Enemies)
                enemy.Deselect();

            Enemies.Clear();
        }

        public void Destroy()
        {
            UnsubscribeFromEnemies();

            foreach (var enemy in Enemies)
                enemy.Die();

            Enemies.Clear();
        }

        private void SubscribeToEnemies()
        {
            foreach (var enemy in Enemies)
                enemy.EnemyDying += RemoveFromSelected;
        }

        private void UnsubscribeFromEnemies()
        {
            foreach (var enemy in Enemies)
                enemy.EnemyDying -= RemoveFromSelected;
        }

        private void RemoveFromSelected(Enemy enemy)
        {
            Enemies.Remove(enemy);
        }

        private void UpdateSelectionStatuses()
        {
            foreach (var enemy in Enemies)
                enemy.Select();
        }
    }
}
