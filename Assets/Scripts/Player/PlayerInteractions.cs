using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerInteractions : MonoBehaviour
{
    private StarterAssetsInputs _input;
    public GameObject grenadeSpawnerEmpty;
    GrenadeSpawner m_grenadeSpawner;

    void Start()
    {
        _input = transform.root.GetComponent<StarterAssetsInputs>();
        if (grenadeSpawnerEmpty != null)
        {
            m_grenadeSpawner = grenadeSpawnerEmpty.GetComponent<GrenadeSpawner>();
        }
    }

    void Update()
    {
        if (_input.throwGrenade)
        {
            if (m_grenadeSpawner != null)
            {
                m_grenadeSpawner.GetComponent<GrenadeSpawner>().ThrowGrenade();
            }
            _input.throwGrenade = false;
        }

        if (_input.interact)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 3f);
            foreach (var rangeObjects in colliders)
            {
                if (rangeObjects.CompareTag("collectable"))
                {
                    Destroy(rangeObjects.gameObject);
                    print("ha agarrado un colectable");
                }
            }
            _input.interact = false;
        }
    }
}
