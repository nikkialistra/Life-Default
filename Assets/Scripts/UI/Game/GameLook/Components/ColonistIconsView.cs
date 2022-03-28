using System;
using System.Collections.Generic;
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
    public class ColonistIconsView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/ColonistIcons";

        [SerializeField] private int _maxShownColonists = 20;
        [SerializeField] private int _iconSizeChangeNumber = 12;
        
        private readonly Dictionary<ColonistFacade, ColonistIconView> _colonistIconViews = new();

        private IconSize _currentIconSize = IconSize.Normal;

        private ColonistRepository _colonistRepository;
        private SelectedColonists _selectedColonists;

        [Inject]
        public void Construct(ColonistRepository colonistRepository, SelectedColonists selectedColonists)
        {
            _colonistRepository = colonistRepository;
            _selectedColonists = selectedColonists;
        }

        private void Awake()
        {
            Tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            ColonistIcons = Tree.Q<VisualElement>("colonist-icons");
        }
        
        public VisualElement Tree { get; private set; }
        public VisualElement ColonistIcons { get; private set; }

        private void OnEnable()
        {
            _colonistRepository.Add += Add;
            _colonistRepository.Remove += Remove;

            _selectedColonists.SelectionChange += OnSelectionChange;
        }

        private void OnDisable()
        {
            _colonistRepository.Add -= Add;
            _colonistRepository.Remove -= Remove;

            _selectedColonists.SelectionChange -= OnSelectionChange;
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

        private void OnSelectionChange(List<ColonistFacade> selectedColonists)
        {
            UpdateOutlines(selectedColonists);
        }

        private void OnColonistClick(ColonistFacade colonist)
        {
            if (Keyboard.current.shiftKey.isPressed)
            {
                _selectedColonists.Add(colonist);
                AddToOutlines(colonist);
            }
            else
            {
                _selectedColonists.Set(colonist);
                UpdateOutlines(new List<ColonistFacade>() { colonist });
            }
        }

        private void UpdateOutlines(List<ColonistFacade> selectedColonists)
        {
            foreach (var (colonist, colonistIconView) in _colonistIconViews)
            {
                if (selectedColonists.Contains(colonist))
                {
                    colonistIconView.ShowOutline();
                }
                else
                {
                    colonistIconView.HideOutline();
                }
            }
        }

        private void AddToOutlines(ColonistFacade selectedColonist)
        {
            foreach (var (colonist, colonistIconView) in _colonistIconViews)
            {
                if (selectedColonist == colonist)
                {
                    colonistIconView.ShowOutline();
                }
            }
        }
    }
}
