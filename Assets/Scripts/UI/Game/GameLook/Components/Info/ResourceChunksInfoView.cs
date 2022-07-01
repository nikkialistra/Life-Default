using System.Collections.Generic;
using System.Linq;
using ResourceManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Info
{
    [RequireComponent(typeof(InfoPanelView))]
    public class ResourceChunksInfoView : MonoBehaviour
    {
        [Required]
        [SerializeField] private VisualTreeAsset _asset;

        private InfoPanelView _parent;
        private TemplateContainer _tree;

        private Label _name;

        private readonly List<VisualElement> _rows = new(1);
        private readonly List<Label> _rowNames = new(1);
        private readonly List<Label> _rowValues = new(1);

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

            _rowNames.Add(_tree.Q<Label>("row-one__name"));

            _rowValues.Add(_tree.Q<Label>("row-one__value"));
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

            _parent.InfoPanel.Remove(_tree);
            _shown = false;
        }

        public void FillIn(List<ResourceChunk> resourceChunks)
        {
            if (resourceChunks.Count == 0) return;

            _name.text = $"{resourceChunks[0].ResourceType.GetStringForResourceChunks()}";

            ShowRow();

            FillRow(0, $"{resourceChunks[0].ResourceType}:", $"~{CalculateQuantity(resourceChunks)}");
        }

        private int CalculateQuantity(List<ResourceChunk> resourceChunks)
        {
            return resourceChunks.Sum(resourceChunk => resourceChunk.Quantity);
        }

        private void ShowRow()
        {
            _rows[0].style.display = DisplayStyle.Flex;
            _rows[1].style.display = DisplayStyle.None;
        }

        private void FillRow(int index, string name, string value)
        {
            _rowNames[index].text = name;
            _rowValues[index].text = value;
        }
    }
}
