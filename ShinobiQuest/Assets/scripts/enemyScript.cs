using System.Collections;
using System;
using UnityEngine;
using Pathfinding;

public class enemyScript : MonoBehaviour
{

    // loot
    public GameObject loot;

    //gérer la vie 
    public float currentHealth;
    public float maxHealth;


    public Transform atkPoint;
    public float atkRange;
    private bool canHit;

    private float atkDelay = 0;

    //path components
    public AIDestinationSetter dest;
    public AIPath path;
    //animator
    private Animator animator;
  
    //physics
    private Rigidbody2D rb;
    private CapsuleCollider2D hitbox;
    public LayerMask playerLayer;
    public float previousPosition;
    public float currentPosition;

    // Start is called before the first frame update
    void Start()
    {   //set Health
        currentHealth = maxHealth;
        //attribution des components
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();  
        hitbox = GetComponent<CapsuleCollider2D>();
        previousPosition = transform.localPosition.x;
        currentPosition = transform.localPosition.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(0.07f, 0.07f, 1f);
        } else if (path.desiredVelocity.x <= -0.03f)
        {
            transform.localScale = new Vector3(-0.07f, 0.07f, 1f);
        }
        currentPosition = transform.localPosition.x;

        float speed = Math.Abs((currentPosition - previousPosition)/Time.fixedDeltaTime);

        previousPosition = currentPosition;
        animator.SetFloat("speed", speed);   

        canHit = Physics2D.OverlapCircle(atkPoint.position, atkRange, playerLayer);
        if(atkDelay <= 0)
        {
            if (canHit)
                {
                    StartCoroutine(Attack());
                    rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
            }
        }
        else
        {
            atkDelay -= Time.deltaTime;
        }
        
    }
    /*
    private void FixedUpdate()
    {
        //animator.SetFloat("speed", Math.Abs(rb.velocity.x));
        
    }*/

    IEnumerator Attack()
    {
        
        Collider2D player = Physics2D.OverlapCircle(atkPoint.position, atkRange, playerLayer);
        
        if (player != null)
        {
            animator.SetTrigger("atk");
            yield return new WaitForSecondsRealtime(1.0f);
            if (Physics2D.OverlapCircle(atkPoint.position, atkRange, playerLayer))
            {
                yield return new WaitForSecondsRealtime(.5f);
                player.GetComponent<PlayerMovement>().takeDamages(20f);

            }
            //set up delay before next attack
            atkDelay = 1.2f;
            //wait before allow enemy to move again
            yield return new WaitForSeconds(0.2f);
            //re-enanble player tracking
            rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
        
        yield return null;
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
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        path.enabled = false;
        dest.enabled = false;
        hitbox.enabled = false;
        atkDelay = 99;
        
        
        animator.SetTrigger("die");
        
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        yield return new WaitForSeconds(.5f);
        GameObject butin = Instantiate(loot, transform.position + new Vector3(0f, 0.2f, 0f), Quaternion.identity);
        butin.transform.position = transform.position;
        this.enabled = false;
        yield return null;
    }

    private void OnDrawGizmosSelected()
    {
        if (atkPoint == null)
        {
            Debug.Log("No atkPoint found");
        }
        else
        {
            Gizmos.DrawWireSphere(atkPoint.position, atkRange);
        }

    }

    }
