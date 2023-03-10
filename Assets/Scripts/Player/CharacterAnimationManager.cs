using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationManager : MonoBehaviour
{
    public Animator characterAnimator;

    public GameObject backgun;
    public GameObject bacshotkgun;

    public GameObject handgun;
    public GameObject handshotgun;


    // Start is called before the first frame update
    void Start()
    {
        characterAnimator = this.gameObject.transform.GetChild(0).GetComponent<Animator>();
    }

    public void AnimateWeaponChange()
    {
        characterAnimator.SetTrigger("hasSwitch");
    }

    public void HideGun()
    {
        print(" solo deberia estar activo la escopeta");

        backgun.SetActive(true);
        bacshotkgun.SetActive(false);

        handgun.SetActive(false);
        handshotgun.SetActive(true);
    }

    public void HideShotGun()
    {
        print(" solo deberia estar activo el arma");

        backgun.SetActive(false);
        bacshotkgun.SetActive(true);

        handgun.SetActive(true);
        handshotgun.SetActive(false);
    }


    public void AnimateShootAnimation()
    {
        characterAnimator.SetBool("isShooting", true);
    }

    public void StopShootAnimation()
    {
        characterAnimator.SetBool("isShooting", false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
