using UnityEngine;

/// <summary>
/// A component that makes the attached GameObject orbit around a central point.
/// Attach this to your AsteroidSpawner or EnemySpawner GameObject to make it spin in a circle.
/// Assumes 2D movement (rotation around Z-axis) but supports 3D positions.
/// </summary>
public class OrbitAround : MonoBehaviour
{
    [Tooltip("Optional: The dynamic central point to orbit around (e.g., the player's Transform). If assigned, this overrides the fixed Center Position.")]
    public Transform centerTransform;

    [Tooltip("The fixed central position to orbit around (x, y, z coordinates). Used if Center Transform is not assigned.")]
    public Vector3 centerPosition = Vector3.zero;

    [Tooltip("The rotation speed in degrees per second. Positive for counterclockwise, negative for clockwise.")]
    public float angularSpeed = 90f;

    private void Update()
    {
        Vector3 center = centerTransform != null ? centerTransform.position : centerPosition;
        transform.RotateAround(center, Vector3.forward, angularSpeed * Time.deltaTime);
    }
}