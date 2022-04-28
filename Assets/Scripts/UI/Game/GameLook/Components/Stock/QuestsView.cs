using System.Collections.Generic;
using General.Questing;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Stock
{
    [RequireComponent(typeof(StockView))]
    public class QuestsView : MonoBehaviour
    {
        [Required]
        [SerializeField] private VisualTreeAsset _asset;
        [Space]
        [SerializeField] private VisualTreeAsset _questAsset;

        private List<QuestView> _questViews = new();

        private VisualElement _questList;

        private StockView _parent;

        private void Awake()
        {
            _parent = GetComponent<StockView>();

            Tree = _asset.CloneTree();

            _questList = Tree.Q<VisualElement>("quest-list");
        }
        
        public bool Shown { get; private set; }
        private VisualElement Tree { get; set; }

        public VisualElement QuestList => _questList;

        public void ShowSelf()
        {
            if (Shown)
            {
                return;
            }

            _parent.Content.Add(Tree);
            Shown = true;
        }

        public void HideSelf()
        {
            if (!Shown)
            {
                return;
            }
            
            _parent.Content.Remove(Tree);
            Shown = false;
        }

        public void AddQuest(Quest quest)
        {
            var questView = new QuestView(this, _questAsset);
            questView.Bind(quest);

            _questViews.Add(questView);
        }
    }
}
