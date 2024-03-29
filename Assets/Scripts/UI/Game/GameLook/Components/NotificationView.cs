﻿using Questing;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class NotificationView
    {
        public VisualElement Tree { get; }

        private readonly NotificationsView _parent;

        private readonly Button _title;

        private Notification _notification;

        public NotificationView(NotificationsView parent, VisualTreeAsset asset)
        {
            _parent = parent;

            var tree = asset.CloneTree();

            Tree = tree.Q<VisualElement>("notification");

            _title = tree.Q<Button>("title");
        }

        public void Bind(Notification notification)
        {
            _notification = notification;

            _title.text = _notification.Title;

            _title.clicked += _notification.Click;
        }

        public void Unbind()
        {
            _title.clicked -= _notification.Click;
        }
    }
}
