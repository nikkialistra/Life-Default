using System;
using System.Collections.Generic;
using Colonists.Colonist;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(InfoPanelView))]
    public class ColonistsInfoView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/ColonistsInfo";
        
        [Required]
        [SerializeField] private Texture2D _multipleUnitsPreview;

        private int _count;

        private InfoPanelView _parent;
        private TemplateContainer _tree;

        private VisualElement _image;
        private Label _colonistCount;

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();

            _tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            _image = _tree.Q<VisualElement>("image");
            _colonistCount = _tree.Q<Label>("description__nomination__count");
            IconContainer = _tree.Q<VisualElement>("description__icon-container");

            InitializeUnitIconViews();
        }

        public event Action<ColonistFacade> SelectColonist;

        public VisualElement IconContainer { get; private set; }

        private void InitializeUnitIconViews()
        {
        }

        private void OnUnitIconRemove()
        {
            _count--;
            UpdateCountText();
        }

        private void OnUnitIconClick(ColonistFacade colonist)
        {
            SelectColonist?.Invoke(colonist);
        }

        public void ShowSelf()
        {
            _parent.InfoPanel.Add(_tree);
        }

        public void HideSelf()
        {
            if (_parent.InfoPanel.Contains(_tree))
            {
                _parent.InfoPanel.Remove(_tree);
            }
        }

        private void HidePanel()
        {
            _parent.HideSelf();
        }

        public void FillIn(List<ColonistFacade> colonists)
        {
            FillInPreview();
            FillInProperties(colonists);
        }

        private void FillInPreview()
        {
            _image.style.backgroundImage = new StyleBackground(_multipleUnitsPreview);
        }

        private void FillInProperties(List<ColonistFacade> units)
        {
            _count = units.Count;
            UpdateCountText();
        }

        private void UpdateCountText()
        {
            if (_count == 0)
            {
                HidePanel();
            }

            _colonistCount.text = $"Colonists ({_count})";
        }
    }
}
