using System.Collections.Generic;
using General;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components.Stock;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class NotificationsView : MonoBehaviour
    {
        [Required]
        [SerializeField] private VisualTreeAsset _asset;
        [Space]
        [Required]
        [SerializeField] private VisualTreeAsset _notificationAsset;

        private readonly Dictionary<Notification, NotificationView> _notificationViews = new();

        private VisualElement _notifications;

        private StockView _parent;

        private void Awake()
        {
            Tree = _asset.CloneTree();

            _notifications = Tree.Q<VisualElement>("notifications");
        }
        
        public VisualElement Tree { get; private set; }

        public void AddNotification(Notification notification)
        {
            var notificationView = new NotificationView(this, _notificationAsset);
            notificationView.Bind(notification);
            _notifications.Add(notificationView.Tree);

            _notificationViews.Add(notification, notificationView);
        }

        public void RemoveNotification(Notification notification)
        {
            if (_notificationViews.ContainsKey(notification))
            {
                _notificationViews[notification].Unbind();
                _notifications.Remove(_notificationViews[notification].Tree);

                _notificationViews.Remove(notification);
            }
        }
    }
}
