﻿using System;
using System.Collections.Generic;
using Colonists.Activities;
using Colonists.Skills;
using Colonists.Stats;
using Common;
using Humans;
using Humans.Appearance;
using ResourceManagement;
using Sirenix.OdinInspector;
using Units;
using Units.Ancillaries;
using Units.Enums;
using Units.Traits;
using UnityEngine;
using Zenject;
using static Humans.Appearance.HumanAppearanceRegistry;

namespace Colonists
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(UnitSelection))]
    [RequireComponent(typeof(UnitAttacker))]
    [RequireComponent(typeof(ColonistStats))]
    [RequireComponent(typeof(ColonistTraits))]
    [RequireComponent(typeof(ColonistAnimator))]
    [RequireComponent(typeof(ColonistMeshAgent))]
    [RequireComponent(typeof(ColonistGatherer))]
    [RequireComponent(typeof(ColonistGathererParameters))]
    [RequireComponent(typeof(ColonistBehavior))]
    [RequireComponent(typeof(ColonistSkills))]
    [RequireComponent(typeof(ColonistActivities))]
    public class Colonist : MonoBehaviour
    {
        public event Action Spawn;
        public event Action VitalityChange;
        public event Action Dying;
        public event Action<Colonist> ColonistDying;

        public event Action<string> NameChange;

        public event Action<ActivityType> ActivityChange;
        public event Action TraitsChange;
        public event Action<Skill> SkillChange;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NameChange?.Invoke(_name);
            }
        }

        public Unit Unit { get; private set; }

        public bool Alive => Unit.Alive;

        public IReadOnlyList<Skill> Skills => _colonistSkills.Skills;
        public IReadOnlyList<Trait> Traits => _colonistTraits.Traits;

        public Vector3 Center => _center.position;

        [SerializeField] private string _name;
        [SerializeField] private Gender _gender;
        [Space]
        [Required]
        [SerializeField] private HumanAppearance _humanAppearance;
        [Space]
        [Required]
        [SerializeField] private GameObject _selectionIndicator;
        [Required]
        [SerializeField] private Transform _center;

        private UnitSelection _unitSelection;
        private UnitAttacker _unitAttacker;

        private ColonistStats _colonistStats;
        private ColonistTraits _colonistTraits;
        private ColonistAnimator _colonistAnimator;
        private ColonistMeshAgent _colonistMeshAgent;
        private ColonistGatherer _colonistGatherer;
        private ColonistGathererParameters _colonistGathererParameters;
        private ColonistBehavior _colonistBehavior;
        private ColonistSkills _colonistSkills;
        private ColonistActivities _colonistActivities;

        private HumanAppearanceRegistry _humanAppearanceRegistry;
        private HumanNames _humanNames;

        [Inject]
        public void Construct(HumanAppearanceRegistry humanAppearanceRegistry, HumanNames humanNames)
        {
            _humanAppearanceRegistry = humanAppearanceRegistry;
            _humanNames = humanNames;
        }

        private void Awake()
        {
            Unit = GetComponent<Unit>();
            _unitSelection = GetComponent<UnitSelection>();
            _unitAttacker = GetComponent<UnitAttacker>();

            _colonistStats = GetComponent<ColonistStats>();
            _colonistTraits = GetComponent<ColonistTraits>();
            _colonistAnimator = GetComponent<ColonistAnimator>();
            _colonistMeshAgent = GetComponent<ColonistMeshAgent>();
            _colonistGatherer = GetComponent<ColonistGatherer>();
            _colonistGathererParameters = GetComponent<ColonistGathererParameters>();
            _colonistBehavior = GetComponent<ColonistBehavior>();
            _colonistSkills = GetComponent<ColonistSkills>();
            _colonistActivities = GetComponent<ColonistActivities>();
        }

        private void Start()
        {
            Initialize();
            ActivateComponents();

            Spawn?.Invoke();
        }

        private void OnEnable()
        {
            _colonistActivities.ActivityChange += OnActivityChange;

            Unit.VitalityChange += OnVitalityChange;
            Unit.Dying += OnDying;
        }

        private void OnDisable()
        {
            _colonistActivities.ActivityChange -= OnActivityChange;

            Unit.VitalityChange -= OnVitalityChange;
            Unit.Dying -= OnDying;
        }

        private void OnDestroy()
        {
            UnbindStatsFromComponents();
        }

        public void SetAt(Vector3 position)
        {
            transform.position = position;
        }

        [Button(ButtonSizes.Medium)]
        public void RandomizeAppearance()
        {
            _humanAppearance.RandomizeAppearanceWith(_gender, HumanType.Colonist, _humanAppearanceRegistry);
        }

        [Button(ButtonSizes.Medium)]
        public void AddTrait(Trait trait)
        {
            _colonistTraits.AddTrait(trait);
            Unit.AddTrait(trait);

            TraitsChange?.Invoke();
        }

        [Button(ButtonSizes.Medium)]
        public void RemoveTrait(Trait trait)
        {
            _colonistTraits.RemoveTrait(trait);
            Unit.RemoveTrait(trait);

            TraitsChange?.Invoke();
        }

        public void Select()
        {
            if (!Unit.Alive) return;

            _unitSelection.Select();
            _selectionIndicator.SetActive(true);
            _colonistMeshAgent.ShowLinePath();

            Unit.Select();
        }

        public void Deselect()
        {
            _unitSelection.Deselect();
            _selectionIndicator.SetActive(false);
            _colonistMeshAgent.HideLinePath();

            Unit.Deselect();
        }

        public void Stop()
        {
            _colonistBehavior.Stop();
        }

        public bool HasWeaponOf(WeaponSlotType weaponSlotType)
        {
            return Unit.HasWeaponOf(weaponSlotType);
        }

        public void ChooseWeapon(WeaponSlotType weaponSlotType)
        {
            Unit.ChooseWeapon(weaponSlotType);
        }

        public void OrderTo(Colonist targetColonist)
        {
            if (this == targetColonist) return;

            _colonistBehavior.OrderTo(targetColonist);
        }

        public void OrderTo(Unit unitTarget)
        {
            _colonistBehavior.OrderTo(unitTarget);
        }

        public void OrderTo(Resource resource)
        {
            _colonistBehavior.OrderTo(resource);
        }

        public void OrderToPosition(Vector3 position, float? angle)
        {
            _colonistBehavior.OrderToPosition(position, angle);
        }

        public void TryAddPositionToOrder(Vector3 position, float? angle)
        {
            _colonistBehavior.AddPositionToOrder(position, angle);
        }

        public void Die()
        {
            Unit.Die();
        }

        public void ToggleUnitFieldOfView()
        {
            Unit.ToggleUnitVisibilityFields();
        }

        public void ToggleResourceFieldOfView()
        {
            _colonistGatherer.ToggleResourceFieldOfView();
        }

        public void ImproveSkill(SkillType skillType, int quantity)
        {
            _colonistSkills.ImproveSkill(skillType, quantity);

            SkillChange?.Invoke(_colonistSkills.GetSkill(skillType));
        }

        private void OnDying()
        {
            Stop();

            DeactivateComponents();

            Deselect();

            ColonistDying?.Invoke(this);
            Dying?.Invoke();

            _colonistAnimator.Die(DestroySelf);
        }

        private void Initialize()
        {
            _gender = EnumUtils.RandomValue<Gender>();

            if (_name == "")
                _name = _humanNames.GetRandomNameFor(_gender);

            _humanAppearance.RandomizeAppearanceWith(_gender, HumanType.Colonist, _humanAppearanceRegistry);

            BindStatsToComponents();

            InitializeUnit();
        }

        private void InitializeUnit()
        {
            foreach (var trait in _colonistTraits.Traits)
                Unit.AddTrait(trait);

            Unit.Initialize();
        }

        private void BindStatsToComponents()
        {
            _colonistGathererParameters.BindStats(_colonistStats.ResourceDestructionSpeed,
                _colonistStats.ResourceExtractionEfficiency);
        }

        private void UnbindStatsFromComponents()
        {
            _colonistGathererParameters.UnbindStats(_colonistStats.ResourceDestructionSpeed,
                _colonistStats.ResourceExtractionEfficiency);
        }

        private void ActivateComponents()
        {
            _unitSelection.Activate();
            _colonistMeshAgent.Activate();
            _colonistBehavior.Activate();
        }

        private void DeactivateComponents()
        {
            Unit.HideUnitVisibilityFields();
            _colonistGatherer.HideResourceFieldOfView();

            _unitSelection.Deactivate();
            _unitAttacker.CoverUnitTarget();
            _unitAttacker.FinalizeAttackingInstantly();

            _colonistMeshAgent.Deactivate();
            _colonistBehavior.Deactivate();
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }

        private void OnVitalityChange()
        {
            VitalityChange?.Invoke();
        }

        private void OnActivityChange(ActivityType activityType)
        {
            ActivityChange?.Invoke(activityType);
        }

        public class Factory : PlaceholderFactory<Colonist> { }
    }
}
