using System;
using System.Collections.Generic;
using Colonists;
using Colonists.Services;
using Colonists.Services.Selecting;
using General.Selection;
using General.Selection.Selected;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Zenject;
using static UI.Game.GameLook.Components.ColonistIconView;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(UIDocument))]
    public class ColonistIconsView : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset _asset;
        [Space]
        [SerializeField] private VisualTreeAsset _assetIcon;
        [SerializeField] private VisualTreeAsset _assetIconSmall;

        [SerializeField] private int _maxShownColonists = 20;
        [SerializeField] private int _iconSizeChangeNumber = 12;

        private UIDocument _uiDocument;

        private readonly Dictionary<Colonist, ColonistIconView> _colonistIconViews = new();

        private IconSize _currentIconSize = IconSize.Normal;

        private ColonistRepository _colonistRepository;
        private SelectedColonists _selectedColonists;
        private SelectionInput _selectionInput;
        private SelectionOperation _selectionOperation;

        [Inject]
        public void Construct(ColonistRepository colonistRepository, SelectedColonists selectedColonists,
            SelectionInput selectionInput, SelectionOperation selectionOperation)
        {
            _colonistRepository = colonistRepository;
            _selectedColonists = selectedColonists;
            _selectionInput = selectionInput;
            _selectionOperation = selectionOperation;
        }

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();

            Tree = _asset.CloneTree();

            ColonistIcons = Tree.Q<VisualElement>("colonist-icons");
        }
        
        public VisualElement Tree { get; private set; }
        public VisualElement ColonistIcons { get; private set; }
        
        public enum IconSize
        {
            Normal,
            Small
        }

        private void OnEnable()
        {
            _colonistRepository.Add += Add;
            _colonistRepository.Remove += Remove;

            _selectedColonists.SelectionChange += UpdateOutlines;
            
            _selectionInput.SelectingEnd += Select;
        }

        private void OnDisable()
        {
            _colonistRepository.Add -= Add;
            _colonistRepository.Remove -= Remove;

            _selectedColonists.SelectionChange -= UpdateOutlines;
            
            _selectionInput.SelectingEnd -= Select;
        }

        private void Add(Colonist colonist)
        {
            if (_colonistIconViews.Count >= _maxShownColonists)
            {
                return;
            }
            
            CreateColonistIconView(colonist);
            
            RecreateIconsOnIconChangeCondition();
        }

        private void RecreateIconsOnIconChangeCondition()
        {
            if (_colonistIconViews.Count > _iconSizeChangeNumber && _currentIconSize == IconSize.Normal)
            {
                _currentIconSize = IconSize.Small;
                ChangeIconSizes();
            }
            else if (_colonistIconViews.Count <= _iconSizeChangeNumber && _currentIconSize == IconSize.Small)
            {
                _currentIconSize = IconSize.Normal;
                ChangeIconSizes();
            }
        }

        private void CreateColonistIconView(Colonist colonist)
        {
            var iconAsset = _currentIconSize == IconSize.Normal ? _assetIcon : _assetIconSmall;
            
            var colonistIconView = new ColonistIconView(this, iconAsset);
            colonistIconView.Bind(colonist);
            colonistIconView.Click += OnColonistClick;

            _colonistIconViews.Add(colonist, colonistIconView);
        }

        private void ChangeIconSizes()
        {
            foreach (var colonistIconView in _colonistIconViews.Values)
            {
                colonistIconView.Unbind();
            }
            
            _colonistIconViews.Clear();

            foreach (var colonist in _colonistRepository.GetColonists())
            {
                CreateColonistIconView(colonist);
            }
        }

        private void Remove(Colonist colonist)
        {
            if (!_colonistIconViews.ContainsKey(colonist))
            {
                throw new ArgumentException("Colonist icons view don't have this colonist");
            }
            
            var colonistIconView = _colonistIconViews[colonist];
            colonistIconView.Unbind();
            colonistIconView.Click -= OnColonistClick;

            _colonistIconViews.Remove(colonist);
            
            RecreateIconsOnIconChangeCondition();
        }

        private void Select(Rect rect)
        {
            var transformedRect = TransformRect(rect);
            var colonists = new List<Colonist>();
            
            foreach (var (colonist, colonistIconView) in _colonistIconViews)
            {
                if (transformedRect.Contains(colonistIconView.Center))
                {
                    colonists.Add(colonist);
                }
            }

            if (colonists.Count != 0)
            {
                _selectedColonists.Set(colonists);
                _selectionOperation.CancelSelection();
            }
        }

        private Rect TransformRect(Rect rect)
        {
            var referenceResolution = (Vector2)_uiDocument.panelSettings.referenceResolution;
            var xScale = Screen.width / referenceResolution.x;
            var yScale = Screen.height / referenceResolution.y;

            var xMin = rect.xMin / xScale;
            var xMax = rect.xMax / xScale;

            var yMin = (Screen.height - rect.yMax) / yScale;
            var yMax = (Screen.height - rect.yMin) / yScale;

            var transformedRect = new Rect
            {
                xMin = xMin,
                xMax = xMax,
                yMin = yMin,
                yMax = yMax
            };

            return transformedRect;
        }

        private void OnColonistClick(Colonist colonist)
        {
            if (Keyboard.current.shiftKey.isPressed)
            {
                _selectedColonists.Add(colonist);
            }
            else
            {
                _selectedColonists.Set(colonist);
            }
            
            UpdateOutlines();
        }

        private void UpdateOutlines()
        {
            foreach (var (colonist, colonistIconView) in _colonistIconViews)
            {
                if (_selectedColonists.Contains(colonist))
                {
                    colonistIconView.ShowOutline();
                }
                else
                {
                    colonistIconView.HideOutline();
                }
            }
        }
    }
}
