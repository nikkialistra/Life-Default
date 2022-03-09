using ColonistManagement.Movement;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Game.GameLook.Components
{
    public class CommandsView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/Commands";

        private Button _move;
        private Button _stop;
        private Button _attack;
        private Button _hold;
        private Button _patrol;
        
        private bool _shown;
        
        private InfoPanelView _parent;
        
        private TemplateContainer _tree;

        private MovementCommand _movementCommand;
        private MovementActionInput _movementActionInput;

        [Inject]
        public void Construct(MovementCommand movementCommand, MovementActionInput movementActionInput)
        {
            _movementCommand = movementCommand;
            _movementActionInput = movementActionInput;
        }

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();
            
            _tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            _move = _tree.Q<Button>("move");
            _stop = _tree.Q<Button>("stop");
            _attack = _tree.Q<Button>("attack");
            _hold = _tree.Q<Button>("hold");
            _patrol = _tree.Q<Button>("patrol");
        }

        public void ShowSelf()
        {
            if (_shown)
            {
                return;
            }

            _parent.InfoPanel.Add(_tree);
            _shown = true;

            BindButtons();
        }

        public void HideSelf()
        {
            if (!_shown)
            {
                return;
            }
            
            UnbindButtons();
            
            _parent.InfoPanel.Remove(_tree);

            _shown = false;
        }

        private void BindButtons()
        {
            _move.clicked += Move;
            _stop.clicked += Stop;
            _attack.clicked += Attack;
            _hold.clicked += Hold;
            _patrol.clicked += Patrol;
        }

        private void UnbindButtons()
        {
            _move.clicked -= Move;
            _stop.clicked -= Stop;
            _attack.clicked -= Attack;
            _hold.clicked -= Hold;
            _patrol.clicked -= Patrol;  
        }

        private void Move()
        {
            _movementActionInput.SelectMove();
        }
        
        private void Stop()
        {
            _movementCommand.Stop();
        }
        
        private void Attack()
        {
            _movementActionInput.SelectAttack();
        }
        
        private void Hold()
        {
            _movementActionInput.SelectHold();
        }
        
        private void Patrol()
        {
            _movementActionInput.SelectPatrol();
        }
    }
}
