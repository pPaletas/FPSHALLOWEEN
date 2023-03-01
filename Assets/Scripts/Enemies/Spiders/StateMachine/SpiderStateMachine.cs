using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderStateMachine : MonoBehaviour
{
    #region StatesVariables
    public float checkRadius = 5f;

    // Variables externas
    [HideInInspector] public GameManager gameManager;

    // Variables hijas
    [HideInInspector] public Animator animator;

    // Variables de animación
    [HideInInspector] public int anim_standUpHash;

    private const string ANIM_STAND_UP = "StandUp";
    #endregion

    private SpiderBaseState _currentState;

    public void SetState(SpiderBaseState newState)
    {
        _currentState.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    // Metodo usado para las variables de animación
    private void TurnStringToHash()
    {
        anim_standUpHash = Animator.StringToHash(ANIM_STAND_UP);
    }

    private void Awake()
    {
        // Variables externas
        gameManager = FindObjectOfType<GameManager>();

        // Variables hijas
        animator = GetComponentInChildren<Animator>();

        TurnStringToHash();

        _currentState = new SpiderIdleState(this);
    }

    private void Update()
    {
        _currentState.Tick();
    }
}