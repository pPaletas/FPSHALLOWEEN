using UnityEngine;

public class SpiderSpitState : SpiderBaseState
{
    public SpiderSpitState(SpiderStateMachine stateMachine) : base(stateMachine) { }

    private Rigidbody _projectile;
    private Vector3 _unit;
    private Vector3 _targetPosition;
    private Vector3 _posDifference;
    private Quaternion _targetRotation;
    private float _angle;
    private float _initialVel;

    private bool _cooldown = false;

    public override void Enter()
    {
        stateMachine.animationListener.attack += OnAttack;
        stateMachine.animationListener.attackEnd += OnFinish;
    }

    public override void Tick()
    {
        Spit();
    }

    public override void Exit()
    {
        stateMachine.animationListener.attack -= OnAttack;
        stateMachine.animationListener.attackEnd -= OnFinish;
    }

    private void AdjustRotation()
    {
        Vector3 mouthPos = stateMachine.mouth.position;
        _posDifference = _targetPosition - mouthPos;

        Vector3 difOnPlane = Vector3.ProjectOnPlane(_posDifference, stateMachine.transform.up);
        _unit = difOnPlane.normalized;

        Vector3 fwdOnPlane = Vector3.ProjectOnPlane(stateMachine.transform.forward, stateMachine.transform.up);

        float angleDif = Mathf.Acos(Vector3.Dot(_unit, fwdOnPlane)) * Mathf.Rad2Deg;

        if (angleDif <= stateMachine.spitAngle && stateMachine.spitCooldown.IsStopped)
        {
            stateMachine.animator.SetTrigger(stateMachine.animAttackHash);
            stateMachine.spitCooldown.StartTimer();
        }
        else
        {
            Quaternion targetDir = Quaternion.LookRotation(_unit, stateMachine.transform.up);
            stateMachine.transform.rotation = targetDir;
        }
    }

    private void OnAttack()
    {
        Vector3 offset = -_unit * 2f;
        _targetPosition = stateMachine.currentTarget.position + offset;

        // v0 = sqrt((g * x^2) / (2 * (cos(theta))^2 * (x * tan(theta) - y)))
        float yDif = Mathf.Abs(_posDifference.y);
        // Quitamos y para calcular solo la diferencia en el plano XZ
        _posDifference.y = 0f;
        float xDif = _posDifference.magnitude;

        _angle = stateMachine.spitAngle * Mathf.Deg2Rad;

        float a = stateMachine.spitGravity * Mathf.Pow(xDif, 2f);
        float b = 2 * Mathf.Pow(Mathf.Cos(_angle), 2f) * Mathf.Abs(xDif * Mathf.Tan(_angle) - yDif);
        _initialVel = Mathf.Sqrt(a / b);

        Vector3 dir = Quaternion.AngleAxis(_angle * Mathf.Rad2Deg, Vector3.Cross(_unit, Vector3.up)) * _unit;
        Vector3 initialVel = dir.normalized * _initialVel;

        Quaternion id = Quaternion.identity;
        GameObject _projectileClone = GameObject.Instantiate(stateMachine.spitProjectile, stateMachine.mouth.position, id);
        _projectile = _projectileClone.GetComponent<Rigidbody>();
        _projectile.GetComponent<PoisonousProjectile>().Init(stateMachine, initialVel, _projectile);
    }

    private void OnFinish()
    {
        bool max = _posDifference.magnitude <= stateMachine.spitMaxDistance;
        bool min = _posDifference.magnitude > stateMachine.attackDistance;

        if (max && min) return;

        if (!stateMachine.isBezierClimbPaused)
        {
            stateMachine.SetState(new SpiderFollowState(stateMachine));
        }
        else
        {
            stateMachine.SetState(new SpiderStartClimbState(stateMachine));
        }
    }

    private void Spit()
    {
        AdjustRotation();
    }
}