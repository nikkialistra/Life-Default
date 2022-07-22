using Pathfinding;
using Sirenix.OdinInspector;
using Units;
using UnityEngine;
using Zenject;

namespace Colonists
{
    [RequireComponent(typeof(UnitMeshAgent))]
    public class ColonistMeshAgentPath : MonoBehaviour
    {
        [Required]
        [SerializeField] private LineRenderer _pathLineRenderer;
        [SerializeField] private Vector3 _pathLineOffset = new(0, 0.2f, 0);

        private Transform _pathLineParent;

        private UnitMeshAgent _unitMeshAgent;

        [Inject]
        public void Construct(Transform pathLineParent)
        {
            _pathLineParent = pathLineParent;
        }

        private void Awake()
        {
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
        }

        private void Start()
        {
            _pathLineRenderer.transform.parent = _pathLineParent;
            _pathLineRenderer.transform.position = Vector3.zero;
        }

        public void ShowPathLine(Path path)
        {
            if (!_unitMeshAgent.IsMoving) return;

            var pathNodes = path.path;

            if (pathNodes.Count <= 2)
            {
                _pathLineRenderer.positionCount = 0;
                return;
            }

            _pathLineRenderer.positionCount = pathNodes.Count - 1;

            for (int i = 1; i < pathNodes.Count; i++)
                _pathLineRenderer.SetPosition(i - 1,
                    (Vector3)pathNodes[i].position + _pathLineOffset);
        }

        public void HidePathLine()
        {
            _pathLineRenderer.positionCount = 0;
        }
    }
}
