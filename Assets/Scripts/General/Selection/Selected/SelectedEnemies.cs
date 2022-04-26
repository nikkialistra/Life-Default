using System.Collections.Generic;
using Enemies;
using UI.Game.GameLook.Components;

namespace General.Selection.Selected
{
    public class SelectedEnemies
    {
        private readonly InfoPanelView _infoPanelView;

        public SelectedEnemies(InfoPanelView infoPanelView)
        {
            _infoPanelView = infoPanelView;
        }

        public List<Enemy> Enemies { get; private set; } = new();

        public void Set(List<Enemy> enemies)
        {
            UnsubscribeFromEnemies();

            Enemies = enemies;
            UpdateSelectionStatuses();
            
            _infoPanelView.SetEnemies(Enemies);

            SubscribeToColonists();
        }

        public void Set(Enemy enemy)
        {
            UnsubscribeFromEnemies();

            Enemies = new List<Enemy> { enemy };
            UpdateSelectionStatuses();
            _infoPanelView.SetEnemy(enemy);

            SubscribeToColonists();
        }

        public void Add(Enemy enemy)
        {
            Enemies.Add(enemy);
            UpdateSelectionStatuses();
            _infoPanelView.SetEnemies(Enemies);
            
            enemy.EnemyDie += RemoveFromSelected;
        }

        public void Deselect()
        {
            UnsubscribeFromEnemies();

            foreach (var enemy in Enemies)
            {
                enemy.Deselect();
            }

            Enemies.Clear();
        }
        
        public void Destroy()
        {
            UnsubscribeFromEnemies();

            foreach (var enemy in Enemies)
            {
                enemy.Die();
            }

            Enemies.Clear();
        }

        private void SubscribeToColonists()
        {
            foreach (var enemy in Enemies)
            {
                enemy.EnemyDie += RemoveFromSelected;
            }
        }

        private void UnsubscribeFromEnemies()
        {
            foreach (var enemy in Enemies)
            {
                enemy.EnemyDie -= RemoveFromSelected;
            }
        }

        private void RemoveFromSelected(Enemy enemy)
        {
            Enemies.Remove(enemy);
        }

        private void UpdateSelectionStatuses()
        {
            foreach (var enemy in Enemies)
            {
                enemy.Select();
            }
        }
    }
}
