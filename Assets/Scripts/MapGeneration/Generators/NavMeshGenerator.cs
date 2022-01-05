using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace MapGeneration.Generators
{
    public class NavMeshGenerator : MonoBehaviour
    {
        public void Build()
        {
            var agentTypeCount = NavMesh.GetSettingsCount();
            if (agentTypeCount < 1)
            {
                return;
            }

            for (var i = 0; i < agentTypeCount; i++)
            {
                BuildForAgent(i);
            }
        }

        private void BuildForAgent(int i)
        {
            var settings = NavMesh.GetSettingsByIndex(i);

            var navMeshSurface = transform.AddComponent<NavMeshSurface>();

            navMeshSurface.agentTypeID = settings.agentTypeID;
            navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;

            navMeshSurface.BuildNavMesh();
        }
    }
}