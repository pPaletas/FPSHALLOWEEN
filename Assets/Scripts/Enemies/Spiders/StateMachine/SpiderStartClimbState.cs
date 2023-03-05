using UnityEngine;
using UnityEngine.AI;

public struct LinkData
{
    public Vector3 startPoint;
    public Vector3 endPoint;
    public Vector3 startTangentPoint;
    public Vector3 endTangentPoint;
}

public class SpiderStartClimbState : SpiderBaseState
{
    // Target interacion
    private bool _targetReached = false;

    private Endpoint _relativeStart = new Endpoint();
    private Endpoint _relativeEnd = new Endpoint();

    private Quaternion _bezierInitialRotation;
    private LinkData? _currentLinkData;

    private float _bezierCurrentTime = 0f;
    private bool _isClimbing = false;

    public SpiderStartClimbState(SpiderStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.agent.isStopped = true;

        if (stateMachine.isBezierClimbPaused)
        {
            _bezierCurrentTime = stateMachine.cachedBezierTime;
            _relativeStart = stateMachine.relativeStart;
            _relativeEnd = stateMachine.relativeEnd;
        }
    }

    public override void Tick()
    {
        CheckIfCanAttack();
        if (stateMachine.gameObject.activeInHierarchy)
        {
            ClimbEdge();
        }
    }

    public override void Exit()
    {
        // Si el estado no pudo finalizar, entonces se guarda el estado actual
        if (_bezierCurrentTime > 0f)
        {
            stateMachine.isBezierClimbPaused = true;
            stateMachine.cachedBezierTime = _bezierCurrentTime;
            stateMachine.relativeStart = _relativeStart;
            stateMachine.relativeEnd = _relativeEnd;
        }
        else stateMachine.isBezierClimbPaused = false;
    }

    private void CheckIfCanSpit(float distance)
    {
        Vector3 plrPos = stateMachine.currentTarget.position;
        Vector3 plrDir = (plrPos - stateMachine.transform.position).normalized;
        float angle = Mathf.Acos(Vector3.Dot(plrDir, stateMachine.transform.forward)) * Mathf.Rad2Deg;

        bool isTargetCloseEnough = distance <= stateMachine.spitMaxDistance;
        bool isInAngle = angle <= stateMachine.attackAngle;

        if (isTargetCloseEnough && isInAngle)
        {
            _targetReached = true;
            stateMachine.attackCooldown.StartTimer();
            stateMachine.SetState(new SpiderSpitState(stateMachine));
        }
    }

    protected void CheckIfCanAttack()
    {
        float targetDistance = (stateMachine.currentTarget.position - stateMachine.transform.position).magnitude;
        LayerMask groundLayer = stateMachine.groundLayer;

        bool isTargetCloseEnough = targetDistance <= stateMachine.attackDistance;
        bool isCooldownStopped = stateMachine.attackCooldown.IsStopped || stateMachine.isInRushMode;
        bool theresSomethingBetween = Physics.Linecast(stateMachine.transform.position, stateMachine.currentTarget.position, groundLayer);

        bool isSpitter = stateMachine.spiderType == SpiderStateMachine.SpiderType.Spitter;

        if (isTargetCloseEnough && !theresSomethingBetween)
        {
            if (!stateMachine.isInRushMode)
            {
                _targetReached = true;

                if (isCooldownStopped)
                {
                    stateMachine.agent.updateRotation = false;
                    stateMachine.attackCooldown.StartTimer();
                    stateMachine.SetState(new SpiderAttackState(stateMachine));
                }
            }
            else
            {
                stateMachine.Detonate();
            }
        }
        else if (isSpitter && !_targetReached && targetDistance > stateMachine.attackDistance)
        {
            CheckIfCanSpit(targetDistance);
        }
        else
        {
            _targetReached = false;
        }
    }

    private void MoveAlongTheCurve(BezierLink bLink)
    {
        Vector3 newPos = bLink.GetPositionAtTime(_currentLinkData.Value, _bezierCurrentTime);
        stateMachine.transform.position = newPos;
    }

    private void ClimbEdge()
    {
        if (_targetReached) return;

        OffMeshLinkData data = stateMachine.agent.currentOffMeshLinkData;
        BezierLink bLink;
        bool isbLink = stateMachine.agent.currentOffMeshLinkData.offMeshLink.TryGetComponent<BezierLink>(out bLink);

        if (isbLink)
        {
            if (!_isClimbing)
            {
                // Gets start and end relative to agent

                if (!stateMachine.isBezierClimbPaused) bLink.GetStartAndEndLink(stateMachine.transform.position, ref _relativeStart, ref _relativeEnd);

                _currentLinkData = new LinkData
                {
                    startPoint = stateMachine.transform.position,
                    endPoint = _relativeEnd.point.position,
                    startTangentPoint = _relativeStart.tangentPoint.position,
                    endTangentPoint = _relativeEnd.tangentPoint.position
                };

                _isClimbing = true;

                // Rotates initial rotation slightly to prevent unwanted rotation
                Quaternion rotation = Quaternion.AngleAxis(1f, -_relativeStart.point.right);
                _bezierInitialRotation = rotation * stateMachine.transform.rotation;
            }

            // Moving along the curve process
            if (_bezierCurrentTime < 1f)
            {
                MoveAlongTheCurve(bLink);
                RotateSmoothly(_bezierInitialRotation, _relativeEnd.point, _bezierCurrentTime);

                float deltaTime = Time.deltaTime * stateMachine.agent.speed;
                float bezierDelta = deltaTime / bLink.GetLength(_currentLinkData.Value.startPoint);
                _bezierCurrentTime += bezierDelta;
            }
            else
            {
                stateMachine.agent.CompleteOffMeshLink();
                //Smoothly transition between velocities
                stateMachine.agent.velocity = _relativeEnd.point.forward * stateMachine.agent.speed;
                RotateSmoothly(_bezierInitialRotation, _relativeEnd.point, _bezierCurrentTime);
                MoveAlongTheCurve(bLink);

                _bezierCurrentTime = 0f;
                stateMachine.agent.updateRotation = true;
                stateMachine.SetState(new SpiderFollowState(stateMachine));
            }
        }
    }

    private void RotateSmoothly(Quaternion initialRot, Transform targetTransform, float time)
    {
        Quaternion target = Quaternion.LookRotation(targetTransform.forward, targetTransform.up);
        Quaternion currentRot = Quaternion.Slerp(initialRot, target, time);

        stateMachine.transform.rotation = currentRot;
    }
}