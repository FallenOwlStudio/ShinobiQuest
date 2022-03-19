using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class loadScene : MonoBehaviour
{
    //UI elements
    public Animator fadeSystem;
    public Animator barFade;
    public Animator progressFade;
    public Animator fill;
    public Image fillColor;
    public Slider progressSlider;
    public Text progressText;
    //public Transform player;
    //public Transform checkpoint;

    //load default scene with fade
    void start(string sceneName)
    {
        fadeSystem.SetTrigger("out");

        
        

    }

    //restart scene (unused)
    /*public IEnumerator reload()
    {
        //string currentScene = SceneManager.GetActiveScene().name;
        fadeSystem.SetTrigger("in");
        yield return new WaitForSeconds(1);
        //StartCoroutine(spawner.placePlayer()); var spawner deleted
        yield return new WaitForSeconds(1);
        fadeSystem.SetTrigger("out");
        yield return null;

    }*/


    public IEnumerator sceneLoader(int seconds, string sceneName)
    {
        //fade appear
        yield return new WaitForSeconds(seconds);
        fadeSystem.SetTrigger("in");
        yield return new WaitForSeconds(0.5f);
        fillColor.color = new Color32(206, 65, 74, 255);
        barFade.SetTrigger("appear");
        progressFade.SetTrigger("appear");
        //wait animations end
        yield return new WaitForSeconds(1);

        //start loading WITHOUT lauching
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            //show progress
            float progress = Mathf.Clamp01(operation.progress / 0.9f);// * 90f / 100f;
            progressSlider.value += 0.01f;
            float newProgress = progressSlider.value * 100f;
            progressText.text = Convert.ToInt32(newProgress) + "%";
            ;
            //extra wait for smoother animation (unused)
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


            //load scene and clear ui is progress complete
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


            //return value (because it's IEnumerator)
            yield return null;

        }
    }
}
