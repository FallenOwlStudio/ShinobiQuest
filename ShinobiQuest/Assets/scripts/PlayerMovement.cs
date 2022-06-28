using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Threading;

public class PlayerMovement : MonoBehaviour
{
    //variables

    //santé et energie
    public Gradient gradient;
    private Image healthFilling;
    private Slider healthSlider;
    private Slider energySlider;
    public float currentHealth;
    private float maxHealth = 100;
    private float maxEnergy = 100;
    private float currentEnergy;

    //déplacement
    public float moveSpeed;
    public float jumpForce;
    private bool stayStill = false;
    private bool isJumping = false;
    private Transform trans;
    public SpriteRenderer sp;

     ///////////
    //attaque//
   ///////////
    //général
    public float atkRate;
    public float nextAtk = 0f;
    public LayerMask enemylayers;

    //sword 
    public Transform atkPoint;
    public float atkRange;
    public float swordDamages;
    //dash
    public float dashForce;
    public float dashDamages;
    private bool dashing = false;
    public Transform dashPoint;
    private loadScene loader;

    //buttons interractions
    public bool jumpButton = false;
    public bool atkButton = false;
    public bool dashButton = false;

    //forces and physics
    public Rigidbody2D rb;
    private BoxCollider2D box;
    private CircleCollider2D circle;
    private bool isGrounded = false;
    private float horizontalMovement;
    

    //gestion de saut
    public Transform groundCheck;
    public float groundCheckArea;
    public LayerMask ground;


    //camera setup
    public Animator vcams;
    public float yvel;

    //autres
    private Vector3 velocity = Vector3.zero;
    private loadScene loadScene;
    public Transform cinemachine;
    private bool isInvicible = false;


    //animator
    private Animator animator;

    //mobile support
    private bool isMobile;
    public Joystick joystick;
    public Canvas mobileControls;

    // Start is called before the first frame update
    void Start()
    {
        //components attribution
        animator = GetComponent<Animator>();
        trans = GetComponent<Transform>();
        circle = GetComponent<CircleCollider2D>();
        box = GetComponent<BoxCollider2D>();
        healthFilling = GameObject.FindGameObjectWithTag("hpFill").GetComponent<Image>();
        healthSlider = GameObject.FindGameObjectWithTag("hpContainer").GetComponent <Slider>();
        energySlider = GameObject.FindGameObjectWithTag("eContainer").GetComponent<Slider>();
        loader = GameObject.FindGameObjectWithTag("gameManager").GetComponent<loadScene>();
        setEnergy(maxEnergy);
        setHealth(maxHealth);

        //enable or not mobile input system according to the current device
        Debug.Log("Device Type:" + SystemInfo.deviceType);
        if(SystemInfo.deviceType == DeviceType.Handheld)
        {
            isMobile = true;
            Debug.Log("mobile device");
            mobileControls.enabled = true;
        }
        else
        {
            isMobile = false;
            mobileControls.enabled = false;
        }

    }

    //set health
    void setHealth(float health)
    {
        currentHealth = health;
    }
    //set energy
    void setEnergy(float energy)
    {
        currentEnergy = energy;
    }

    public IEnumerator fillEnergy(int energy)
    {
        

        Thread filler = new Thread(() =>{
            Debug.Log(energy);
            for (int i = 0; i < energy; i++)
            {
                currentEnergy += 1;
                Debug.Log("current energy : " + currentEnergy);
                if (currentEnergy > maxEnergy)
                {
                    currentEnergy = maxEnergy;
                }
                Thread.Sleep(50);
            }
            
        });
        
        filler.Start();
        yield return new WaitForSeconds(2);
        filler.Abort();
        yield return null;
    }

    void addEnergy(int energy)
    {
        Debug.Log(energy);
        for (int i = 0; i < energy; i++)
        {
            currentEnergy += 1;
            Debug.Log("current energy : " + currentEnergy);
            if (currentEnergy > maxEnergy)
            {
                currentEnergy = maxEnergy;
            }
            Thread.Sleep(100);
        }
    }

    void Update()
    {

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckArea, ground);
        if ((Input.GetButtonDown("Jump") || jumpButton) && isGrounded == true)
        {
            jumpButton = false;
            isJumping = true;
        }
        

