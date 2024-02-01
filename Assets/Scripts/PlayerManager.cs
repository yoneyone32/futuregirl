using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [Header("走る速さ")] public float moveSpeed;
    [Header("ジャンプの高さ")]public float jumpPower;
    private Rigidbody2D rb;
    public Animator animator;
    public LayerMask blockLayer;
    public GameManager gameManager;
    public GameObject textClear;
    public GameObject btnRetry;


    Vector2 screenMin;
    private float speed;
    private bool isDead = false;

    enum DIRECTION_TYPE
    {
        IDLE,
        RIGHT,
        LEFT
    }

    DIRECTION_TYPE direction = DIRECTION_TYPE.IDLE;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDead) return;

        float x = Input.GetAxis("Horizontal");

        animator.SetFloat("isRun", Mathf.Abs(x));

        if (x == 0)
        {
            direction = DIRECTION_TYPE.IDLE;
        }
        else if (x > 0)
        {
            direction = DIRECTION_TYPE.RIGHT;
        }
        else if (x < 0)
        {
            direction = DIRECTION_TYPE.LEFT;
        }

        if (IsGround())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
            else
            {
                animator.SetBool("isJump", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            rb.gravityScale = -rb.gravityScale;
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, 1);
        }
    }

    private void FixedUpdate()
    {
        switch (direction)
        {
            case DIRECTION_TYPE.IDLE:
                speed = 0;
                break;
            case DIRECTION_TYPE.RIGHT:
                speed = moveSpeed;
                transform.localScale = new Vector3(1, transform.localScale.y, 1);
                break;
            case DIRECTION_TYPE.LEFT:
                speed = -moveSpeed;
                transform.localScale = new Vector3(-1, transform.localScale.y, 1);
                break;
        }
        rb.velocity = new Vector2(speed, rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "OutZone")
        {
            SceneManager.LoadScene("SampleScene");
        }

        if(collision.tag == "Clear")
        {
            textClear.SetActive(true);
            btnRetry.SetActive(true);
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene("SampleScene");
    }

    void Jump()
    {
        animator.SetBool("isJump", true);
        rb.AddForce(Vector2.up * jumpPower * 100);
    }

    bool IsGround()
    {
        Vector3 leftPoint = (transform.position - Vector3.up * 1.5f) - Vector3.right * 0.2f;
        Vector3 rightPoint = (transform.position - Vector3.up * 1.5f) + Vector3.right * 0.2f;
        Vector3 endPoint = (transform.position - Vector3.up * 1.5f) - Vector3.up * 0.1f;
        Debug.DrawLine(leftPoint, endPoint);
        Debug.DrawLine(rightPoint, endPoint);
        return Physics2D.Linecast(leftPoint, endPoint, blockLayer)
            || Physics2D.Linecast(rightPoint, endPoint, blockLayer);
    }
}