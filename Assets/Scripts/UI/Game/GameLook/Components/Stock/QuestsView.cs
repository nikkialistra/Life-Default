using System;
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

        private readonly Dictionary<Quest, QuestView> _questViews = new();

        private VisualElement _activeQuests;
        private VisualElement _finishedQuests;

        private StockView _parent;

        private void Awake()
        {
            _parent = GetComponent<StockView>();

            Tree = _asset.CloneTree();

            _activeQuests = Tree.Q<VisualElement>("active-quests");
            _finishedQuests = Tree.Q<VisualElement>("finished-quests");
        }
        
        public bool Shown { get; private set; }
        private VisualElement Tree { get; set; }

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
            var questView = new QuestView(_questAsset);
            questView.Bind(quest);
            
            _activeQuests.Add(questView.Tree);

            quest.Complete += MoveToFinished;

            _questViews.Add(quest, questView);
        }

        private void MoveToFinished(Quest quest)
        {
            _questViews[quest].Unbind();

            if (_activeQuests.Contains(_questViews[quest].Tree))
            {
                _activeQuests.Remove(_questViews[quest].Tree);
            }
            
            _finishedQuests.Add(_questViews[quest].Tree);
        }
    }
}
