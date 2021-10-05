
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
        DestroyAds();
        TitleSEManager.Instance.SoundStart();
        EndFadeManager.Instance.FadeOutScene();
    }

    // AdMobの広告を削除する
    void DestroyAds()
    {
        EndAdTop.Instance.Destroy();
        EndAdBottom.Instance.Destroy();
    }
}
