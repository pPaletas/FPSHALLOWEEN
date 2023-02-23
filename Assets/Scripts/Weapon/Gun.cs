using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;


public class Gun : MonoBehaviour
{
    private StarterAssetsInputs _input;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private GameObject bulletPoint;

    [SerializeField]
    private float bulletSpeed = 1000f;

    void Start()
    {
        _input = transform.root.GetComponent<StarterAssetsInputs>();
    }

    void Update()
    {
        if (_input.shoot)
        {
            Shoot();
            _input.shoot = false;
        }
    }

    void Shoot()
    {
        Debug.Log("shoot");
        GameObject bullet = Instantiate(bulletPrefab, bulletPoint.transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);
        Destroy(bullet, 1);
    }
}
