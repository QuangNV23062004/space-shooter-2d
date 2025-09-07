using UnityEngine;

/// <summary>
/// A class which spawns asteroids and launches them toward the player.
/// </summary>
public class AsteroidSpawner : MonoBehaviour
{
    [Header("GameObject References")]
    [Tooltip("The asteroid prefab to spawn")]
    public GameObject asteroidPrefab = null;

    [Tooltip("The target of the spawned asteroids (usually the Player)")]
    public Transform player = null;

    [Header("Spawn Position")]
    [Tooltip("The distance within which asteroids can spawn in the X direction")]
    [Min(0)]
    public float spawnRangeX = 10.0f;

    [Tooltip("The distance within which asteroids can spawn in the Y direction")]
    [Min(0)]
    public float spawnRangeY = 10.0f;

    [Header("Spawn Variables")]
    [Tooltip("The maximum number of asteroids this spawner can create")]
    public int maxSpawn = 20;

    [Tooltip("If true, ignores the maxSpawn limit and spawns infinitely")]
    public bool spawnInfinite = true;

    [Tooltip("Delay between spawns (seconds)")]
    public float spawnDelay = 2.5f;

    [Tooltip("Speed at which asteroids travel toward the target")]
    public float asteroidSpeed = 5f;

    [Tooltip("Optional parent object to organize spawned asteroids under")]
    public Transform asteroidHolder = null;

    // Internal counters
    private int currentlySpawned = 0;
    private float lastSpawnTime = Mathf.NegativeInfinity;

    private void Update()
    {
        CheckSpawnTimer();
    }

    /// <summary>
    /// Checks the timer and spawns an asteroid if it’s time.
    /// </summary>
    private void CheckSpawnTimer()
    {
        if (Time.timeSinceLevelLoad > lastSpawnTime + spawnDelay &&
            (currentlySpawned < maxSpawn || spawnInfinite))
        {
            Vector3 spawnLocation = GetSpawnLocation();
            SpawnAsteroid(spawnLocation);
        }
    }

    /// <summary>
    /// Spawns and launches an asteroid toward the player.
    /// </summary>
    private void SpawnAsteroid(Vector3 spawnLocation)
    {
        if (asteroidPrefab != null)
        {
            // Spawn asteroid
            GameObject asteroid = Instantiate(
                asteroidPrefab,
                spawnLocation,
                Quaternion.identity,
                asteroidHolder);

            // Target player if assigned
            if (player != null)
            {
                Vector3 direction = (player.position - spawnLocation).normalized;

                if (asteroid.TryGetComponent<Rigidbody2D>(out var rb2D))
                    rb2D.linearVelocity = direction * asteroidSpeed;

                if (asteroid.TryGetComponent<Rigidbody>(out var rb3D))
                    rb3D.linearVelocity = direction * asteroidSpeed;

                // Rotate asteroid to face direction (2D look-at)
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                asteroid.transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            currentlySpawned++;
            lastSpawnTime = Time.timeSinceLevelLoad;
        }
    }

    /// <summary>
    /// Calculates a random spawn position around this spawner.
    /// </summary>
    protected virtual Vector3 GetSpawnLocation()
    {
        float x = Random.Range(-spawnRangeX, spawnRangeX);
        float y = Random.Range(-spawnRangeY, spawnRangeY);
        return new Vector3(transform.position.x + x, transform.position.y + y, 0);
    }
}
