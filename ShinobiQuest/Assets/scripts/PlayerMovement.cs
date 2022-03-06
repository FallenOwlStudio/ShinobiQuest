using UnityEngine;
using System;
using System.Threading;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    //variables

    //stats
    public float moveSpeed;
    public float jumpForce;

    //forces
    public Rigidbody2D rb;
    public bool isGrounded = false;
    public bool isAttacking = false;
    private float horizontalMovement;

    //gestion de saut
    public Transform groundCheckLeft;
    public Transform groundCheckRight;


    //autres
    private Vector3 velocity = Vector3.zero;
    public bool isJumping = false;

    //animator
    private Animator animator;
    private Transform transform;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        transform = GetComponent<Transform>();

    }



    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C) && isAttacking == false)
        {
            isAttacking = true;
            Debug.Log("triggered");
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {

        horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        
        isGrounded = Physics2D.OverlapArea(groundCheckLeft.position, groundCheckRight.position);

        if(Input.GetButtonDown("Jump") && isGrounded == true)
        {
            isJumping = true;
        }

        if(isAttacking == true) 
        {
            horizontalMovement = 0f;
        }

        MovePlayer(horizontalMovement);

    }

    void MovePlayer(float _horizontalMovement)
    {
        Vector3 targetVelocity = new Vector2(_horizontalMovement, rb.velocity.y);
        
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);
        animator.SetFloat("Speed", Math.Abs(rb.velocity.x));
        if(rb.velocity.x < -0.03f)
        {
            transform.localScale = new Vector3(-0.3f, 0.3f, 1);
        }

        if(rb.velocity.x > 0.03f)
        {
            transform.localScale = new Vector3(0.3f, 0.3f, 1);
        }

        if(isJumping ==true)
        {
        
            rb.AddForce(new Vector2(0f, jumpForce));
            isJumping = false;
        }

        if(isGrounded == false) {
            {
                animator.SetBool("isJumping", true);
            }
        }else{
            animator.SetBool("isJumping", false);
        }
        
        
        if(isAttacking == true)
        {
            
            animator.SetBool("isAttacking", true);
            StartCoroutine(wait(0.35f));
            
        }
    }


    IEnumerator wait(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSecondsRealtime(time);
        isAttacking = false;
        Debug.Log("done");
        animator.SetBool("isAttacking", false);
    }

}