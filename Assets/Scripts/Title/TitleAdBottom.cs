using System;
using UnityEngine;
using GoogleMobileAds.Api;

// Gameシーン開始時、アダプティブバナーの表示にやや時間がかかり、UIの位置調整が遅れてしまう
// そうなると広告を誤ってタップしてしまうなど、ユーザビリティの観点で問題がある
// よって、Titleシーンであらかじめアダプティブバナーをロードしておく
public class TitleAdBottom : SingletonMonoBehaviour<TitleAdBottom>
{
    // シーン切替時にTitleFadeManager.csで値を渡すためpublicにしておく
    public BannerView bannerView;

    // 広告ユニットのIDを設定する
    public string androidAdUnitID;
    public string iosAdUnitID;

    // 一定時間ごとにリロードを行うための変数
    [System.NonSerialized] public bool adLoaded;
    private float elapsedTime;
    public float timeToReload;

    // シーン切替時にTitleFadeManager.csで値を渡すためpublicにしておく
    [System.NonSerialized] public float ratio;
    [System.NonSerialized] public float adHeight;

    // シーンロード後、OnEnableの後に一度だけ呼び出す
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    void Init()
    {
        RequestBanner();
    }

    void Start()
    {
        if (adLoaded == false)
        {
            RequestBanner();
        }
    }

    // 一定時間ごとに広告がロードされているかどうか確認し、ロードされていなければ再試行する
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= timeToReload)
        {
            elapsedTime = 0.0f;
            if (adLoaded == false)
            {
                RequestBanner();
            }
        }
    }

    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitID = androidAdUnitID;
#elif UNITY_IOS
        string adUnitID = iosAdUnitID;        
#endif

        // Clean up banner ad before creating a new one.
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }

        // 縦画面におけるアダプティブバナーのサイズを取得する
        AdSize adaptiveSize =
                AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        this.bannerView = new BannerView(adUnitID, adaptiveSize, AdPosition.Bottom);

        // Register for ad events.
        this.bannerView.OnAdLoaded += this.HandleAdLoaded;
        this.bannerView.OnAdFailedToLoad += this.HandleAdFailedToLoad;
        // this.bannerView.OnAdOpening += this.HandleAdOpened;
        // this.bannerView.OnAdClosed += this.HandleAdClosed;
        // this.bannerView.OnAdLeavingApplication += this.HandleAdLeftApplication;        

        // Create an empty ad request.
        AdRequest adRequest = new AdRequest.Builder().Build();

        // Load a banner ad.
        this.bannerView.LoadAd(adRequest);

        // タイトル画面では広告を非表示にする
        if (this.bannerView != null)
        {
            this.bannerView.Hide();
        }
    }

    #region Banner callback handlers

    // アダプティブバナーのロードが完了したときに実行される関数
    // シーン切替時の処理を少しでも軽くするため、ここでadHeightまでは計算しておく
    public void HandleAdLoaded(object sender, EventArgs args)
    {
        // GetHeight(Width)InPixelsメソッドはデバイス本来の解像度を基準にした値を取得する        
        // 実際の画面幅をデバイス本来の横の解像度で割ることで、アダプティブバナーの縮小率を求める
        ratio = (float)Screen.width / MaxResolution.GetDefaultWidth();

        // アダプティブバナーの高さを取得し、先ほど計算した縮小率を掛ける
        adHeight = this.bannerView.GetHeightInPixels() * ratio;

        // 一定時間ごとにリロードを行うときに判定するための変数
        // シーン切替時にも処理を行うかどうかの条件に使う
        adLoaded = true;
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        // 一定時間ごとにリロードを行うときに判定するための変数
        // シーン切替時にも処理を行うかどうかの条件に使う
        adLoaded = false;
    }

    /*
    public void HandleAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }
    */

    /*
    public void HandleAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }
    */

    /*
    public void HandleAdLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeftApplication event received");
    }
    */

    #endregion
}
