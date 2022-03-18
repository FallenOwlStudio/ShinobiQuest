using System.Collections;
using UnityEngine;

public class fallingPlatform : MonoBehaviour
{
    //components
    private Rigidbody2D rb;
    private BoxCollider2D box;
    // Start is called before the first frame update
    void Start()
    {
        //obtenir les components
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collission");
        if(collision.gameObject.tag == "Player")
        {
            StartCoroutine(Tremble());
        }/*else if(collision.gameObject.tag == "tilemap"){
            box.enabled = false;
            this.gameObject.SetActive(false);
            this.enabled = false;*/
        
    }

    void OntriggerEnter2D(Collider2D collision)
    {
    if(collision.gameObject.tag == "deathZone")
    {
        box.enabled = false;
        this.gameObject.SetActive(false);
        this.enabled = false;
    }
    }

    IEnumerator Tremble()
    {
        //tremble avant de tomber
        for (int i = 0; i < 10; i++)
        {
            transform.localPosition += new Vector3(0, 2f, 0);
            yield return new WaitForSeconds(0.05f);
            transform.localPosition -= new Vector3(0, 2f, 0);
            yield return new WaitForSeconds(0.05f);
        }
        //amorcer la chute
        rb.constraints = RigidbodyConstraints2D.None;
    }

}
