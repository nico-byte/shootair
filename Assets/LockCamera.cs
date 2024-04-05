using UnityEngine;

namespace ShootAirRLAgent
{
    public class LockCamera : MonoBehaviour
    {
        [SerializeField]
        private Transform target;
        [SerializeField]
        private bool follow;
    
        // Update is called once per frame
        void LateUpdate()
        {
            if (follow) transform.position = new Vector3(target.position.x, target.position.y, -40);
        }
    }
}
