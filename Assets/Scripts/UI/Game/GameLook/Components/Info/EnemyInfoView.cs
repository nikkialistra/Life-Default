using System.Collections.Generic;
using Enemies;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Info
{
    [RequireComponent(typeof(InfoPanelView))]
    public class EnemyInfoView : MonoBehaviour
    {
        [Required]
        [SerializeField] private VisualTreeAsset _asset;

        private InfoPanelView _parent;
        private TemplateContainer _tree;
        
        private Label _name;

        private readonly List<VisualElement> _rows = new(2);
        private readonly List<Label> _rowNames = new(2);
        private readonly List<Label> _rowValues = new(2);
        
        private Enemy _enemy;

        private bool _shown;

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();

            _tree = _asset.CloneTree();
            _tree.pickingMode = PickingMode.Ignore;

            _name = _tree.Q<Label>("name");
            
            BindRows();
        }

        private void BindRows()
        {
            _rows.Add(_tree.Q<VisualElement>("row-one"));
            _rows.Add(_tree.Q<VisualElement>("row-two"));
            _rows.Add(_tree.Q<VisualElement>("row-three"));

            _rowNames.Add(_tree.Q<Label>("row-one__name"));
            _rowNames.Add(_tree.Q<Label>("row-two__name"));
            _rowNames.Add(_tree.Q<Label>("row-three__name"));

            _rowValues.Add(_tree.Q<Label>("row-one__value"));
            _rowValues.Add(_tree.Q<Label>("row-two__value"));
            _rowValues.Add(_tree.Q<Label>("row-three__value"));
        }

        private void OnDestroy()
        {
            UnsubscribeFromEnemy();
        }
        
        public void ShowSelf()
        {
            if (_shown)
            {
                return;
            }

            _parent.InfoPanel.Add(_tree);
            _shown = true;
        }

        public void HideSelf()
        {
            if (!_shown)
            {
                return;
            }
            
            UnsubscribeFromEnemy();
            
            _parent.InfoPanel.Remove(_tree);
            _shown = false;
        }
        
        public void FillIn(Enemy enemy)
        {
            UnsubscribeFromEnemy();
            
            _enemy = enemy;
            
            _name.text = $"{enemy.Name}";

            ShowRows();

            FillRow(0, "Health:", $"{_enemy.Unit.UnitVitality.Health}");
            FillRow(1, "Recovery Speed:", $"{_enemy.Unit.UnitVitality.RecoverySpeed}");
            FillRow(2, "Behavior:", $"{_enemy.FightManner.ToString()}");

            SubscribeToEnemy();
        }
        
        private void HidePanel()
        {
            _parent.UnsetEnemy(_enemy);
        }

        private void SubscribeToEnemy()
        {
            _enemy.HealthChange += UpdateRows;
            _enemy.Dying += HidePanel;
        }

        private void UnsubscribeFromEnemy()
        {
            if (_enemy != null)
            {
                _enemy.HealthChange -= UpdateRows;
                _enemy.Dying -= HidePanel;
            }
        }

        private void ShowRows()
        {
            _rows[0].style.display = DisplayStyle.Flex;
            _rows[1].style.display = DisplayStyle.Flex;
            _rows[2].style.display = DisplayStyle.Flex;
        }

        private void UpdateRows()
        {
            _rowValues[0].text = $"{_enemy.Unit.UnitVitality.Health}";
            _rowValues[0].text = $"{_enemy.Unit.UnitVitality.RecoverySpeed}";
        }

        private void FillRow(int index, string name, string value)
        {
            _rowNames[index].text = name;
            _rowValues[index].text = value;
        }
    }
}
