using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float delay = 3f;
    float countDown;
    public float radius = 5f;
    public float explosionForce = 70f;

    bool exploded = false;

    // Start is called before the first frame update
    void Start()
    {
        countDown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countDown -= Time.deltaTime;
        
        if(countDown <= 0 & exploded == false)
        {
            Explode();
            exploded = true;
        }
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (var rangeObjects in colliders)
        {
            Rigidbody rb = rangeObjects.GetComponent<Rigidbody>();

            if(rb != null)
            {
                rb.AddExplosionForce(explosionForce * 10,transform.position,radius);
            }
        }
        Destroy(gameObject);
    }
}
