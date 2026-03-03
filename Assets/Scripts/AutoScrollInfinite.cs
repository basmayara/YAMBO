using UnityEngine;

public class AutoScrollInfinite : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    private float width;

    void Start()
    {
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // تحريك ديما
        transform.Translate(Vector2.left * scrollSpeed * Time.deltaTime);

        // ملي يخرج كامل من اليسار → رجعو لليمين
        if (transform.position.x <= -width)
        {
            transform.position += Vector3.right * width * 2f;
        }
    }
}