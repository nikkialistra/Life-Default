using System.Collections.Generic;
using General.Questing;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Stock
{
    public class QuestView
    {
        private readonly QuestsView _parent;
        private readonly TemplateContainer _tree;

        private readonly VisualElement _root;

        private readonly Label _title;
        private readonly Label _description;

        private readonly List<Label> _objectives = new();

        public QuestView(QuestsView parent, VisualTreeAsset asset)
        {
            _parent = parent;
            
            _tree = asset.CloneTree();

            _root = _tree.Q<VisualElement>("quest");
            
            _title = _tree.Q<Label>("title");
            _description = _tree.Q<Label>("description");

            _objectives.Add(_tree.Q<Label>("first-objective"));
            _objectives.Add(_tree.Q<Label>("second-objective"));
            _objectives.Add(_tree.Q<Label>("third-objective"));
        }

        public void Bind(Quest quest)
        {
            _title.text = quest.Title;
            _description.text = quest.Description;

            for (int i = 0; i < quest.Objectives.Count; i++)
            {
                _objectives[i].text = quest.Objectives[i].ToText();
            }
            
            _parent.QuestList.Add(_root);
        }

        public void Unbind()
        {
            _parent.QuestList.Remove(_root);
        }
    }
}
