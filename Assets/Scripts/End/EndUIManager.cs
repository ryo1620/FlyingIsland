using System.Collections;
using UnityEngine;

public class EndUIManager : MonoBehaviour
{
    // 表示させるUIを取得    
    public GameObject reviewPanel;
    public GameObject returnToTitle;

    public void OnEndBackground()
    {
        TitleSEManager.Instance.SoundSelect();
        reviewPanel.SetActive(true);

        // 自動スリープを有効にする
        Screen.sleepTimeout = SleepTimeout.SystemSetting;

        StartCoroutine(RequestReviewCoroutine());
    }

    // レビュー画面の表示後に「タイトル画面に戻る」ボタンを表示させるコルーチン
    IEnumerator RequestReviewCoroutine()
    {
        // レビュー画面を表示させる
#if UNITY_ANDROID
        var requestReviewAndroid = StartCoroutine(StoreReviewManager.Instance.RequestReviewAndroid());
        yield return requestReviewAndroid;
#elif UNITY_IOS
        var requestReviewIos = UnityEngine.iOS.Device.RequestStoreReview();
        yield return requestReviewIos;
#endif        
        // 「タイトル画面に戻る」ボタンを表示させる
        returnToTitle.SetActive(true);
    }

    public void OnReturnToTitle()
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
