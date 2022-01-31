using UnityEngine;

namespace Entities.Entity
{
    public class ObjectRotation : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private void Update()
        {
            transform.Rotate(0, _speed * Time.deltaTime, 0);
        }
    }
}
