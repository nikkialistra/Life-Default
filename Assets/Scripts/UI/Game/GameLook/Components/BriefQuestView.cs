using General.Questing;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class BriefQuestView
    {
        private readonly BriefQuestsView _parent;
        private readonly TemplateContainer _tree;

        private readonly VisualElement _root;

        private readonly Label _title;

        public BriefQuestView(BriefQuestsView parent, VisualTreeAsset asset)
        {
            _parent = parent;
            
            _tree = asset.CloneTree();

            _root = _tree.Q<VisualElement>("brief-quest");
            
            _title = _tree.Q<Label>("title");
        }

        public void Bind(Quest quest)
        {
            _title.text = quest.Title;

            _parent.BriefQuestList.Add(_root);
        }

        public void Unbind()
        {
            _parent.BriefQuestList.Remove(_root);
        }
    }
}
