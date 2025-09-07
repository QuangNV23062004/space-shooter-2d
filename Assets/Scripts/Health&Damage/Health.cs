using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// This class handles the health state of a game object.
/// 
/// Implementation Notes: 2D Rigidbodies must be set to never sleep for this to interact with trigger stay damage
/// </summary>
public class Health : MonoBehaviour
{
    [Header("Team Settings")]
    [Tooltip("The team associated with this damage")]
    public int teamId = 0;

    [Header("Health Settings")]
    [Tooltip("The default health value")]
    public int defaultHealth = 1;
    [Tooltip("The maximum health value")]
    public int maximumHealth = 1;
    [Tooltip("The current in game health value")]
    public int currentHealth = 1;
    [Tooltip("Invulnerability duration, in seconds, after taking damage")]
    public float invincibilityTime = 3f;
    [Tooltip("Whether or not this health is always invincible")]
    public bool isAlwaysInvincible = false;

    [Header("Lives settings")]
    [Tooltip("Whether or not to use lives")]
    public bool useLives = false;
    [Tooltip("Current number of lives this health has")]
    public int currentLives = 3;
    [Tooltip("The maximum number of lives this health can have")]
    public int maximumLives = 5;


    private Controller controller;
    private Rigidbody2D rb;
    /// <summary>
    /// Description:
    /// Standard unity funciton called before the first frame update
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    void Start()
    {
        SetRespawnPoint(transform.position);
        controller = GetComponent<Controller>();
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Description:
    /// Standard Unity function called once per frame
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    void Update()
    {
        InvincibilityCheck();
    }

    // The specific game time when the health can be damged again
    private float timeToBecomeDamagableAgain = 0;
    // Whether or not the health is invincible
    private bool isInvincableFromDamage = false;

    /// <summary>
    /// Description:
    /// Checks against the current time and the time when the health can be damaged again.
    /// Removes invicibility if the time frame has passed
    /// Inputs:
    /// None
    /// Returns:
    /// void (no return)
    /// </summary>
    private void InvincibilityCheck()
    {
        if (timeToBecomeDamagableAgain <= Time.time)
        {
            isInvincableFromDamage = false;
        }
    }

    // The position that the health's gameobject will respawn at if lives are being used
    private Vector3 respawnPosition;
    /// <summary>
    /// Description:
    /// Changes the respawn position to a new position
    /// Inputs:
    /// Vector3 newRespawnPosition
    /// Returns:
    /// void (no return)
    /// </summary>
    /// <param name="newRespawnPosition">The new position to respawn at</param>
    public void SetRespawnPoint(Vector3 newRespawnPosition)
    {
        respawnPosition = newRespawnPosition;
    }

    /// <summary>
    /// Description:
    /// Repositions the health's game object to the respawn position and resets the health to the default value
    /// Inputs:
    /// None
    /// Returns:
    /// void (no return)
    /// </summary>
    void Respawn()
    {
        transform.position = respawnPosition;
        currentHealth = defaultHealth;
    }

    /// <summary>
    /// Description:
    /// Applies damage to the health unless the health is invincible.
    /// Inputs:
    /// int damageAmount
    /// Returns:
    /// void (no return)
    /// </summary>
    /// <param name="damageAmount">The amount of damage to take</param>
    public void TakeDamage(int damageAmount)
    {
        if (isInvincableFromDamage || isAlwaysInvincible)
        {
            return;
        }
        else
        {
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, transform.rotation, null);
            }
            timeToBecomeDamagableAgain = Time.time + invincibilityTime;
            isInvincableFromDamage = true;
            currentHealth -= damageAmount;
            CheckDeath();
        }
    }

    /// <summary>
    /// Description:
    /// Applies healing to the health, capped out at the maximum health.
    /// Inputs:
    /// int healingAmount
    /// Returns:
    /// void (no return)
    /// </summary>
    /// <param name="healingAmount">How much healing to apply</param>
    public void ReceiveHealing(int healingAmount)
    {
        currentHealth += healingAmount;
        if (currentHealth > maximumHealth)
        {
            currentHealth = maximumHealth;
        }
        CheckDeath();
    }

    [Header("Effects & Polish")]
    [Tooltip("The effect to create when this health dies")]
    public GameObject deathEffect;
    [Tooltip("The effect to create when this health is damaged")]
    public GameObject hitEffect;

    /// <summary>
    /// Description:
    /// Checks if the health is dead or not. If it is, true is returned, false otherwise.
    /// Calls Die() if the health is dead.
    /// Inputs:
    /// none
    /// Returns:
    /// bool
    /// </summary>
    /// <returns>Bool: true or false value representing if the health has died or not (true for dead)</returns>
    bool CheckDeath()
    {
        if (currentHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Description:
    /// Handles the death of the health. If a death effect is set, it is created. If lives are being used, the health is respawned.
    /// If lives are not being used or the lives are 0 then the health's game object is destroyed.
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    public void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation, null);
        }

        // if (rb != null)
        // {
        //     rb.linearVelocity = Vector2.zero;
        //     rb.angularVelocity = 0f;
        // }

        // // Disable movement
        // if (controller != null && controller.movementMode == Controller.MovementModes.Astroids)
        // {
        //     controller.enabled = false;
        // }

        if (useLives)
        {
            HandleDeathWithLives();
        }
        else
        {
            HandleDeathWithoutLives();
        }
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincableFromDamage = true;

        // --- Notify start ---
        Debug.Log("Player is now IMMORTAL!");

        // Sprite flickering
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;

        // Speed buff (assumes player has a script with 'moveSpeed')
        Controller pm = GetComponent<Controller>();
        float originalSpeed = 0f;
        if (pm != null)
        {
            originalSpeed = pm.moveSpeed;
            pm.moveSpeed *= 1.5f; // Increase speed by 50%
        }

        float elapsed = 0f;
        while (elapsed < invincibilityTime)
        {
            // Flicker player sprite
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b,
                                 Mathf.PingPong(Time.time * 5, 1f) > 0.5f ? 0.3f : 1f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset visuals
        sr.color = originalColor;

        // Reset speed buff
        if (pm != null)
        {
            pm.moveSpeed = originalSpeed;
        }

        // --- Notify end ---
        isInvincableFromDamage = false;
        Debug.Log("Player is now VULNERABLE again.");
    }



    /// <summary>
    /// Description:
    /// Handles the death of the health when lives are being used
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    void HandleDeathWithLives()
    {
        currentLives -= 1;
        if (currentLives > 0)
        {
            currentHealth = defaultHealth;

            // if (controller != null)
            // {
            //     controller.enabled = true;
            // }
            StartCoroutine(InvincibilityCoroutine());

            // Respawn();
        }
        else
        {
            if (gameObject.name == "Player" && gameObject.tag != "Player")
            {
                Debug.LogWarning("It looks like you're trying to kill a player, but your player hasn't been tagged as 'Player' in the inspector! \n Please tag your player.");
            }
            if (gameObject.tag == "Player" && GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }
            if (gameObject.GetComponent<Enemy>() != null)
            {
                gameObject.GetComponent<Enemy>().DoBeforeDestroy();
            }
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Description:
    /// Handles death when lives are not being used
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    void HandleDeathWithoutLives()
    {
        if (gameObject.tag == "Player" && GameManager.instance != null)
        {
            GameManager.instance.GameOver();
        }
        if (gameObject.GetComponent<Enemy>() != null)
        {
            gameObject.GetComponent<Enemy>().DoBeforeDestroy();
        }
        Destroy(this.gameObject);
    }
}
