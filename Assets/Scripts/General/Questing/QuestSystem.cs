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
        private NotificationsView _notificationsView;

        [Inject]
        public void Construct(QuestsView questsView, NotificationsView notificationsView)
        {
            _questsView = questsView;
            _notificationsView = notificationsView;
        }
        
        private void Start()
        {
            foreach (var quest in _activeQuests)
            {
                _questsView.AddQuest(quest);

                var notification = new Notification(quest.Title);
                
                _notificationsView.AddNotification(notification);
            }
        }
    }
}
