using NPBehave;
using UnityEngine;

namespace Units.Unit
{
    public class UnitBehavior : MonoBehaviour
    {
        private Root _behaviorTree;

        private void Start()
        {
            ConstructBehaviorTree();
            _behaviorTree.Start();
        }

        private void ConstructBehaviorTree()
        {
            _behaviorTree = new Root(
                new Sequence(
                    new Action(() => Debug.Log("Hello World!")),
                    new WaitUntilStopped()
                )
            );
        }
    }
}
