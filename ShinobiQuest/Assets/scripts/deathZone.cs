using UnityEngine;

public class deathZone : MonoBehaviour
{
    //accéder aux HP du joueur
    private PlayerMovement player;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collission");
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Joueur décédé");
            player.currentHealth = 0f;
        }else if (collision.gameObject.tag == "fallingPlatform")
        {
            Debug.Log("bridge collapsed");
            collision.gameObject.SetActive(false);
        }

    }
}
