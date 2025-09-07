using UnityEngine;

public class BulletLifeSpan : MonoBehaviour
{
    [Header("World Bounds")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    [Header("Safety")]
    [Tooltip("Destroy after this many seconds no matter what.")]
    public float maxLifeSeconds = 5f;

    [Tooltip("Also destroy when not visible by any camera.")]
    public bool destroyWhenInvisible = true;

    void Start()
    {
        if (maxLifeSeconds > 0f)
            Destroy(gameObject, maxLifeSeconds);
    }

    void Update()
    {
        Vector3 p = transform.position;
        if (p.x < minX || p.x > maxX || p.y < minY || p.y > maxY)
        {
            Destroy(gameObject);
        }
    }

    // Optional: only runs if destroyWhenInvisible = true
    void OnBecameInvisible()
    {
        if (destroyWhenInvisible)
            Destroy(gameObject);
    }
}
