using UnityEngine;
using TMPro;

public class HackerPatrol : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;
    public float chaseSpeed = 6f;
    public float stopDistance = 2f;
    public float detectionRadius = 10f;
    public float rotationSpeed = 1f;
    public Transform castleCenter;

    [Header("Ellipse Orbit")]
    public float radiusX = 8f;
    public float radiusY = 4f;
    public float verticalOffset = -1.5f;

    [Header("UI Dialogue")]
    public GameObject dialogueBox;
    // J'ai enlevé dialogueText d'ici pour éviter les erreurs NullReference
    public TMP_InputField codeInputField;

    private Animator anim;
    private bool isTalking = false;
    private Transform player;
    private Rigidbody2D rb;
    private float currentAngle = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
        rb.gravityScale = 0;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (dialogueBox != null) dialogueBox.SetActive(false);
        anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        if (isTalking || player == null) return;

        float distToPlayer = Vector2.Distance(rb.position, (Vector2)player.position);

        if (distToPlayer < detectionRadius)
        {
            ChasePlayer(distToPlayer);
        }
        else
        {
            CircleAroundCastle();
        }
    }

    void CircleAroundCastle()
    {
        currentAngle += rotationSpeed * Time.fixedDeltaTime;

        float x = castleCenter.position.x + Mathf.Cos(currentAngle) * radiusX;
        float y = (castleCenter.position.y + verticalOffset) + Mathf.Sin(currentAngle) * radiusY;

        Vector2 targetPos = new Vector2(x, y);
        Vector2 direction = (targetPos - rb.position).normalized;

        rb.velocity = direction * speed;
        UpdateAnimation(direction);
    }

    void ChasePlayer(float dist)
    {
        if (dist > stopDistance)
        {
            Vector2 direction = ((Vector2)player.position - rb.position).normalized;
            rb.velocity = direction * chaseSpeed;
            UpdateAnimation(direction);
        }
        else
        {
            rb.velocity = Vector2.zero;
            if (!isTalking) StartConversation();
        }
    }

    void UpdateAnimation(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0.01f)
        {
            Vector2 normalizedDir = dir.normalized;
            anim.SetFloat("xInput", normalizedDir.x);
            anim.SetFloat("yInput", normalizedDir.y);
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    void StartConversation()
    {
        isTalking = true;
        anim.SetBool("isWalking", false);
        rb.velocity = Vector2.zero;

        var playerScript = player.GetComponent<PlayerController>();
        if (playerScript != null) playerScript.enabled = false;

        var playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null) playerRb.velocity = Vector2.zero;

        if (dialogueBox != null)
        {
            dialogueBox.SetActive(true);
            // Ligne de texte supprimée ici comme demandé
        }
    }

    public void OnSubmitCode()
    {
        string correctCode = "5821";
        if (codeInputField.text == correctCode)
        {
            dialogueBox.SetActive(false);
            var playerScript = player.GetComponent<PlayerController>();
            if (playerScript != null) playerScript.enabled = true;
            Destroy(gameObject);
        }
        else
        {
            var health = player.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(1);
            codeInputField.text = "";
        }
    }
}