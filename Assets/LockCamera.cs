using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCamera : MonoBehaviour
{
    public Transform target;
    public bool follow;
 
    // Update is called once per frame
    void LateUpdate()
    {
        if (follow) transform.position = new Vector3(target.position.x, target.position.y, -40);
    }
}
