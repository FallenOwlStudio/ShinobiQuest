
using UnityEngine;

public class DoNotDestroyOnLoad : MonoBehaviour
{
    //éléments à garder au changement de scènes
    public GameObject[] objects;
    void Awake()
    {
        foreach (var objetc  in objects)
        {
            DontDestroyOnLoad(objetc);
        }
    }
}
