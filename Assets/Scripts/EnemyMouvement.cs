using UnityEngine;
using System.Collections;

public class EnemyMouvement : MonoBehaviour
{
    public float speed = 2f;
    public Transform LeftPoint;
    public Transform RightPoint;
    public Transform playerTransform; // جر اللاعب لهنا في الـ Inspector

    [Header("Fire Settings")]
    public GameObject firenew;
    public Transform firePoint;
    public float fireRate = 1.5f;
    public float fireSpeed = 25f;
    public float stopDistance = 3f; // المسافة اللي غايوقف فيها العدو

    private int direction = 1;
    private bool isPlayerDead = false;

    void Start()
    {
        StartCoroutine(FireRoutine());
    }

    void Update()
    {
        // 1. التحقق من حالة اللاعب (إلا سالاو القلوب)
        CheckPlayerStatus();

        if (isPlayerDead) return; // إلا مات اللاعب، العدو كايوقف ومكايضربش

        // 2. حساب المسافة بين العدو واللاعب
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > stopDistance)
        {
            // الحركة العادية بين النقطتين
            transform.Translate(Vector2.left * direction * speed * Time.deltaTime);

            if (transform.position.x < LeftPoint.position.x && direction == 1)
            {
                direction = -1;
                Flip();
            }
            else if (transform.position.x > RightPoint.position.x && direction == -1)
            {
                direction = 1;
                Flip();
            }
        }
        else
        {
            // العدو واقف وكايشوف في اتجاه اللاعب
            FacePlayer();
        }
    }

    IEnumerator FireRoutine()
    {
        while (!isPlayerDead) // كايضرب غير فاش يكون اللاعب حي
        {
            ShootFire();
            yield return new WaitForSeconds(fireRate);
        }
    }

    void FacePlayer()
    {
        // كايخلي العدو ديما يشوف جيهت اللاعب بلا ما يدوز فوقو
        if (playerTransform.position.x < transform.position.x && direction != 1)
        {
            direction = 1;
            Flip();
        }
        else if (playerTransform.position.x > transform.position.x && direction != -1)
        {
            direction = -1;
            Flip();
        }
    }

    void CheckPlayerStatus()
    {
        // كنجيبو السكريبت ديال القلوب باش نعرفو واش مات
        PlayerHealth health = playerTransform.GetComponent<PlayerHealth>();
        if (health != null && health.currentHealth <= 0)
        {
            isPlayerDead = true;
        }
    }

    void ShootFire()
    {
        if (firenew != null && firePoint != null && !isPlayerDead)
        {
            GameObject fire = Instantiate(firenew, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = fire.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.velocity = Vector2.left * fireSpeed * direction;

            Destroy(fire, 4f);
        }
    }

    void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
}