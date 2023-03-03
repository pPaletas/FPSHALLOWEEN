using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;


public class Gun : MonoBehaviour
{

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private GameObject bulletPoint;

    [SerializeField]
    private float bulletSpeed = 1000f;

    public void Shoot()
    {
        Debug.Log("shoot");
        GameObject bullet = Instantiate(bulletPrefab, bulletPoint.transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);
        Destroy(bullet, 1);
    }
}
