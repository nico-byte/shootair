//using UnityEngine;

//public class EnemyRotationController : MonoBehaviour
//{
    // reference to the ridgitbody 2D
//    private Rigidbody2D rb;

    // setting the parameter for the z rotation as x and y values
//    public string rotationXParameterName = "RotationX";
//    public string rotationYParameterName = "RotationY";

//    private void Start()
//    {
        // get the reference of the ridgidbody
//        rb = GetComponent<Rigidbody2D>();
//    }

//    private void Update()
//    {
        // define the z rotation of the enemys
//        float rotationAngle = transform.eulerAngles.z;

        // calculating the angle of the z rotation of the enemys
//        float xValue = Mathf.Cos(rotationAngle * Mathf.Deg2Rad);
//        float yValue = Mathf.Sin(rotationAngle * Mathf.Deg2Rad);

        // 
//        bool IsMoving = rb.velocity.magnitude > 0;

        // updating the x- and y values in the animator controller
//        GetComponent<Animator>().SetFloat(rotationXParameterName, xValue);
//        GetComponent<Animator>().SetFloat(rotationYParameterName, yValue);

//    }
//}
