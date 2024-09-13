using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float directionFacing = 1;
    public float speed = 4f;
    float speedUpdate = 0f;
    public float sprintMultiplier = 2f;
    public float parryWindowBase = 0.5f;
    public float parryWindowInit;
    public bool normalBlocking = false;
    public float parryWindow = 0f;
    public float parryStaleInit = 0.35f;
    public float parryStaleAmount = 0.9f;
    public float parryStale = 0f;
    public bool lastParrySuccessful = false;
    float horizontalMove;
    SpriteRenderer playerSprite;
    // Start is called before the first frame update
    void Start()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        parryWindowInit = parryWindowBase;
        speedUpdate = speed;
    }

    // Update is called once per frame
    void Update()
    {
        parryWindow -= Time.deltaTime;
        parryStale -= Time.deltaTime;

        if (parryWindow > 0f)
        {
            playerSprite.color = new Color(239f/255f, 176f/255f, 55f/255f);
        }

        if (Input.GetKeyDown(KeyCode.F))
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

        horizontalMove = Input.GetAxis("Horizontal");
        if (parryWindow > 0f || Input.GetKey(KeyCode.F))
        {
            speedUpdate = speed / 2f;
        }
        else
        {
            speedUpdate = speed;
        }
        transform.Translate(Vector3.right * speedUpdate * horizontalMove * Time.deltaTime);
    }
}
