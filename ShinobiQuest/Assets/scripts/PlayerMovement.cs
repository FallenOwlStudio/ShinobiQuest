using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using System.Collections;

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


     ///////////
    //attaque//
   ///////////
    //général
    private float atkRate = 0.5f;
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
    public loadScene loader;



    //forces et physique
    public Rigidbody2D rb;
    private BoxCollider2D box;
    private CircleCollider2D circle;
    private bool isGrounded = false;
    private float horizontalMovement;
    

    //gestion de saut
    public Transform groundCheck;
    public float groundCheckArea;
    public LayerMask ground;



    //autres
    private Vector3 velocity = Vector3.zero;
    public loadScene loadScene;

    //animator
    private Animator animator;
    


    // Start is called before the first frame update
    void Start()
    {
        //attribution des components
        animator = GetComponent<Animator>();
        trans = GetComponent<Transform>();
        circle = GetComponent<CircleCollider2D>();
        box = GetComponent<BoxCollider2D>();
        healthFilling = GameObject.FindGameObjectWithTag("hpFill").GetComponent<Image>();
        healthSlider = GameObject.FindGameObjectWithTag("hpContainer").GetComponent <Slider>();
        energySlider = GameObject.FindGameObjectWithTag("eContainer").GetComponent<Slider>();
        setEnergy(maxEnergy);
        setHealth(maxHealth);

    }

    //régler la santé
    void setHealth(float health)
    {
        currentHealth = health;
    }
    //régler l'énergie
    void setEnergy(float energy)
    {
        currentEnergy = energy;
    }

    void Update()
    {

        //savoir si on saute 
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckArea, ground);
        if (Input.GetButtonDown("Jump") && isGrounded == true)
        {
            isJumping = true;
        }
        //savoir si on se déplace
        horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;

        //tester les dégâts
        if (Input.GetKeyDown(KeyCode.F))
        {
            takeDamages(10f);
        }

        //système d'attaque
        if(currentEnergy < 100)
        {
            //recharge de l'énergie au fil du temps
            currentEnergy += 0.01f;
        }
        //ne pas dépasser la limite
        if(currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
            
        if(nextAtk <= 0)
            {
                if(Input.GetKeyDown(KeyCode.C))
                {
                    stayStill = true;
                    Atk(1);
                    StartCoroutine(swordAtk());
                    nextAtk = 0.5f;
                }
                if(Input.GetKeyDown(KeyCode.V)&&currentEnergy >= 50)
                {
                    stayStill = true;
                    Atk(2);
                    dashing = true;
                    nextAtk = 0.75f;
                    currentEnergy -= 50;
            
                }
            }else{
                nextAtk -= atkRate*Time.fixedDeltaTime;
            }
        //mettre les barres à jour
        energySlider.value = currentEnergy * 100f / maxEnergy;
        healthSlider.value = currentHealth * 100f /maxHealth;
        healthFilling.color = gradient.Evaluate(healthSlider.normalizedValue);

        //mourir  et relancer la scène
        if (currentHealth <= 0.01f)
        {
            currentHealth = 100f;
            Debug.Log("You died");
            trans.localPosition = new Vector3(trans.position.x, trans.position.y - 0.24f, trans.position.z);
            trans.Rotate(0.0f, 0.0f, 90.0f, Space.Self);
            StartCoroutine(loadScene.sceneLoader(1, SceneManager.GetActiveScene().name));
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(stayStill == false){MovePlayer(horizontalMovement);}
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
            //rb.velocity = Vector3.SmoothDamp(transform.position, dashPoint.position, ref velocity, dashSpeed);
            float timeout = 0.2f;
            StartCoroutine(dash(timeout));

            dashing = false;
        }

    }


    void Atk(int id)
    {
        animator.SetTrigger("atk"+id);
        stayStill = false;
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
    }

    IEnumerator dash(float timeout)
    {
        
        yield return new WaitForSeconds(timeout);

        //disable physical constraints
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        /*box.enabled = false;    
        circle.enabled = false;*/


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
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        /*box.enabled = true;
        circle.enabled = true;*/

        //hurt enemies
        foreach(Collider2D enemy in enemiesToHit)
        {
            Debug.Log("hit" + enemy.name);
            enemy.GetComponent<enemyScript>().takeDamage(dashDamages);
        }
    }
    //ajuster le cercle d'attaque
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
    void takeDamages(float damages)
    {
        currentHealth -= damages;
        stayStill = true;
        
        animator.SetTrigger("hurt");
    }

}