using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{

    public float Health = 100f;
    public float Resistance;

    public abstract void Damage(float damage);
      
    public abstract void Die();
}
