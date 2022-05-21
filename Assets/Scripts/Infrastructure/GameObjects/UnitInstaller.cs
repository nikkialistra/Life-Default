using Sirenix.OdinInspector;
using Units;
using UnityEngine;
using Zenject;

namespace Infrastructure.GameObjects
{
    public class UnitInstaller : MonoInstaller
    {
        [Required]
        [SerializeField] private UnitMeshAgent _unitMeshAgent;
        [Required]
        [SerializeField] private UnitAttacker _unitAttacker;

        public override void InstallBindings()
        {
            Container.BindInstance(_unitMeshAgent);
            Container.BindInstance(_unitAttacker);
        }
    }
}
