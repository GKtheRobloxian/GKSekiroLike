using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float directionFacing = 1;
    public float speed = 4f;
    public float speedUpdate = 0f;
    public float sprintMultiplier = 2f;
    public float parryWindowBase = 0.5f;
    public float parryWindowInit;
    public bool normalBlocking = false;
    public float parryWindow = 0f;
    public float parryStaleInit = 0.35f;
    public float parryStaleAmount = 0.9f;
    public float parryStale = 0f;
    public bool lastParrySuccessful = false;
    public float jumpForce;
    public bool movementBlocked = false;
    public bool actionBlocked = false;
    public float parryActionBlockTimeInit = 0.6f;
    public float parryActionBlockTime;
    public bool grounded = false;
    Rigidbody2D rb;
    float horizontalMove;
    SpriteRenderer playerSprite;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        parryWindowInit = parryWindowBase;
        speedUpdate = speed;
    }

    // Update is called once per frame
    void Update()
    {
        parryWindow -= Time.deltaTime;
        parryStale -= Time.deltaTime;
        parryActionBlockTime -= Time.deltaTime;

        if (parryWindow > 0f)
        {
            playerSprite.color = new Color(239f/255f, 176f/255f, 55f/255f);
        }

        if (Input.GetKeyDown(KeyCode.F) && parryWindow < parryWindowInit - 0.4f)
        {
            if (parryStale > 0f && !lastParrySuccessful)
            {
                parryWindowInit *= parryStaleAmount;
            }
            else
            {
                parryWindowInit = parryWindowBase;
            }
            parryWindow = parryWindowInit;
            parryStale = parryStaleInit;
            parryActionBlockTime = parryActionBlockTimeInit;
        }

        if (Input.GetKey(KeyCode.F) && parryWindow < 0f)
        {
            normalBlocking = true;
            playerSprite.color = new Color((207f / 255f), (102f / 255f), (85f / 255f));
        }
        
        if (Input.GetKeyUp(KeyCode.F) && normalBlocking || !Input.GetKey(KeyCode.F) && parryWindow < 0f)
        {
            playerSprite.color = new Color(147f / 255f, 147f / 255f, 147f / 255f);
            normalBlocking = false;
        }

        
        horizontalMove = Input.GetAxisRaw("Horizontal");
        if (movementBlocked)
        {
            horizontalMove = 0;
        }
        if (parryWindow > 0f || Input.GetKey(KeyCode.F))
        {
            speedUpdate = speed / 2f;
        }
        else
        {
            speedUpdate = speed;
        }

        if (Input.GetKey(KeyCode.LeftShift) && horizontalMove != 0f && !Input.GetKey(KeyCode.F) && parryWindow < 0f)
        {
            speedUpdate = speed * 2f;
        }

        if (grounded)
        {
            rb.AddRelativeForce(Vector3.right * speedUpdate * 2f * horizontalMove);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            directionFacing = -1;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            directionFacing = 1;
        }
        if (directionFacing == -1)
        {
            transform.localScale = Vector3.left + Vector3.up * 2 + Vector3.forward;
        }
        else
        {
            transform.localScale = Vector3.right + Vector3.up * 2 + Vector3.forward;
        }
        if (grounded && rb.velocity.magnitude > speedUpdate && !movementBlocked)
        {
            if (rb.velocity.x < 0f)
            {
                rb.velocity = Vector3.left * speedUpdate;
            }
            else
            {
                rb.velocity = Vector3.right * speedUpdate;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && parryActionBlockTime < 0 && grounded)
        {
            StartCoroutine(JumpCoroutine());
        }
    }

    IEnumerator JumpCoroutine()
    {
        movementBlocked = true;
        yield return new WaitForSeconds(0.1f);
        grounded = false;
        rb.AddRelativeForce(Vector3.up * jumpForce + Vector3.right * rb.velocity.x * 0.6f, ForceMode2D.Impulse);
        movementBlocked = false;

    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }
    }
}
