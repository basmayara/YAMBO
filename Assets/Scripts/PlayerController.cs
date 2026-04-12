using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 9f;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveInput;

    [HideInInspector]
    public bool isDead = false;

    private float lastMoveX;
    private float lastMoveY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;
        rb.drag = 0f;

        lastMoveY = -1f; // وجها لتحت في البدية
    }

    void Update()
    {
        if (isDead) return;

        // 1. قراءة الحركة
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        bool isMoving = moveInput.sqrMagnitude > 0;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);

        // 2. تحديث الأنميشن
        anim.SetBool("isWalking", isMoving && !isRunning);
        anim.SetBool("isRunning", isRunning);

        if (isMoving)
        {
            lastMoveX = moveInput.x;
            lastMoveY = moveInput.y;

            anim.SetFloat("xInput", moveInput.x);
            anim.SetFloat("yInput", moveInput.y);
        }
        else
        {
            // الرجوع للـ Idle في نفس الاتجاه
            anim.SetFloat("xInput", lastMoveX);
            anim.SetFloat("yInput", lastMoveY);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("Attack");
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (moveInput.sqrMagnitude > 0.01f) 
        {
            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            rb.velocity = moveInput.normalized * speed;
        }
        else
        {
            rb.velocity = Vector2.zero; 
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        anim.SetTrigger("Die");
        rb.velocity = Vector2.zero;
    }
}