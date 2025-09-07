using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LivesDisplaySimple : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;   // drag your Player object here

    public Image heartIcon;             // drag HeartIcon (Image) here
    public TextMeshProUGUI lifeCountText; // drag LifeCountText (TextMeshPro) here

    [Header("Sprites")]
    public Sprite fullLifeSprite;       // assign full heart sprite

    private Health health;

    private void Start()
    {
        if (playerTransform != null)
            health = playerTransform.GetComponent<Health>();

        if (heartIcon != null && fullLifeSprite != null)
            heartIcon.sprite = fullLifeSprite;
    }

    private void Update()
    {
        if (health != null && health.useLives)
        {
            if (health.currentLives > 0)
            {
                // show and update
                if (heartIcon != null) heartIcon.enabled = true;
                if (lifeCountText != null)
                {
                    lifeCountText.enabled = true;
                    lifeCountText.text = "x " + health.currentLives;
                }
            }
            else
            {
                // hide when dead
                if (heartIcon != null) heartIcon.enabled = false;
                if (lifeCountText != null) lifeCountText.enabled = false;
            }
        }
    }
}
