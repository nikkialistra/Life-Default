using System;
using System.Collections.Generic;
using ColonistManagement.Selection;
using Colonists.Colonist;
using Colonists.Services;
using Colonists.Services.Selecting;
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
        private const string VisualTreePath = "UI/Markup/GameLook/Components/ColonistIcons";

        [SerializeField] private int _maxShownColonists = 20;
        [SerializeField] private int _iconSizeChangeNumber = 12;

        private UIDocument _uiDocument;

        private readonly Dictionary<ColonistFacade, ColonistIconView> _colonistIconViews = new();

        private IconSize _currentIconSize = IconSize.Normal;

        private ColonistRepository _colonistRepository;
        private SelectedColonists _selectedColonists;
        private SelectionInput _selectionInput;
        private ColonistSelection _colonistSelection;

        [Inject]
        public void Construct(ColonistRepository colonistRepository, SelectedColonists selectedColonists,
            SelectionInput selectionInput, ColonistSelection colonistSelection)
        {
            _colonistRepository = colonistRepository;
            _selectedColonists = selectedColonists;
            _selectionInput = selectionInput;
            _colonistSelection = colonistSelection;
        }

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();

            Tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            ColonistIcons = Tree.Q<VisualElement>("colonist-icons");
        }
        
        public VisualElement Tree { get; private set; }
        public VisualElement ColonistIcons { get; private set; }

        private void OnEnable()
        {
            _colonistRepository.Add += Add;
            _colonistRepository.Remove += Remove;

            _selectedColonists.SelectionChange += UpdateOutlines;
            
            _selectionInput.SelectingEnd += SelectColonists;
        }

        private void OnDisable()
        {
            _colonistRepository.Add -= Add;
            _colonistRepository.Remove -= Remove;

            _selectedColonists.SelectionChange -= UpdateOutlines;
            
            _selectionInput.SelectingEnd -= SelectColonists;
        }

        private void Add(ColonistFacade colonist)
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

        private void CreateColonistIconView(ColonistFacade colonist)
        {
            var colonistIconView = new ColonistIconView(this, _currentIconSize);
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

        private void Remove(ColonistFacade colonist)
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

        private void SelectColonists(Rect rect)
        {
            var transformedRect = TransformRect(rect);
            var colonists = new List<ColonistFacade>();
            
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
                _colonistSelection.CancelSelection();
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

        private void OnColonistClick(ColonistFacade colonist)
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
