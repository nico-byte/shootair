using UnityEngine;

public class EnemyRotationController : MonoBehaviour
{
    // Referenz zum Rigidbody2D des Enemies
    private Rigidbody2D rb;

    // Der Name der Float-Parameter im Animator Controller für die horizontalen und vertikalen Komponenten
    public string rotationXParameterName = "RotationX";
    public string rotationYParameterName = "RotationY";

    private void Start()
    {
        // Hole die Referenz zum Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Bestimme die Z-Rotation des Enemies
        float rotationAngle = transform.eulerAngles.z;

        // Berechne die horizontalen und vertikalen Komponenten der Z-Rotation
        float xValue = Mathf.Cos(rotationAngle * Mathf.Deg2Rad);
        float yValue = Mathf.Sin(rotationAngle * Mathf.Deg2Rad);

        // 
        bool IsMoving = rb.velocity.magnitude > 0;

        // Aktualisiere die X- und Y-Werte im Blend Tree
        GetComponent<Animator>().SetFloat(rotationXParameterName, xValue);
        GetComponent<Animator>().SetFloat(rotationYParameterName, yValue);

    }
}
