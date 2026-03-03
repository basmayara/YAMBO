using UnityEngine;

public class GroundInfinite : MonoBehaviour
{
    public float speed = 1.5f;

    private float width;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;

        // ناخدو العرض الحقيقي ديال الground
        width = GetComponent<BoxCollider2D>().bounds.size.x;
    }

    void Update()
    {
        // تحريك الأرض لليسار
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // ملي يخرج ground كامل من الكاميرا
        if (transform.position.x <= cam.position.x - width)
        {
            transform.position += Vector3.right * width * 2f;
        }
    }
}