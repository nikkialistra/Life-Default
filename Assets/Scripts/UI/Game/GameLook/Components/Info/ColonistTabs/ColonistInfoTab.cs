using System;
using Colonists;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components.Info.ColonistInfo;
using Units.Traits;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Info.ColonistTabs
{
    [RequireComponent(typeof(ColonistInfoView))]
    public class ColonistInfoTab : MonoBehaviour
    {
        private const int MaxTraits = 4;


        public bool Shown { get; private set; }

        [Required]
        [SerializeField] private VisualTreeAsset _asset;

        private readonly TraitElement[] _traitElements = new TraitElement[MaxTraits];

        private ColonistInfoView _parent;

        private Colonist _colonist;

        private void Awake()
        {
            _parent = GetComponent<ColonistInfoView>();

            Tree = _asset.CloneTree();

            BindElements();
            HideAll();
        }

        private VisualElement Tree { get; set; }

        public void FillIn(Colonist colonist)
        {
            UnsubscribeFromChanges();

            _colonist = colonist;

            if (colonist.Traits.Count > MaxTraits)
                throw new ArgumentException($"Info tab can show only {MaxTraits} traits");

            FillTraits();

            SubscribeToChanges();
        }

        public void ShowSelf()
        {
            if (Shown) return;

            _parent.TabContent.Add(Tree);
            Shown = true;
        }

        public void HideSelf()
        {
            if (!Shown) return;

            UnsubscribeFromChanges();

            _parent.TabContent.Remove(Tree);
            Shown = false;
        }

        private void SubscribeToChanges()
        {
            _colonist.TraitsChange += FillTraits;
        }

        private void UnsubscribeFromChanges()
        {
            if (_colonist != null)
            {
                _colonist.TraitsChange -= FillTraits;
                _colonist = null;
            }
        }

        private void FillTraits()
        {
            var traits = _colonist.Traits;

            for (int i = 0; i < MaxTraits; i++)
            {
                if (traits.Count > i)
                    FillTrait(i, traits[i]);
                else
                    HideTrait(i);
            }
        }

        private void FillTrait(int index, Trait trait)
        {
            _traitElements[index].Root.style.display = DisplayStyle.Flex;

            _traitElements[index].Icon.style.backgroundImage = new StyleBackground(trait.Icon);
            _traitElements[index].Name.text = trait.Name;
        }

        private void HideTrait(int index)
        {
            _traitElements[index].Root.style.display = DisplayStyle.None;
        }

        private void BindElements()
        {
            _traitElements[0] = new TraitElement
            {
                Root = Tree.Q<VisualElement>("first-trait"),
                Icon = Tree.Q<VisualElement>("first-trait__icon"),
                Name = Tree.Q<Label>("first-trait__name")
            };

            _traitElements[1] = new TraitElement
            {
                Root = Tree.Q<VisualElement>("second-trait"),
                Icon = Tree.Q<VisualElement>("second-trait__icon"),
                Name = Tree.Q<Label>("second-trait__name")
            };

            _traitElements[2] = new TraitElement
            {
                Root = Tree.Q<VisualElement>("third-trait"),
                Icon = Tree.Q<VisualElement>("third-trait__icon"),
                Name = Tree.Q<Label>("third-trait__name")
            };

            _traitElements[3] = new TraitElement
            {
                Root = Tree.Q<VisualElement>("forth-trait"),
                Icon = Tree.Q<VisualElement>("forth-trait__icon"),
                Name = Tree.Q<Label>("forth-trait__name")
            };
        }

        private void HideAll()
        {
            for (int i = 0; i < MaxTraits; i++)
                HideTrait(i);
        }

        private struct TraitElement
        {
            public VisualElement Root;

            public VisualElement Icon;
            public Label Name;
        }
    }
}
