using System;
using ColonistManagement.Tasking;
using Colonists;
using Colonists.Activities;
using Sirenix.OdinInspector;
using Units.Enums;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Game.GameLook.Components.Info.ColonistInfo
{
    public class ColonistActivityPanel : MonoBehaviour
    {
        [Required]
        [SerializeField] private Sprite _iconTasks;
        [Required]
        [SerializeField] private Sprite _iconOrders;
        
        private VisualElement _currentActivityIcon;
        private Label _currentActivity;

        private Button _activityType;
        private VisualElement _activityTypeIcon;
        
        private Button _melee;
        private Button _ranged;

        private bool _isOrdering;

        private WeaponSlotType _activeWeaponSlot = WeaponSlotType.None;

        private Colonist _colonist;
        
        private ActionIconsRegistry _actionIconsRegistry;

        [Inject]
        public void Construct(ActionIconsRegistry actionIconsRegistry)
        {
            _actionIconsRegistry = actionIconsRegistry;
        }

        public void Initialize(VisualElement tree)
        {
            _currentActivityIcon = tree.Q<VisualElement>("current-activity__icon");
            _currentActivity = tree.Q<Label>("current-activity__text");

            _activityType = tree.Q<Button>("activity-type");
            _activityTypeIcon = tree.Q<VisualElement>("activity-type__icon");

            _melee = tree.Q<Button>("melee");
            _ranged = tree.Q<Button>("ranged");
        }

        public void FillIn(Colonist colonist)
        {
            _colonist = colonist;
            
            _colonist.ActivityChange -= UpdateActivity;
            _colonist.ActivityChange += UpdateActivity;
        }

        public void BindSelf()
        {
            _activityType.clicked += ToggleActivityType;

            _melee.clicked += ChooseMelee;
            _ranged.clicked += ChooseRanged;
        }

        public void UnbindSelf()
        {
            if (_colonist == null)
            {
                return;
            }
            
            _colonist.ActivityChange -= UpdateActivity;
            
            _activityType.clicked -= ToggleActivityType;
            
            _melee.clicked -= ChooseMelee;
            _ranged.clicked -= ChooseRanged;
        }

        private void UpdateActivity(ActivityType activityType)
        {
            _currentActivity.text = activityType.ToString();
        }

        private void ToggleActivityType()
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
            _activityTypeIcon.style.backgroundImage = new StyleBackground(_iconOrders);

            _currentActivityIcon.style.backgroundImage =
                new StyleBackground(_actionIconsRegistry[ActionType.FollowingOrders]);
            _currentActivity.text = ActionType.FollowingOrders.GetString();
        }

        private void SwitchToTasking()
        {
            _activityTypeIcon.style.backgroundImage = new StyleBackground(_iconTasks);
            
            _currentActivityIcon.style.backgroundImage =
                new StyleBackground(_actionIconsRegistry[ActionType.Relaxing]);
            _currentActivity.text = ActionType.Relaxing.GetString();
        }

        private void UpdateWeaponType()
        {
            switch (_activeWeaponSlot)
            {
                case WeaponSlotType.Melee:
                    _melee.RemoveFromClassList("disabled");
                    _ranged.AddToClassList("disabled");
                    break;
                case WeaponSlotType.Ranged:
                    _melee.AddToClassList("disabled");
                    _ranged.RemoveFromClassList("disabled");
                    break;
                case WeaponSlotType.None:
                    _melee.AddToClassList("disabled");
                    _ranged.AddToClassList("disabled");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
