using UnityEngine;

public class TitleFadeIn : MonoBehaviour
{
    void Start()
    {
        TitleFadeManager.Instance.FadeInScene();
    }
}