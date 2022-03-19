using UnityEngine;

public class LimitFps : MonoBehaviour
{
    //Max FPS number
    public int frameRate;
    void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = frameRate;
    }
}
