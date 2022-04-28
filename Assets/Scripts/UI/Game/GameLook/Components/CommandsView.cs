using ColonistManagement.Movement;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Game.GameLook.Components
{
    public class CommandsView : MonoBehaviour
    {
        [Required]
        [SerializeField] private VisualTreeAsset _asset;

        private Button _move;
        private Button _stop;
        private Button _attack;
        private Button _hold;
        private Button _patrol;
        
        private bool _shown;

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
            _tree = _asset.CloneTree();

            _move = _tree.Q<Button>("move");
            _stop = _tree.Q<Button>("stop");
            _attack = _tree.Q<Button>("attack");
            _hold = _tree.Q<Button>("hold");
            _patrol = _tree.Q<Button>("patrol");
        }

        public void BindSelf(VisualElement parent)
        {
            if (_shown)
            {
                return;
            }

            if (parent.childCount == 0)
            {
                parent.Add(_tree);
            }
            
            _shown = true;

            BindButtons();
        }

        public void UnbindSelf()
        {
            if (!_shown)
            {
                return;
            }
            
            UnbindButtons();

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
