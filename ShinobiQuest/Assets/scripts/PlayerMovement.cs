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
    private float atkRate = 0.7f;
    public float nextAtk = 0f;
    public Transform sword;
    public float swordRange = 0.6f;
    public float dashForce;
    public LayerMask enemylayers;
    public float power = 20;
    private bool stayStill = false;

    //forces
    public Rigidbody2D rb;
    private bool isGrounded = false;
    private float horizontalMovement;
    private bool dashing = false;

    //gestion de saut
    public Transform groundCheckLeft;
    public Transform groundCheckRight;


    //autres
    private Vector3 velocity = Vector3.zero;
    private bool isJumping = false;

    //animator
    private Animator animator;
    private Transform transform;
    public Transform dashPoint;
    


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        transform = GetComponent<Transform>();

    }



    void Update()
    {
        if(nextAtk <= 0)
        {
            if(Input.GetKeyDown(KeyCode.C))
            {
                stayStill = true;
                Atk(1);
                
                nextAtk = atkRate;
                

            }
            if(Input.GetKeyDown(KeyCode.V))
            {
                stayStill = true;
                Atk(2);
                dashing = true;
                nextAtk = atkRate;
                

            }
        }else{
            nextAtk -= atkRate*Time.deltaTime;
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

        

        if(stayStill == false){MovePlayer(horizontalMovement);}

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
        
        if(dashing == true)
        {
            //rb.velocity = Vector3.SmoothDamp(transform.position, dashPoint.position, ref velocity, dashSpeed);
            float timeout = 0.2f;
            StartCoroutine(dash(timeout));

            dashing = false;
        }

    }


    void Atk(int id)
    {
        animator.SetInteger("aktID", id);
        animator.SetTrigger("atk"+id);
        stayStill = false;
        Debug.Log("started atk :"+id);
    }

    IEnumerator dash(float timeout)
    {
        
        yield return new WaitForSeconds(timeout);
        Debug.Log('e');
        //transform.position = Vector3.MoveTowards(transform.position, dashPoint.position, dashForce);
        if(transform.localScale.x > 0) 
        {
            rb.AddForce(new Vector3(1, -0.001f, 0) * dashForce);
        } else {
            rb.AddForce(new Vector3(-1f, -0.001f, 0) * dashForce);
        }
        Debug.Log('d');
    }


}