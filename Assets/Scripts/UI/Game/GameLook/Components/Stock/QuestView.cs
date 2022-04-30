using System;
using System.Collections.Generic;
using General.Questing;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Stock
{
    public class QuestView
    {
        private readonly VisualElement _root;

        private readonly Label _title;
        private readonly Label _description;

        private Quest _quest;
        
        private readonly List<Label> _objectives = new();

        private readonly List<Action<string>> _updateObjectiveActions = new();
        private readonly List<Action<string>> _completeObjectiveActions = new();

        public QuestView(VisualTreeAsset asset)
        {
            var tree = asset.CloneTree();

            _root = tree.Q<VisualElement>("quest");
            
            _title = tree.Q<Label>("title");
            _description = tree.Q<Label>("description");

            _objectives.Add(tree.Q<Label>("first-objective"));
            _objectives.Add(tree.Q<Label>("second-objective"));
            _objectives.Add(tree.Q<Label>("third-objective"));

            AddUpdateObjectiveActions();
            AddCompleteObjectiveActions();
        }

        public VisualElement Tree => _root;

        private void AddUpdateObjectiveActions()
        {
            for (int i = 0; i < 3; i++)
            {
                _updateObjectiveActions.Add(CreateAction(i));
            }

            Action<string> CreateAction(int index)
            {
                return text => _objectives[index].text = text;
            }
        }
        
        private void AddCompleteObjectiveActions()
        {
            for (int i = 0; i < 3; i++)
            {
                _completeObjectiveActions.Add(CreateAction(i));
            }

            Action<string> CreateAction(int index)
            {
                return text => _objectives[index].text = text;
            }
        }

        public void Bind(Quest quest)
        {
            _quest = quest;
            
            _title.text = _quest.Title;
            _description.text = _quest.Description;

            BindObjectives();
        }

        private void BindObjectives()
        {
            for (int i = 0; i < 3; i++)
            {
                if (_quest.HasObjectiveAt(i))
                {
                    _objectives[i].style.display = DisplayStyle.Flex;
                    _objectives[i].text = _quest.Objectives[i].ToText();

                    _quest.Objectives[i].Update += _updateObjectiveActions[i];
                    _quest.Objectives[i].Complete += _completeObjectiveActions[i];
                }
            }
        }
        
        public void Unbind()
        {
            UnbindObjectives();
        }

        private void UnbindObjectives()
        {
            for (int i = 0; i < 3; i++)
            {
                if (_quest.HasObjectiveAt(i))
                {
                    _quest.Objectives[i].Update -= _updateObjectiveActions[i];
                    _quest.Objectives[i].Complete -= _completeObjectiveActions[i];
                }
            }
        }
    }
}
