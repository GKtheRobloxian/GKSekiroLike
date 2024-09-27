using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float directionFacing = 1;
    public float speed = 4f;
    public float speedUpdate = 0f;
    public float sprintMultiplier = 2f;
    public float dashForce = 10f;
    public BoxCollider2D[] allColliders;
    public BoxCollider2D[] collidersOnPlayer;
    BoxCollider2D hitbox;
    public float parryWindowBase = 0.5f;
    public float parryWindowInit;
    public float thrustParryWindow;
    public bool normalBlocking = false;
    public float stuckInBlock = 0f;
    public float parryWindow = 0f;
    public float parryStaleInit = 0.35f;
    public float parryStaleAmount = 0.9f;
    public float parryStale = 0f;
    public bool lastParrySuccessful = false;
    public float jumpForce;
    public bool movementBlocked = false;
    public bool actionBlocked = false;
    bool forceMovementBlock = false;
    bool forceActionBlock = false;
    public bool inHitstun = false;
    public float parryActionBlockTimeInit = 0.6f;
    public float parryActionBlockTime;
    public float parryMovementBlockTime;
    public bool grounded = false;
    Rigidbody2D rb;
    float horizontalMove;
    SpriteRenderer playerSprite;
    // Start is called before the first frame update
    void Start()
    {
        collidersOnPlayer = new BoxCollider2D[2];
        rb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        parryWindowInit = parryWindowBase;
        speedUpdate = speed;
        allColliders = FindObjectsOfType<BoxCollider2D>();
        int count = 0;
        foreach (BoxCollider2D c in allColliders)
        {
            Debug.Log(c.gameObject);
            if (c.gameObject.CompareTag("Player"))
            {
                collidersOnPlayer[count] = c;
                count++;
            }
        }
        Debug.Log(collidersOnPlayer[0]);
        foreach (BoxCollider2D p in collidersOnPlayer)
        {
            if (p.isTrigger)
            {
                hitbox = p;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        thrustParryWindow -= Time.deltaTime;
        stuckInBlock -= Time.deltaTime;
        parryWindow -= Time.deltaTime;
        parryStale -= Time.deltaTime;
        parryActionBlockTime -= Time.deltaTime;

        if (parryWindow > 0f)
        {
            playerSprite.color = new Color(239f/255f, 176f/255f, 55f/255f);
        }

        if (Input.GetKeyDown(KeyCode.F) && parryWindow < parryWindowInit - 0.4f && !actionBlocked)
        {
            if (parryStale > 0f && !lastParrySuccessful)
            {
                parryWindowInit *= parryStaleAmount;
            }
            else
            {
                parryWindowInit = parryWindowBase;
            }
            lastParrySuccessful = false;
            parryWindow = parryWindowInit;
            thrustParryWindow = parryWindowInit * 2f / 3f;
            parryStale = parryStaleInit;
            parryActionBlockTime = parryActionBlockTimeInit;
        }

        if (Input.GetKey(KeyCode.F) && parryWindow < 0f && !actionBlocked || stuckInBlock > 0f)
        {
            normalBlocking = true;
            lastParrySuccessful = false;
            playerSprite.color = new Color((207f / 255f), (102f / 255f), (85f / 255f));
        }
        else if (Input.GetKeyUp(KeyCode.F) && normalBlocking && stuckInBlock < 0f || !Input.GetKey(KeyCode.F) && parryWindow < 0f || inHitstun)
        {
            playerSprite.color = new Color(147f / 255f, 147f / 255f, 147f / 255f);
            normalBlocking = false;
        }


        horizontalMove = Input.GetAxisRaw("Horizontal");
        if (movementBlocked)
        {
            horizontalMove = 0;
        }
        if (parryWindow > 0f || Input.GetKey(KeyCode.F) || stuckInBlock > 0f)
        {
            speedUpdate = speed / 2f;
        }
        else
        {
            speedUpdate = speed;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !movementBlocked && grounded)
        {
            StartCoroutine(WaitForDash());
            if (directionFacing == 1)
            {
                rb.AddForce(Vector3.right * dashForce, ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(Vector3.left * dashForce, ForceMode2D.Impulse);
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && horizontalMove != 0f && !normalBlocking && parryWindow < 0f)
        {
            speedUpdate = speed * 2f;
        }

        if (grounded && !movementBlocked)
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
            transform.localRotation = Quaternion.Euler(Vector3.up * 180f);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        if (grounded && rb.velocity.magnitude > speedUpdate && horizontalMove != 0 && !movementBlocked)
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
        if (Input.GetKeyDown(KeyCode.Space) && parryActionBlockTime < 0 && grounded && !movementBlocked && stuckInBlock < 0f)
        {
            StartCoroutine(JumpCoroutine());
        }

        if (inHitstun)
        {
            normalBlocking = false;
            forceMovementBlock = false;
            forceActionBlock = false;
            movementBlocked = true;
            actionBlocked = true;
            parryWindow = 0;
        }
        else
        {
            movementBlocked = false;
            actionBlocked = false;
        }

        if (forceMovementBlock)
        {
            movementBlocked = true;
        }

        if (forceActionBlock)
        {
            actionBlocked = true;
        }
    }

    IEnumerator JumpCoroutine()
    {
        float speeds = Mathf.Abs(rb.velocity.x);
        movementBlocked = true;
        yield return new WaitForSeconds(0.1f);
        grounded = false;
        rb.AddRelativeForce(Vector2.up * jumpForce + rb.velocity.normalized * Mathf.Pow(speeds, 0.9f), ForceMode2D.Impulse);
        movementBlocked = false;

    }

    IEnumerator ApplyHitstun (float stunTime, float push)
    {
        inHitstun = true;
        rb.AddForce(push * Vector2.left, ForceMode2D.Impulse);
        yield return new WaitForSeconds(stunTime);
        inHitstun = false;
    }

    IEnumerator ParryMovementBlock()
    {
        forceMovementBlock = true;
        yield return new WaitForSeconds(parryMovementBlockTime);
        forceMovementBlock = false;
    }

    IEnumerator WaitForDash()
    {
        forceMovementBlock = true;
        forceActionBlock = true;
        hitbox.enabled = false;
        yield return new WaitForSeconds(0.2f);
        hitbox.enabled = true;
        yield return new WaitForSeconds(0.3f);
        forceMovementBlock = false;
        forceActionBlock = false;
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

    void OnTriggerEnter2D(Collider2D collide)
    {
        if (collide.gameObject.CompareTag("Hitbox"))
        {
            GameObject colliding = collide.gameObject;
            Hitbox hitboxScript = colliding.GetComponent<Hitbox>();
            if (hitboxScript.hitboxType != 2)
            {
                if (parryWindow < 0)
                {
                    if (!normalBlocking)
                    {
                        StartCoroutine(ApplyHitstun(hitboxScript.onHitStun, Mathf.Abs(colliding.transform.position.x - transform.position.x) / (colliding.transform.position.x - transform.position.x) * hitboxScript.hitPushback));
                    }
                    else if (((collide.gameObject.transform.position.x - transform.position.x) > 0 && directionFacing == -1) || ((collide.gameObject.transform.position.x - transform.position.x) < 0 && directionFacing == 1) || hitboxScript.hitboxType == 1)
                    {
                        StartCoroutine(ApplyHitstun(hitboxScript.onHitStun, Mathf.Abs(colliding.transform.position.x - transform.position.x) / (colliding.transform.position.x - transform.position.x) * hitboxScript.hitPushback));
                    }
                    else
                    {
                        stuckInBlock = hitboxScript.onBlockStun;
                        rb.AddForce(Mathf.Abs(colliding.transform.position.x - transform.position.x) / (colliding.transform.position.x - transform.position.x) * hitboxScript.blockPushback * Vector2.left, ForceMode2D.Impulse);
                    }
                }
                else
                {
                    if (((collide.gameObject.transform.position.x - transform.position.x) > 0 && directionFacing == -1) || ((collide.gameObject.transform.position.x - transform.position.x) < 0 && directionFacing == 1))
                    {
                        StartCoroutine(ApplyHitstun(hitboxScript.onHitStun, Mathf.Abs(colliding.transform.position.x - transform.position.x) / (colliding.transform.position.x - transform.position.x) * hitboxScript.hitPushback));
                    }
                    else if (hitboxScript.hitboxType == 1 && thrustParryWindow < 0f)
                    {
                        StartCoroutine(ApplyHitstun(hitboxScript.onHitStun, Mathf.Abs(colliding.transform.position.x - transform.position.x) / (colliding.transform.position.x - transform.position.x) * hitboxScript.hitPushback));
                    }
                    else
                    {
                        lastParrySuccessful = true;
                        parryWindow = 0;
                        parryActionBlockTime = hitboxScript.onParryStun;
                        StartCoroutine(ParryMovementBlock());
                        rb.AddForce(Mathf.Abs(colliding.transform.position.x - transform.position.x) / (colliding.transform.position.x - transform.position.x) * hitboxScript.deflectPushback * Vector2.left, ForceMode2D.Impulse);
                    }
                }
            }
            else
            {
                if (grounded)
                {
                    StartCoroutine(ApplyHitstun(hitboxScript.onHitStun, Mathf.Abs(colliding.transform.position.x - transform.position.x) / (colliding.transform.position.x - transform.position.x) * hitboxScript.hitPushback));
                }
            }
            Destroy(collide.gameObject);
        }
    }
}
