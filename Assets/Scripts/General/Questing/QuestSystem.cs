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
        private StockView _stockView;
        private QuestServices _questServices;

        [Inject]
        public void Construct(QuestServices questServices,QuestsView questsView, NotificationsView notificationsView, StockView stockView)
        {
            _questServices = questServices;
            
            _questsView = questsView;
            _notificationsView = notificationsView;
            _stockView = stockView;
        }
        
        private void Start()
        {
            foreach (var quest in _activeQuests)
            {
                quest.Activate(_questServices);
                _questsView.AddQuest(quest);

                var notification = new Notification(quest.Title, _stockView.ToggleQuests);
                
                _notificationsView.AddNotification(notification);
            }
        }
    }
}
