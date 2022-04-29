using System.Collections.Generic;
using General;
using General.Questing;
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

        private readonly List<NotificationView> _notificationViews = new();

        private VisualElement _notificationList;

        private StockView _parent;

        private void Awake()
        {
            Tree = _asset.CloneTree();

            _notificationList = Tree.Q<VisualElement>("notification-list");
        }
        
        public bool Shown { get; private set; }
        public VisualElement Tree { get; set; }

        public VisualElement NotificationList => _notificationList;

        public void AddNotification(Notification notification)
        {
            var notificationView = new NotificationView(this, _notificationAsset);
            notificationView.Bind(notification);

            _notificationViews.Add(notificationView);
        }
    }
}
