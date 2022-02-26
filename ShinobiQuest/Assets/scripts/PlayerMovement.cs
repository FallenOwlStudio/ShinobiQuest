using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //variables

    //stats
    public float moveSpeed;
    public float jumpForce;

    //forces
    public Rigidbody2D rb;
    public bool isGrounded = false;

    //gestion de saut
    public Transform groundCheckLeft;
    public Transform groundCheckRight;


    //autres
    private Vector3 velocity = Vector3.zero;
    public bool isJumping = false;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapArea(groundCheckLeft.position, groundCheckRight.position);

        float horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        
        if(Input.GetButtonDown("Jump") && isGrounded == true)
        {
            isJumping = true;
        }

        MovePlayer(horizontalMovement);

    }

    void MovePlayer(float _horizontalMovement)
    {
        Vector3 targetVelocity = new Vector2(_horizontalMovement, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);
        if(isJumping ==true)
        {
        
            rb.AddForce(new Vector2(0f, jumpForce));
            isJumping = false;
        }
    }

}