using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDrop : MonoBehaviour

{
    // éléments du globe
    public GameObject[] elements;
    public Transform transform;
    public  float speed;
    private Animator animator;
    public GameObject holder;

    private void Start()
    {
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        Transform holderPos = holder.GetComponent<Transform>();
        holderPos.Translate(new Vector3(0f, .2f, 0f));
    }

    // quand le joueur ramasse
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            /*foreach (GameObject element in elements)
            {
                //element.SetActive(false);
                

            }*/
            
            StartCoroutine(selfDestroy());

        }

        IEnumerator selfDestroy()
        {
            

            animator.SetTrigger("goAway");
            PlayerMovement player = FindObjectOfType<PlayerMovement>();
            int energyBoost = 30;
            StartCoroutine(player.fillEnergy(energyBoost));

            yield return new WaitForSeconds(2f);
            
            
            
            Debug.Log("looted");
            

            Destroy(holder);
            this.enabled = false;
            
            yield return null;
        }

    }
}
