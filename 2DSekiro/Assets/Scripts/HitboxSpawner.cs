using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxSpawner : MonoBehaviour
{
    public GameObject hitboxToSpawn;
    public float intervals;
    float timeTilSpawn = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeTilSpawn -= Time.deltaTime;
        if (timeTilSpawn < 0)
        {
            GameObject hitbox = Instantiate(hitboxToSpawn, transform.position, Quaternion.identity);
            hitbox.GetComponent<Hitbox>().CreateHitbox(Random.Range(0, 3), 0, 5, 7, 10, Random.Range(0.5f, 0.7f), 0.3f, 1.5f, Vector3.one + Vector3.up * Random.Range(-0.5f, 1f));
            timeTilSpawn = intervals + Random.Range(-1.0f, 1.0f);
        }
    }
}
