using UnityEngine;

public class deathZone : MonoBehaviour
{
    //accéder aux HP du joueur
    public PlayerMovement player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collission");
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Joueur décédé");
            player.currentHealth = 0f;
        }

    }
}
