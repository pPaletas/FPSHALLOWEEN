using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrenadeSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject grenadePrefab;
    [SerializeField]
    private float grenadeSpeed = 500f;

    public void ThrowGrenade()
    {
        GameObject Grenade = Instantiate(grenadePrefab, transform.position, transform.rotation);
        Grenade.GetComponent<Rigidbody>().AddForce(transform.forward * grenadeSpeed);
    }
}
