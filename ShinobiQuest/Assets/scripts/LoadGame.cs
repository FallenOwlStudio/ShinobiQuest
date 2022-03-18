using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LoadGame : MonoBehaviour
{
    //les objets 
    public Slider progressSlider;
    public Text progressText;
    public Animator logoFade;
    public Animator barFade;
    public Animator textFade;
    public Animator progressFade;


    void Start()
    {
        logoFade.SetTrigger("appear");
        Debug.Log("Fade started");
        StartCoroutine(loadTheGame(1, "Didacticiel"));
    }

    IEnumerator loadTheGame(int seconds, string sceneName)
    {
        //attendre la fin de la chute de la chouette
        yield return new WaitForSeconds(seconds);
        logoFade.SetTrigger("fall");
        yield return null;
        Debug.Log("Fall satrted");
        yield return new WaitForSeconds(seconds);

        //apparitions en fondu
        textFade.SetTrigger("appear");
        barFade.SetTrigger("appear");
        progressFade.SetTrigger("appear");
        //attendre la fin de ces animations
        yield return new WaitForSeconds(seconds);

        //lancer le chargement de la scène sans la lancer
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {   
            //récupérer l'avancement et l'afficher
            float progress = Mathf.Clamp01(operation.progress / 0.9f);// * 90f / 100f;
            progressSlider.value += 0.01f;
            float newProgress = progressSlider.value * 100f;
            progressText.text = Convert.ToInt32(newProgress) + "%";
            ;


            //attendre pour plus de confort visuel en fonction de l'avancement
            if (progress < progressSlider.value )
            {
                yield return new WaitForSeconds(0.001f);
            }
            else
            {
                yield return new WaitForSeconds(0.01f);
            }
            

            //vérifier le progrès, charger la scène si terminé
            if (progressSlider.value == 1)
            {
                //lancer la scène
                Debug.Log("done");
                operation.allowSceneActivation = true;
            }


            //passer (ne bloque pas la  coroutine)
            yield return null;
            
        }
    }

}
