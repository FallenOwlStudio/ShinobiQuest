using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class loadScene : MonoBehaviour
{
    public Animator fadeSystem;
    public Animator barFade;
    public Animator progressFade;
    public Animator fill;
    public Image fillColor;
    public Slider progressSlider;
    public Text progressText;
    public Transform player;
    public Transform checkpoint;
    public Spawner spawner;

    //charger une scene
    void start(string sceneName)
    {
        fadeSystem.SetTrigger("out");
    }

    //relancer la scene
    public IEnumerator reload()
    {
        //string currentScene = SceneManager.GetActiveScene().name;
        fadeSystem.SetTrigger("in");
        yield return new WaitForSeconds(1);
        StartCoroutine(spawner.placePlayer());
        yield return new WaitForSeconds(1);
        fadeSystem.SetTrigger("out");
        yield return null;

    }


    public IEnumerator sceneLoader(int seconds, string sceneName)
    {
        //apparitions en fondu
        fadeSystem.SetTrigger("in");
        yield return new WaitForSeconds(0.5f);
        fillColor.color = new Color32(206, 65, 74, 255);
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

            /*
            //attendre pour plus de confort visuel en fonction de l'avancement
            if (progress < progressSlider.value)
            {
                yield return new WaitForSeconds(0.001f);
            }
            else
            {
                yield return new WaitForSeconds(0.001f);
            }*/


            //vérifier le progrès, charger la scène si terminé
            if (progressSlider.value == 1)
            {
                //lancer la scène
                Debug.Log("done");
                progressFade.SetTrigger("hide");
                barFade.SetTrigger("hide");
                fill.SetTrigger("hide");

                operation.allowSceneActivation = true;

                fadeSystem.SetTrigger("out");
            }


            //passer (ne bloque pas la  coroutine)
            yield return null;

        }
    }
}
