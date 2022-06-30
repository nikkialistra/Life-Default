using System.Collections;
using Sirenix.OdinInspector;
using Units;
using Units.Enums;
using UnityEngine;

namespace Colonists
{
    [RequireComponent(typeof(UnitAttacker))]
    [RequireComponent(typeof(Colonist))]
    public class ColonistNeedsIndication : MonoBehaviour
    {
        [Title("Want Escape")]
        [SerializeField] private float _escapeRelieveDistanceFromOpponents = 15f;
        [SerializeField] private float _rescanTime = 0.2f;
        [SerializeField] private LayerMask _targetMask;

        [SerializeField] private GameObject _wantEscapeIndicator;

        private UnitAttacker _unitAttacker;

        private Colonist _colonist;

        private void Awake()
        {
            _unitAttacker = GetComponent<UnitAttacker>();

            _colonist = GetComponent<Colonist>();
        }
        
        private void OnEnable()
        {
            _colonist.Dying += HideAllIndicators;
            
            _unitAttacker.WantEscape += ShowWantEscapeIndicator;
        }

        private void OnDisable()
        {
            _colonist.Dying += HideAllIndicators;
            
            _unitAttacker.WantEscape -= ShowWantEscapeIndicator;
        }

        private void HideAllIndicators()
        {
            StopAllCoroutines();
            
            _wantEscapeIndicator.SetActive(false);
        }

        private void ShowWantEscapeIndicator()
        {
            _wantEscapeIndicator.SetActive(true);
            StartCoroutine(Escaping());
        }

        private IEnumerator Escaping()
        {
            while (IsOpponentsAround())
                yield return new WaitForSeconds(_rescanTime);

            _wantEscapeIndicator.SetActive(false);
        }

        private bool IsOpponentsAround()
        {
            var colliders = Physics.OverlapSphere(transform.position, _escapeRelieveDistanceFromOpponents, _targetMask);
            
            foreach (var collider in colliders)
                if (collider.TryGetComponent(out Unit unit) && unit.Faction == Faction.Enemies)
                    return true;

            return false;
        }
    }
}
