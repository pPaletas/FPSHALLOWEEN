using System.Collections.Generic;
using UnityEngine;

public class SpiderIdleState : SpiderBaseState
{
    private bool _detectedPlayer = false;

    public SpiderIdleState(SpiderStateMachine stateMachine) : base(stateMachine) { }

    public override void Tick()
    {
        base.Tick();

        CheckTransitions();
    }

    private bool CheckForAnyTarget()
    {
        List<GameObject> players = stateMachine.gameManager.Players;

        for (int i = 0; i < players.Count; i++)
        {
            // Revisa la distancia que hay entre la araña, y alguno de los jugadores
            float dist = Vector3.Distance(players[i].transform.position, stateMachine.transform.position);

            if (dist <= stateMachine.checkRadius)
            {
                _detectedPlayer = true;
                return true;
            }
        }

        return false;
    }

    protected virtual void CheckTransitions()
    {
        if (!_detectedPlayer && CheckForAnyTarget())
        {
            stateMachine.animator.SetTrigger(stateMachine.anim_standUpHash);
        }
    }
}

public class SpiderFollowState : SpiderBaseState
{
    public SpiderFollowState(SpiderStateMachine stateMachine) : base(stateMachine)
    {

    }
}