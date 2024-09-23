using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float damageDealt = 0;
    public float hitPushback = 5;
    public float blockPushback = 25;
    public float deflectPushback = 100;
    public float onHitStun = 0.5f;
    public float onBlockStun = 0.2f;
    public float onParryStun = 0.3f;
    public Vector3 size = Vector3.one;
    public int hitboxType;
    public Color debugColor;

    public void CreateHitbox(int typeOfHitbox, float damage, float pushOnHit, float pushOnBlock, float pushOnDeflect, float hitstun, float blockstun, float parrystun, Vector3 sizeOfHitbox)
    {
        hitboxType = typeOfHitbox;
        damageDealt = damage;
        hitPushback = pushOnHit;
        blockPushback = pushOnBlock;
        deflectPushback = pushOnDeflect;
        onHitStun = hitstun;
        onBlockStun = blockstun;
        onParryStun = parrystun;
        size = sizeOfHitbox;
    }

    void Start()
    {
        if (hitboxType == 0)
        {
            debugColor = Color.red;
        }
        else if (hitboxType == 1)
        {
            debugColor = Color.green;
            Debug.Log("Thrust Hitbox");
        }
        else
        {
            debugColor = Color.black;
            Debug.Log("Sweep Hitbox");
        }
        GetComponent<SpriteRenderer>().color = debugColor;
        transform.localScale = size;
    }
}
