using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyScript : Entity
{

    //gérer la vie 
    public float currentHealth;
    public float maxHealth = 100;
    

    //animator
    private Animator animator;
  
    //physics
    private Rigidbody2D rb;
    private BoxCollider2D box;
    private CircleCollider2D circle;

    // Start is called before the first frame update
    void Start()
    {   //set Health
        currentHealth = maxHealth;
        //attribution des components
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();  
        box = GetComponent<BoxCollider2D>();
        circle = GetComponent<CircleCollider2D>();
    }


    void Update()
    {
    }

    public override void Damage(float damage)
    {
        if ((damage - Resistance) < 0) return;
        Health -= (damage-Resistance);
        animator.SetTrigger("hurt");
        if (Health <= 0)
        {
            Die();
        }
    }


    public override void Die()
    {
        animator.SetTrigger("die");
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        circle.enabled = false;
        box.enabled = false;
        this.enabled = false;
    }
}
