using System.Collections.Generic;
using UI.Game.GameLook.Components;
using UI.Game.GameLook.Components.Stock;
using UnityEngine;
using Zenject;

namespace General.Questing
{
    public class QuestSystem : MonoBehaviour
    {
        [SerializeField] private List<Quest> _activeQuests;
        
        private QuestsView _questsView;
        private BriefQuestsView _briefQuestsView;

        [Inject]
        public void Construct(QuestsView questsView, BriefQuestsView briefQuestsView)
        {
            _questsView = questsView;
            _briefQuestsView = briefQuestsView;
        }
        
        private void Start()
        {
            foreach (var quest in _activeQuests)
            {
                _questsView.AddQuest(quest);
                _briefQuestsView.AddQuest(quest);
            }
        }
    }
}
