using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public static int currentHealth;

    [Header("Hearts UI")]
    public Image[] hearts = new Image[3];

    [Header("Heart Sprites")]
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Invincibility Settings")]
    public float invincibilityTime = 0.5f;
    private bool isInvincible = false;

    private PlayerController playerController;

    void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();
        UpdateHearts();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || (playerController != null && playerController.isDead)) return;
        currentHealth -= damage;
        UpdateHearts();
        if (currentHealth <= 0)
        {
            if (playerController != null) playerController.Die();
            StartCoroutine(DieAndShowMenu());
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    // ✅ الإضافة الجديدة — رجع heart واحد بشوية
    public void HealOneHeart()
    {
        if (currentHealth < maxHealth)
            StartCoroutine(HealSlowly());
    }

    private IEnumerator HealSlowly()
    {
        yield return new WaitForSeconds(1.5f);
        if (currentHealth < maxHealth)
        {
            currentHealth++;
            UpdateHearts();
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        float blinkInterval = 0.1f;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float elapsed = 0f;
            while (elapsed < invincibilityTime)
            {
                sr.enabled = !sr.enabled;
                yield return new WaitForSeconds(blinkInterval);
                elapsed += blinkInterval;
            }
            sr.enabled = true;
        }
        isInvincible = false;
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                hearts[i].sprite = (i < currentHealth) ? fullHeart : emptyHeart;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    public IEnumerator DieAndShowMenu()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            yield return new WaitForSeconds(1f);
            gm.ShowRestartUI();
        }
        GetComponent<SpriteRenderer>().enabled = false;
        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;
    }
}