using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyScript : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
        //ne fais rien de spécial pour l'instant
        return;
    }

    public void takeDamage(float damages)
    {
        currentHealth -= damages;
        //mourrir si la santé atteint 0
        if (currentHealth <= 0)
        {
            StartCoroutine(die());

        }
        else//animation de blessure si les dégâts ne sont pas mortels
        {
            animator.SetTrigger("hurt");
        }

    }

    
    //aniamtion de mort et désactivation
    IEnumerator die()
    {
        animator.SetTrigger("die");
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        circle.enabled = false;
        box.enabled = false;
        this.enabled = false;
        yield return null;
    }


    

}
