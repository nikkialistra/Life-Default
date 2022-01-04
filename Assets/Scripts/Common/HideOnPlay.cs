using UnityEngine;

namespace Common
{
    public class HideOnPlay : MonoBehaviour
    {
        private void Start()
        {
            gameObject.SetActive(false);
        }
    }
}