        //get movement inputs
        if(stayStill == false)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            if (isMobile)
            {
                if(joystick.Horizontal <= -0.2)
                {
                    horizontalMovement = -1f * moveSpeed * Time.deltaTime;  
                }else if (joystick.Horizontal >= 0.2)
                {
                    horizontalMovement = 1f * moveSpeed * Time.deltaTime;
                }
                else
                {
                    horizontalMovement = 0f;
                }
            }
            else
            {
                horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            }
        }
        else
        {
            horizontalMovement = 0;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            
        }
            

        //tester les dégâts
        if (Input.GetKeyDown(KeyCode.F))
        {

            stayStill = true;
            takeDamages(20f);
        }

        //charge energy bar
        if(currentEnergy < 100)
        {
            //refill energy along time
            currentEnergy += 0.01f;
        }
        //not overpass energy limit
        if(currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
            
        if(nextAtk <= 0)
            {
                if(Input.GetKeyDown(KeyCode.C) || atkButton)
                {
                atkButton = false; 
                stayStill = true;
                    Atk(1);
                    StartCoroutine(swordAtk());
                    nextAtk = 0.5f;
                }
                if((Input.GetKeyDown(KeyCode.V) || dashButton)&&currentEnergy >= 50)
                {
                    dashButton = false;
                    Atk(2);
                    dashing = true;
                    nextAtk = 0.75f;
                    currentEnergy -= 50;
            
                }
            }else{
                nextAtk -= atkRate*Time.fixedDeltaTime;
            }
        //update bars
        energySlider.value = currentEnergy * 100f / maxEnergy;
        healthSlider.value = currentHealth * 100f /maxHealth;
        healthFilling.color = gradient.Evaluate(healthSlider.normalizedValue);

        //die and restart scene
        if (currentHealth <= 0f)
        {
            currentHealth = 0.1f;
            
            Debug.Log("You died");
            animator.SetTrigger("death");
            stayStill = true;
            mobileControls.enabled = false;
            stayStill = true;
            StartCoroutine(loader.sceneLoader(1, SceneManager.GetActiveScene().name));
            cinemachine.position = trans.position;
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        MovePlayer(horizontalMovement);
    }



    void MovePlayer(float _horizontalMovement)
    {
        Vector3 targetVelocity = new Vector2(_horizontalMovement, rb.velocity.y);
        
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);
        animator.SetFloat("Speed", Math.Abs(rb.velocity.x));
        if(rb.velocity.x < -0.03f)
        {
            trans.localScale = new Vector3(-0.3f, 0.3f, 1);
        }

        if(rb.velocity.x > 0.03f)
        {
            trans.localScale = new Vector3(0.3f, 0.3f, 1);
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
            float timeout = 0.2f;
            StartCoroutine(dash(timeout));

            dashing = false;
        }

    }


    void Atk(int id)
    {
        animator.SetTrigger("atk"+id);
    }

    IEnumerator swordAtk()
    {
        yield return new WaitForSeconds(0.1f);
        //get all enemies in atk range
        Collider2D[] enemiesToHit = Physics2D.OverlapCircleAll(atkPoint.position, atkRange, enemylayers);

        //inflict damages
        foreach (Collider2D enemy in enemiesToHit)
        {
            Debug.Log("hit" + enemy.name + "damages :"+ swordDamages);
            enemy.GetComponent<enemyScript>().takeDamage(swordDamages);
            
        }
        yield return new WaitForSeconds(0.45f);
        stayStill = false;
        yield return null;
    }

    IEnumerator dash(float timeout)
    {
        
        yield return new WaitForSeconds(timeout);

        //disable physics on Y axis
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;



        //get enemies in dash range
        Collider2D[] enemiesToHit = Physics2D.OverlapAreaAll(trans.position, dashPoint.position, enemylayers);
        
        //operate dash
        if(trans.localScale.x > 0) 
        {
            rb.AddForce(new Vector3(1, -0.001f, 0) * dashForce);
        } else {
            rb.AddForce(new Vector3(-1f, -0.001f, 0) * dashForce);
        }
        //re-enable constraints after attack
        stayStill=false;    
        

        //hurt enemies
        foreach(Collider2D enemy in enemiesToHit)
        {
            Debug.Log("hit" + enemy.name);
            enemy.GetComponent<enemyScript>().takeDamage(dashDamages);
        }
    }
    //see gizmo elements
    private void OnDrawGizmosSelected()
    {
        if(atkPoint == null)
        {
            Debug.Log("No atkPoint found");
        }
        else
        {
            Gizmos.DrawWireSphere(atkPoint.position, atkRange);
        }
        if (groundCheck == null)
        {
            Debug.Log("No groundCheck found");
        }
        else
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckArea);
        }

    }


    //prendre des dégâts
     public void takeDamages(float damages)
    {
        if (!isInvicible)
        {
            currentHealth -= damages;
        stayStill = true;
        
        animator.SetTrigger("hurt");
        isInvicible = true;
        StartCoroutine(Flash());
        StartCoroutine(InvicibilityManager());
        }
        
    }

    public IEnumerator Flash()
    {
        while (isInvicible)
        {
            sp.color = new Color(255f, 255, 255f, 0f);
            yield return new WaitForSeconds(0.1f);
            sp.color = new Color(255f, 255, 255f, 255f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator InvicibilityManager()
    {
        yield return new WaitForSeconds(0.5f);
        stayStill = false;
        yield return new WaitForSeconds(1.5f);
        isInvicible = false;
        yield return null;
    }


    //buttons functions for mobile
    public void jumpButtonTriggered()
    {
        isJumping = true;
    }
    public void atkButtonTriggered()
    {
        atkButton = true;
    }
    public void dashButtonTriggered()
    {
        dashButton = true;
    }
}