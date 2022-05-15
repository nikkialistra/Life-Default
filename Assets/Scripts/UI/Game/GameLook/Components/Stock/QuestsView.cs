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
        [Space]
        [SerializeField] private float _scrollSpeed = 100f;
        [SerializeField] private float _scrollDamping = 10f;

        private readonly Dictionary<Quest, QuestView> _questViews = new();

        private ScrollView _scrollView;
        private Scroller _scroller;

        private VisualElement _activeQuests;
        private VisualElement _finishedQuests;

        private Label _noQuests;
        
        private Label _active;
        private Label _finished;

        private StockView _parent;

        private float _scrollVelocity;

        private void Awake()
        {
            _parent = GetComponent<StockView>();

            Tree = _asset.CloneTree();

            _scrollView = Tree.Q<ScrollView>("scroll-view");
            _scroller = _scrollView.Q("unity-content-and-vertical-scroll-container").Q<Scroller>();

            _activeQuests = Tree.Q<VisualElement>("active-quests");
            _finishedQuests = Tree.Q<VisualElement>("finished-quests");
            
            _noQuests = Tree.Q<Label>("no-quests");
            
            _active = Tree.Q<Label>("active");
            _finished = Tree.Q<Label>("finished");
        }
        
        public bool Shown { get; private set; }
        private VisualElement Tree { get; set; }

        private void Start()
        {
            UpdateLabelShowing();
        }

        private void OnEnable()
        {
            _scrollView.RegisterCallback<WheelEvent>(OnScroll);
        }

        private void OnDisable()
        {
            _scrollView.UnregisterCallback<WheelEvent>(OnScroll);
        }

        private void Update()
        {
            if (Shown)
            {
                _scroller.value += _scrollVelocity;
                _scrollVelocity -= _scrollVelocity * _scrollDamping;
            }
        }

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

            UpdateLabelShowing();
        }

        private void OnScroll(WheelEvent @event)
        {
            _scrollVelocity += @event.delta.y * _scrollSpeed;

            @event.StopPropagation();
        }

        private void MoveToFinished(Quest quest)
        {
            _questViews[quest].Unbind();

            if (_activeQuests.Contains(_questViews[quest].Tree))
            {
                _activeQuests.Remove(_questViews[quest].Tree);
            }
            
            _finishedQuests.Add(_questViews[quest].Tree);
            
            UpdateLabelShowing();
        }

        private void UpdateLabelShowing()
        {
            _noQuests.style.display = _activeQuests.childCount + _finishedQuests.childCount == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            
            _active.style.display = _activeQuests.childCount > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            _finished.style.display = _finishedQuests.childCount > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
