using System.Collections.Generic;
using Aborigines;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Info
{
    [RequireComponent(typeof(InfoPanelView))]
    public class AborigineInfoView : MonoBehaviour
    {
        [Required]
        [SerializeField] private VisualTreeAsset _asset;

        private InfoPanelView _parent;
        private TemplateContainer _tree;

        private Label _name;

        private readonly List<VisualElement> _rows = new(2);
        private readonly List<Label> _rowNames = new(2);
        private readonly List<Label> _rowValues = new(2);

        private Aborigine _aborigine;

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
            UnsubscribeFromAborigine();
        }

        public void ShowSelf()
        {
            if (_shown) return;

            _parent.InfoPanel.Add(_tree);
            _shown = true;
        }

        public void HideSelf()
        {
            if (!_shown) return;

            UnsubscribeFromAborigine();

            _parent.InfoPanel.Remove(_tree);
            _shown = false;
        }

        public void FillIn(Aborigine aborigine)
        {
            UnsubscribeFromAborigine();

            _aborigine = aborigine;

            _name.text = $"{aborigine.Name}";

            ShowRows();

            FillRow(0, "Health:", $"{_aborigine.Unit.UnitVitality.Health}");
            FillRow(1, "Recovery Speed:", $"{_aborigine.Unit.UnitVitality.RecoverySpeed}");
            FillRow(2, "Behavior:", $"{_aborigine.FightManner.ToString()}");

            SubscribeToAborigine();
        }

        private void HidePanel()
        {
            _parent.UnsetAborigine(_aborigine);
        }

        private void SubscribeToAborigine()
        {
            _aborigine.HealthChange += UpdateRows;
            _aborigine.Dying += HidePanel;
        }

        private void UnsubscribeFromAborigine()
        {
            if (_aborigine != null)
            {
                _aborigine.HealthChange -= UpdateRows;
                _aborigine.Dying -= HidePanel;
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
            _rowValues[0].text = $"{_aborigine.Unit.UnitVitality.Health}";
            _rowValues[0].text = $"{_aborigine.Unit.UnitVitality.RecoverySpeed}";
        }

        private void FillRow(int index, string name, string value)
        {
            _rowNames[index].text = name;
            _rowValues[index].text = value;
        }
    }
}
