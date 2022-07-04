using System;
using Colonists;
using Colonists.Skills;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components.Info.ColonistInfo;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Info.ColonistTabs
{
    [RequireComponent(typeof(ColonistInfoView))]
    public class ColonistSkillsTab : MonoBehaviour
    {
        private const int NumberOfSkills = 13;

        public bool Shown { get; private set; }

        [Required]
        [SerializeField] private VisualTreeAsset _asset;

        [Space]
        [SerializeField] private Sprite _oneStar;
        [SerializeField] private Sprite _twoStars;

        private readonly SkillElement[] _skills = new SkillElement[NumberOfSkills];

        private ColonistInfoView _parent;

        private Colonist _colonist;

        private void Awake()
        {
            _parent = GetComponent<ColonistInfoView>();

            Tree = _asset.CloneTree();

            BindElements();
        }

        private VisualElement Tree { get; set; }

        public void FillIn(Colonist colonist)
        {
            UnsubscribeFromChanges();

            _colonist = colonist;

            FillSkills();

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
            _colonist.SkillChange += UpdateSkill;
        }

        private void UnsubscribeFromChanges()
        {
            if (_colonist != null)
            {
                _colonist.SkillChange -= UpdateSkill;
                _colonist = null;
            }
        }

        private void FillSkills()
        {
            foreach (var skill in _colonist.Skills)
                FillSkill(skill);
        }

        private void FillSkill(Skill skill)
        {
            var skillElement = _skills[(int)skill.SkillType];

            FillFavoriteIcon(skillElement.FavoriteIcon, skill.FavoriteLevel);

            if (skill.CanDo)
                skillElement.Root.RemoveFromClassList("disabled");
            else
                skillElement.Root.AddToClassList("disabled");

            UpdateSkill(skill);
        }

        private void UpdateSkill(Skill skill)
        {
            var skillElement = _skills[(int)skill.SkillType];

            skillElement.Level.text = skill.Level.ToString();
            skillElement.ProgressBar.value = skill.Progress;
        }

        private void FillFavoriteIcon(VisualElement favoriteIcon, FavoriteLevel favoriteLevel)
        {
            favoriteIcon.style.backgroundImage = new StyleBackground(favoriteLevel switch
            {
                FavoriteLevel.None => null,
                FavoriteLevel.OneStar => _oneStar,
                FavoriteLevel.TwoStars => _twoStars,
                _ => throw new ArgumentOutOfRangeException()
            });
        }

        private void BindElements()
        {
            for (int i = 0; i < NumberOfSkills; i++)
            {
                var skillUxmlName = ((SkillType)i).GetUxmlName();

                _skills[i] = new SkillElement
                {
                    Root = Tree.Q<VisualElement>($"{skillUxmlName}"),
                    FavoriteIcon = Tree.Q<VisualElement>($"{skillUxmlName}__favorite-icon"),
                    ProgressBar = Tree.Q<ProgressBar>($"{skillUxmlName}__progress-bar"),
                    Level = Tree.Q<Label>($"{skillUxmlName}__level")
                };
            }
        }

        private struct SkillElement
        {
            public VisualElement Root;

            public VisualElement FavoriteIcon;
            public ProgressBar ProgressBar;
            public Label Level;
        }
    }
}
