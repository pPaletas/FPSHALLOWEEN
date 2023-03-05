using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProrotype : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(_damage);
        }

        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}