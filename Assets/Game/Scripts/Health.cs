using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    private Character _cc;
    
    private void Awake()
    {
        _cc = GetComponent<Character>();
        currentHealth = maxHealth;
    }
    
    public void ApplyDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Damege Applied: " + damage);
        Debug.Log("Current Health: " + currentHealth);
        checkHealth();
    }

    private void checkHealth()
    {
        if (currentHealth <= 0)
        {
            _cc.SwitchStateTo(Character.CharacterState.Dead);
        }
    }

    public void AddHealth(int health)
    {
        currentHealth += health;
        
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
