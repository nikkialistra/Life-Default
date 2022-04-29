using General;
using General.Questing;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class NotificationView
    {
        private readonly NotificationsView _parent;
        private readonly TemplateContainer _tree;

        private readonly VisualElement _root;

        private readonly Button _title;

        public NotificationView(NotificationsView parent, VisualTreeAsset asset)
        {
            _parent = parent;
            
            _tree = asset.CloneTree();

            _root = _tree.Q<VisualElement>("notification");
            
            _title = _tree.Q<Button>("title");
        }

        public void Bind(Notification notification)
        {
            _title.text = notification.Title;

            _parent.NotificationList.Add(_root);
        }

        public void Unbind()
        {
            _parent.NotificationList.Remove(_root);
        }
    }
}
