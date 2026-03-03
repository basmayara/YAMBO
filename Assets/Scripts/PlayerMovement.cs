using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin")) // كيتأكد واش هادي عملة
        {
            ScoreManager.instance.AddScore(1); // كيزيد 10 نقط
            Destroy(other.gameObject); // كيمسح العملة من الشاشة
        }
    }
}