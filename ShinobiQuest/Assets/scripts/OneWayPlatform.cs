using System.Collections;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;
    private float waitTime= 0.5f;

    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
    }


    void Update()
    {
        if(waitTime <= 0f)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                StartCoroutine(transversible());
                waitTime = 0.5f;
            }
        }
        else
        {
            waitTime -= Time.deltaTime;
        }
    }


    IEnumerator transversible()
    {
        effector.rotationalOffset = 180f;
        yield return new WaitForSeconds(0.3f);
        effector.rotationalOffset = 0f;
        yield return null;

    }
}
