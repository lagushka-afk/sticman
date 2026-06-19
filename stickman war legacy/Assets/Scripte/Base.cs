using NUnit.Framework.Interfaces;
using UnityEngine;

public class Base : MonoBehaviour
{
    public int MaxHealth = 1000;
    private int CurrentHealth;
    public GameObject baze;
    public GameObject ruins;

    void Start()
    {
        
        CurrentHealth = MaxHealth;
        
    }

    public void TakeDamage(int Damage)
    {
        CurrentHealth -= Damage;
        if (CurrentHealth <= 0) 
        {
            ruins.SetActive(true);  
            baze.SetActive(false);
            
            Debug.Log("База уничтожена!"); 
        }
    }
}