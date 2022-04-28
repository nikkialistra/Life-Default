using System.Collections.Generic;
using General.Questing;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components.Stock;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class BriefQuestsView : MonoBehaviour
    {
        [Required]
        [SerializeField] private VisualTreeAsset _asset;
        [Space]
        [Required]
        [SerializeField] private VisualTreeAsset _briefQuestAsset;

        private List<BriefQuestView> _briefQuestViews = new();

        private VisualElement _briefQuestList;

        private StockView _parent;

        private void Awake()
        {
            Tree = _asset.CloneTree();

            _briefQuestList = Tree.Q<VisualElement>("brief-quest-list");
        }
        
        public bool Shown { get; private set; }
        public VisualElement Tree { get; set; }

        public VisualElement BriefQuestList => _briefQuestList;

        public void AddQuest(Quest quest)
        {
            var questView = new BriefQuestView(this, _briefQuestAsset);
            questView.Bind(quest);

            _briefQuestViews.Add(questView);
        }
    }
}
