using UnityEngine;

public class LimitFps : MonoBehaviour
{
    void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 50;
    }
}
