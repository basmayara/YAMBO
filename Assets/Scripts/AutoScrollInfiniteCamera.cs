using UnityEngine;

public class AutoScrollInfiniteCamera : MonoBehaviour
{
    public float scrollSpeed = 0.3f;

    private float width;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        transform.Translate(Vector2.left * scrollSpeed * Time.deltaTime);

        // إلى خرج كامل من جهة اليسار ديال الكاميرا
        if (transform.position.x + width / 2 < cam.position.x - width)
        {
            transform.position += Vector3.right * width * 2f;
        }
    }
}