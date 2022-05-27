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
        [Required]
        [SerializeField] private VisualTreeAsset _asset;

        private const int NumberOfSkills = 13;

        private readonly SkillElement[] _skills = new SkillElement[NumberOfSkills];

        private ColonistInfoView _parent;

        private Colonist _colonist;

        private void Awake()
        {
            _parent = GetComponent<ColonistInfoView>();

            Tree = _asset.CloneTree();

            BindElements();
        }

        public bool Shown { get; private set; }
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
            if (Shown)
            {
                return;
            }

            _parent.TabContent.Add(Tree);
            Shown = true;
        }

        public void HideSelf()
        {
            if (!Shown)
            {
                return;
            }

            UnsubscribeFromChanges();
            
            _parent.TabContent.Remove(Tree);
            Shown = false;
        }

        private void SubscribeToChanges()
        {
            _colonist.SkillChange += FillSkill;
        }

        private void UnsubscribeFromChanges()
        {
            if (_colonist != null)
            {
                _colonist.SkillChange -= FillSkill;
                _colonist = null;
            }
        }

        private void FillSkills()
        {
            var traits = _colonist.Traits;
            
            for (int i = 0; i < NumberOfSkills; i++)
            {
                
            }
        }

        private void FillSkill(Skill skill)
        {
            var skillElement = _skills[(int)skill.SkillType];

            skillElement.Level.text = skill.Level.ToString();
            skillElement.ProgressBar.value = skill.Progress;
        }

        private void BindElements()
        {
            for (int i = 0; i < NumberOfSkills; i++)
            {
                var skillUxmlName = ((SkillType)i).GetUxmlName();

                _skills[i] = new SkillElement()
                {
                    Icon = Tree.Q<VisualElement>($"{skillUxmlName}__icon"),
                    Name = Tree.Q<Label>($"{skillUxmlName}__name"),

                    FavoriteIcon = Tree.Q<VisualElement>($"{skillUxmlName}__favorite-icon"),
                    ProgressBar = Tree.Q<ProgressBar>($"{skillUxmlName}__progress-bar"),
                    Level = Tree.Q<Label>($"{skillUxmlName}__level")
                };
            }
        }

        private struct SkillElement
        {
            public VisualElement Icon;
            public Label Name;
            
            public VisualElement FavoriteIcon;
            public ProgressBar ProgressBar;
            public Label Level;
        }
    }
}
