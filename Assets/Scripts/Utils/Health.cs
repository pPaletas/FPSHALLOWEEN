using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Action onEntityDied; //TODO: CAMBIAR A EVENTOS DE UNITY
    [SerializeField] private float _maxHealth = 100f;

    private float _currentHealth;
    private bool _isPoisoned = false;

    public float CurrentHealth { get => _currentHealth; }

    public void TakeDamage(float damage)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0f, _maxHealth);

        if (_currentHealth <= 0f)
        {
            onEntityDied?.Invoke();
        }
    }

    public void Poison(float poisonDamage, float poisonTime)
    {
        if (!_isPoisoned)
        {
            _isPoisoned = true;
            StartCoroutine(PoisonAsync(poisonDamage, poisonTime));
        }
    }

    public void Heal(float health)
    {
        _currentHealth = Math.Clamp(_currentHealth + health, 0f, _maxHealth);
    }

    private IEnumerator PoisonAsync(float poisonDamage, float poisonTime)
    {
        int currentSeconds = 0;

        while (currentSeconds < poisonTime)
        {
            TakeDamage(poisonDamage);
            yield return new WaitForSeconds(1);
            currentSeconds++;
        }

        _isPoisoned = false;
    }

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }
}