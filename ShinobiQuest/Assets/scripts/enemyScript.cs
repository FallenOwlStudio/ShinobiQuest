using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyScript : MonoBehaviour
{

    //gérer la vie 
    public float currentHealth;
    public float maxHealth;
    

    //animator
    private Animator animator;
  
    //physics
    private Rigidbody2D rb;
    private CapsuleCollider2D hitbox;

    // Start is called before the first frame update
    void Start()
    {   //set Health
        currentHealth = maxHealth;
        //attribution des components
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();  
        hitbox = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //ne fais rien de spécial pour l'instant
        return;
    }

    public void takeDamage(float damages)
    {
        currentHealth -= damages;
        //die if health reaches 0
        if (currentHealth <= 0f)
        {
            StartCoroutine(die());

        }
        else//hurt animation if injuries are not deadly
        {
            animator.SetTrigger("hurt");
        }

    }

    
    //aniamtion de mort et désactivation
    IEnumerator die()
    {
        animator.SetTrigger("die");
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        hitbox.enabled = false;
        this.enabled = false;
        yield return null;
    }


    

}
