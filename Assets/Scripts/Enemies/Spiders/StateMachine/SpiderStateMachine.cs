using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderStateMachine : MonoBehaviour
{
    private SpiderBaseState _currentState;

    public void SetState(SpiderBaseState newState)
    {
        _currentState.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    private void Update()
    {
        _currentState.Tick();
    }
}