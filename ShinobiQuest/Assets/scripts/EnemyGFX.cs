using UnityEngine;
using Pathfinding;

public class EnemyGFX : MonoBehaviour
{
    public AIPath path;
    

    void Update()
    {
        if(path.desiredVelocity.x >= 0.03f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }else if (path.desiredVelocity.x <=0.03f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
