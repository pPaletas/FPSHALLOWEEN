using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SpiderLookAtTarget : MonoBehaviour
{
    [SerializeField] private LayerMask _groundMask;
    [SerializeField, Range(0, 1)] private float _smoothness;
    [SerializeField, Range(0, 1)] private float _targetOffset = 1.25f;
    private Rig _headRig;

    private MultiAimConstraint _constraint;
    private SpiderStateMachine _stateMachine;
    private Transform _target;
    private Transform _targetChecker;

    private float _targetWeight = 1f;

    private bool _isActive = false;

    public void SetLookAtActive(bool active)
    {
        _isActive = active;
        if (!_isActive)
        {
            _headRig.weight = 0f;
            _targetWeight = 0f;
        }
    }

    private void CheckIfTargetOnSight()
    {
        Vector3 targetPos = _targetChecker.transform.position + (_stateMachine.currentTarget.up * _targetOffset);

        Vector3 targetDir = Vector3.Normalize(_stateMachine.currentTarget.position - _stateMachine.transform.position);
        float angleDiff = Vector3.Dot(targetDir, _stateMachine.transform.forward);
        angleDiff = Mathf.Acos(angleDiff) * Mathf.Rad2Deg;

        if (/*angleDiff <= _constraint.data.limits.y*/ true)
        {
            _targetWeight = 1f;
        }
        else
        {
            _targetWeight = 0f;
        }

        _headRig.weight = Mathf.Lerp(_headRig.weight, _targetWeight, _smoothness);
    }

    private void SetClosestPlayerAsTarget()
    {
        if (_stateMachine.currentTarget == null) return;
        _target.position = _stateMachine.currentTarget.transform.position;
    }

    private void Awake()
    {
        _constraint = GetComponent<MultiAimConstraint>();
        _headRig = _constraint.transform.GetComponentInParent<Rig>();
        _stateMachine = GetComponentInParent<SpiderStateMachine>();

        _targetChecker = _stateMachine.transform.Find("TargetChecker");

        _target = transform.Find("Target");
        _target.SetParent(null);
    }

    private void Update()
    {
        if (_isActive)
            SetClosestPlayerAsTarget();
    }

    private void LateUpdate()
    {
        if (_isActive)
            CheckIfTargetOnSight();
    }
}
