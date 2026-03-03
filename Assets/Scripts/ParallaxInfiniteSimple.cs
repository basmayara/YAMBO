using UnityEngine;

public class ParallaxInfiniteSimple : MonoBehaviour
{
    public float parallaxSpeed = 0.3f;
    private float width;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // parallax (clouds يتحركو أبطأ من الكاميرا)
        transform.position += Vector3.left * parallaxSpeed * Time.deltaTime;

        // infinite reset
        if (transform.position.x <= cam.position.x - width)
        {
            transform.position += Vector3.right * width * 2f;
        }
    }
}