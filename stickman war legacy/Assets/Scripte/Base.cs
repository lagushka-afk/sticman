using UnityEngine;

public class Base : MonoBehaviour
{
    public int MaxHealth = 1000;
    private int CurrentHealth;

    void Start() { CurrentHealth = MaxHealth; }

    public void TakeDamage(int Damage)
    {
        CurrentHealth -= Damage;
        if (CurrentHealth <= 0) { Destroy(gameObject); Debug.Log("База уничтожена!"); }
    }
}