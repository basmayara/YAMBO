using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Hearts UI")]
    public Image[] hearts = new Image[3];

    [Header("Heart Sprites")]
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Invincibility Settings")]
    public float invincibilityTime = 0.5f;
    private bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHearts();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        UpdateHearts();

        if (currentHealth <= 0)
        {
            StartCoroutine(DieAndShowMenu());
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
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
        yield return new WaitForSeconds(invincibilityTime);
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

    // هادي هي الدالة الوحيدة اللي غاتبقى دابا (سميتها DieAndShowMenu لتفادي أي خلط)
    public IEnumerator DieAndShowMenu()
    {
        Debug.Log("PLAYER IS DEAD");

        // 1. أولاً نعلم الـ GameManager يشعل الـ Canvas
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            Debug.Log("GameManager FOUND");
            gm.ShowRestartUI();
        }

        // 2. ثانياً نخفي اللاعب أو نعطل حركته
        // يفضل تعطيل الـ Sprite والـ Collider بدل إطفاء الكائن كلياً فوراً
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        // أو إذا أردتِ إطفاءه تماماً اجعليها هي آخر خطوة
        // gameObject.SetActive(false); 

        yield return null;
    }

}