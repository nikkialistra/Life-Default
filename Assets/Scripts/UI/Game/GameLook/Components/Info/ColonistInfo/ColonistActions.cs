using ColonistManagement.Tasking;
using Colonists;
using Sirenix.OdinInspector;
using Units.Enums;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Game.GameLook.Components.Info.ColonistInfo
{
    public class ColonistActions : MonoBehaviour
    {
        [Required]
        [SerializeField] private Sprite _iconTasks;
        [Required]
        [SerializeField] private Sprite _iconOrders;
        
        private VisualElement _currentActionIcon;
        private Label _currentAction;

        private Button _actionType;
        private VisualElement _actionTypeIcon;
        
        private Button _melee;
        private Button _ranged;
        
        private VisualElement _meleeIcon;
        private VisualElement _rangedIcon;

        private bool _isOrdering;

        private WeaponSlotType _activeWeaponSlot;

        private Colonist _colonist;
        
        private ActionIconsRegistry _actionIconsRegistry;

        [Inject]
        public void Construct(ActionIconsRegistry actionIconsRegistry)
        {
            _actionIconsRegistry = actionIconsRegistry;
        }

        public void Initialize(VisualElement tree)
        {
            _currentActionIcon = tree.Q<VisualElement>("current-action__icon");
            _currentAction = tree.Q<Label>("current-action__text");

            _actionType = tree.Q<Button>("action-type");
            _actionTypeIcon = tree.Q<VisualElement>("action-type__icon");

            _melee = tree.Q<Button>("melee");
            _ranged = tree.Q<Button>("ranged");
            
            _meleeIcon = tree.Q<VisualElement>("melee__icon");
            _rangedIcon = tree.Q<VisualElement>("ranged__icon");
        }

        public void FillIn(Colonist colonist)
        {
            _colonist = colonist;
        }

        public void BindSelf()
        {
            _actionType.clicked += ToggleActionType;

            _melee.clicked += ChooseMelee;
            _ranged.clicked += ChooseRanged;
        }

        public void UnbindSelf()
        {
            _actionType.clicked -= ToggleActionType;
            
            _melee.clicked -= ChooseMelee;
            _ranged.clicked -= ChooseRanged;
        }

        private void ToggleActionType()
        {
            _isOrdering = !_isOrdering;

            if (_isOrdering)
            {
                SwitchToOrdering();
            }
            else
            {
                SwitchToTasking();
            }
        }

        private void ChooseMelee()
        {
            if (_colonist.HasWeaponOf(WeaponSlotType.Melee))
            {
                _activeWeaponSlot = WeaponSlotType.Melee;
                UpdateWeaponType();

                _colonist.ChooseWeapon(WeaponSlotType.Melee);
            } 
        }

        private void ChooseRanged()
        {
            if (_colonist.HasWeaponOf(WeaponSlotType.Ranged))
            {
                _activeWeaponSlot = WeaponSlotType.Ranged;
                UpdateWeaponType();
                
                _colonist.ChooseWeapon(WeaponSlotType.Ranged);
            } 
        }

        private void SwitchToOrdering()
        {
            _actionTypeIcon.style.backgroundImage = new StyleBackground(_iconOrders);

            _currentActionIcon.style.backgroundImage =
                new StyleBackground(_actionIconsRegistry[ActionType.FollowingOrders]);
            _currentAction.text = ActionType.FollowingOrders.GetString();
        }

        private void SwitchToTasking()
        {
            _actionTypeIcon.style.backgroundImage = new StyleBackground(_iconTasks);
            
            _currentActionIcon.style.backgroundImage =
                new StyleBackground(_actionIconsRegistry[ActionType.Relaxing]);
            _currentAction.text = ActionType.Relaxing.GetString();
        }

        private void UpdateWeaponType()
        {
            if (_activeWeaponSlot == WeaponSlotType.Melee)
            {
                _melee.RemoveFromClassList("disabled");
                _ranged.AddToClassList("disabled");
            }
            else
            {
                _melee.AddToClassList("disabled");
                _ranged.RemoveFromClassList("disabled");
            }
        }
    }
}
