using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    void Awake()
    {
        placePlayer();
    }
    public IEnumerator placePlayer()
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = transform.position;
        GameObject.FindGameObjectWithTag("Player").transform.localRotation = transform.localRotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().currentHealth =100f;
        yield return null;
    }
    void updateCheckpoint()
    {
        transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
    }
}
