using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerInteractions : MonoBehaviour
{
    private StarterAssetsInputs _input;
    private CharacterAnimationManager m_characterAnimationManager;
    public GameObject grenadeSpawnerEmpty;
    GrenadeSpawner m_grenadeSpawner;
    public GameObject gunScriptHolder;
    Gun m_gun;


    void Start()
    {
        _input = transform.root.GetComponent<StarterAssetsInputs>();
        m_characterAnimationManager = GetComponent<CharacterAnimationManager>();

        if (grenadeSpawnerEmpty != null)
        {
            m_grenadeSpawner = grenadeSpawnerEmpty.GetComponent<GrenadeSpawner>();
        }

        if (gunScriptHolder != null)
        {
            m_gun = gunScriptHolder.GetComponent<Gun>();
        }
    }

    void Update()
    {
        if (_input.throwGrenade)
        {
            if (m_grenadeSpawner != null)
            {
                m_grenadeSpawner.ThrowGrenade();
            }
            _input.throwGrenade = false;
        }

        if (_input.shoot)
        {
            if (m_gun != null)
            {
                if (m_characterAnimationManager.characterAnimator.GetBool("hasShotgun"))
                {
                    m_gun.Shoot(true);
                } 
                else
                {
                    m_gun.Shoot(false);
                }
                m_characterAnimationManager.AnimateShootAnimation();
            } 
        }

        if (_input.shoot == false)
        {
            if (m_gun != null)
            {
                m_characterAnimationManager.StopShootAnimation();
            }
        }

        if(_input.hasSwitchWeapon)
        {
            print("se ordenó switchear");

            if (m_characterAnimationManager.characterAnimator.GetBool("hasShotgun"))
            {
                m_characterAnimationManager.characterAnimator.SetBool("hasShotgun", false);
                m_characterAnimationManager.AnimateWeaponChange();
                m_characterAnimationManager.HideShotGun();
            }
            else
            {
                m_characterAnimationManager.characterAnimator.SetBool("hasShotgun", true);
                m_characterAnimationManager.AnimateWeaponChange();
                m_characterAnimationManager.HideGun();
            }



            _input.hasSwitchWeapon = false;
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

                if (rangeObjects.CompareTag("munition1"))
                { 
                    rangeObjects.gameObject.GetComponent<Animator>().SetTrigger("isOpening");
                    AnimatorClipInfo[] m_CurrentClipInfo = rangeObjects.gameObject.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
                    string m_ClipName = m_CurrentClipInfo[0].clip.name;
                    if (m_ClipName == "ysefue")
                    {
                        Destroy(rangeObjects.gameObject);
                        print("agarro la municion");
                    }
                }
            }
            _input.interact = false;
        }
    }
}
