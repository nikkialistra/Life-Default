// using System;
// using System.Collections;
// using Animancer;
// using Animancer.FSM;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace Units.Humans.Animations.Actions.States
// {
//     [RequireComponent(typeof(AnimancerComponent))]
//     [RequireComponent(typeof(HumanAnimations))]
//     public abstract class ActionsHumanState : MonoBehaviour, IState
//     {
//         [Required]
//         [SerializeField] protected ClipTransition _clip;
//         [Space]
//         [Required]
//         [SerializeField] private UnitMeshAgent _unitMeshAgent;
//         [Space]
//         [SerializeField] private float _waitTimeToIdle = 0.1f;
//
//         protected AnimancerComponent _animancer;
//
//         private HumanAnimations _humanAnimations;
//
//         private Coroutine _lowerBodyOverwriteCoroutine;
//
//         private bool _isMoving;
//
//         private Coroutine _updatingMovingCoroutine;
//         private Coroutine _idleCoroutine;
//
//         private void Awake()
//         {
//             _animancer = GetComponent<AnimancerComponent>();
//             _humanAnimations = GetComponent<HumanAnimations>();
//         }
//
//         public virtual ActionsAnimationType ActionsAnimationType =>
//             throw new InvalidOperationException("Cannot get actions animation type of base human state");
//
//         public virtual bool CanEnterState => true;
//
//         public virtual bool CanExitState => true;
//
//         public virtual void OnEnterState()
//         {
//             _animancer.Layers[AnimationLayers.Actions].Weight = 0;
//             _animancer.Layers[AnimationLayers.Actions].Play(_clip);
//             _animancer.Layers[AnimationLayers.Actions].StartFade(1f, _clip.FadeDuration);
//             
//             
//         }
//
//         public virtual void OnExitState()
//         {
//             
//         }
//     }
// }
