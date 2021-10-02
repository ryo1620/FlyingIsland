
using UnityEngine;

public class EndUIManager : MonoBehaviour
{
    // 表示させるUIを取得
    public GameObject reviewPanel;

    void Start()
    {

    }

    public void OnEndBackground()
    {
        TitleSEManager.Instance.SoundSelect();
        reviewPanel.SetActive(true);
    }

    public void OnDoReview()
    {
        TitleSEManager.Instance.SoundSelect();
        StoreReviewManager.Instance.RequestReview();
    }

    public void OnDoNotReview()
    {
        TitleSEManager.Instance.SoundStart();
        EndFadeManager.Instance.FadeOutScene();
    }
}
