using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class DeathPrototype : MonoBehaviour
{
    Health health;
    FirstPersonController fps;
    GameObject deathScreen;

    private void Awake()
    {
        health = GetComponent<Health>();
        deathScreen = GameObject.Find("DeathScreen");
        deathScreen.SetActive(false);

        fps = GetComponent<FirstPersonController>();

        health.onEntityDied += DiedL;
    }

    private void DiedL()
    {
        fps.MoveSpeed = 0f;
        fps.JumpHeight = 0f;
        deathScreen.SetActive(true);
    }
}