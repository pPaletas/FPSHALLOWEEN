using System.Collections.Generic;
using UnityEngine;

public class SpiderFollowState : SpiderBaseState
{
    private bool _targetReached = false;

    public SpiderFollowState(SpiderStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.agent.isStopped = false;
        stateMachine.lookAtTarget.SetLookAtActive(true);
    }

    public override void Tick()
    {
        CheckTransitions();
        FollowTarget();
    }

    public override void Exit()
    {
        base.Exit();
        // Evita que el enemigo siga moviendose cuando ya no está siguiendo al jugador
        stateMachine.agent.isStopped = true;
    }

    private void CheckIfCanSpit(float distance)
    {
        Vector3 plrPos = stateMachine.currentTarget.position;
        Vector3 plrDir = (plrPos - stateMachine.transform.position).normalized;
        float angle = Mathf.Acos(Vector3.Dot(plrDir, stateMachine.transform.forward)) * Mathf.Rad2Deg;

        bool isTargetCloseEnough = distance <= stateMachine.spitMaxDistance;

        if (isTargetCloseEnough)
        {
            _targetReached = true;
            stateMachine.attackCooldown.StartTimer();
            stateMachine.SetState(new SpiderSpitState(stateMachine));
        }
    }

    private void CheckIfCanAttack()
    {
        float targetDistance = (stateMachine.currentTarget.position - stateMachine.transform.position).magnitude;
        LayerMask groundLayer = stateMachine.groundLayer;

        bool isTargetCloseEnough = targetDistance <= stateMachine.attackDistance;
        bool isCooldownStopped = stateMachine.attackCooldown.IsStopped || stateMachine.isInRushMode;
        bool theresSomethingBetween = Physics.Linecast(stateMachine.transform.position, stateMachine.currentTarget.position, groundLayer);

        bool isSpitter = stateMachine.spiderType == SpiderStateMachine.SpiderType.Spitter;

        if (isTargetCloseEnough && isCooldownStopped && !theresSomethingBetween)
        {
            if (!stateMachine.isInRushMode)
            {
                _targetReached = true;
                stateMachine.attackCooldown.StartTimer();
                stateMachine.SetState(new SpiderAttackState(stateMachine));
            }
            else
            {
                stateMachine.Detonate();
            }
        }
        else if (isSpitter && !_targetReached && targetDistance > stateMachine.spitLookAngle)
        {
            CheckIfCanSpit(targetDistance);
        }
    }

    protected virtual void CheckTransitions()
    {
        if (stateMachine.currentTarget == null) return;

        CheckIfCanAttack();

        // Smoothly climbing
        if (stateMachine.agent.isOnOffMeshLink)
        {
            stateMachine.SetState(new SpiderStartClimbState(stateMachine));
        }
    }

    private void FollowTarget()
    {
        if (!_targetReached && stateMachine.currentTarget != null && stateMachine.gameObject.activeInHierarchy)
        {
            stateMachine.agent.SetDestination(stateMachine.currentTarget.position);
        }
    }
}