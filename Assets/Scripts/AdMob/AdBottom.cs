using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdBottom : MonoBehaviour
{
    // Titleシーンからの切替時にTitleFadeManager.csで呼び出されるのでpublicにしておく
    public BannerView bannerView;

    // 広告ユニットのIDを設定する
    public string androidAdUnitID;
    public string iosAdUnitID;

    // 位置変更用にメニューバーとアイテムBoxのゲームオブジェクトを取得する
    public GameObject uiBottom;

    // 一定時間ごとにリロードを行うための変数
    [System.NonSerialized] public bool adLoaded;
    private float elapsedTime;
    public float timeToReload;

    void Start()
    {
        // Titleシーンでロードされていなければロードする
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

    public void Destroy()
    {
        this.bannerView.Destroy();
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
    }

    #region Banner callback handlers

    // アダプティブバナーのロードが完了したときに実行される関数
    // UIのy座標をアダプティブバナーの高さ分ずらす
    public void HandleAdLoaded(object sender, EventArgs args)
    {
        // GetHeight(Width)InPixelsメソッドはデバイス本来の解像度を基準にした値を取得する        
        // 実際の画面幅をデバイス本来の横の解像度で割ることで、アダプティブバナーの縮小率を求める
        float ratio = (float)Screen.width / MaxResolution.GetDefaultWidth();

        // アダプティブバナーの高さを取得し、先ほど計算した縮小率を掛ける
        float adHeight = this.bannerView.GetHeightInPixels() * ratio;

        // UIのRect Transformを取得する
        RectTransform myTransform = uiBottom.GetComponent<RectTransform>();

        // offsetMinはxがleft、yがbottomに相当する
        Vector2 Pos = myTransform.offsetMin;

        if (Math.Abs(Pos.y) < adHeight)
        {
            // UIのy座標をアダプティブバナーの高さ分ずらす                   
            Pos.y += adHeight;

            // UIの座標を設定
            myTransform.offsetMin = Pos;
        }

        // 一定時間ごとにリロードを行うときに判定するための変数
        adLoaded = true;
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        // 一定時間ごとにリロードを行うときに判定するための変数
        adLoaded = false;

        // 広告の読み込みに失敗してから非表示になるまでタイムラグがあるので隠しておく
        this.bannerView.Hide();

        // 以下、ずらしたUIを元の位置に戻す処理        
        RectTransform myTransform = uiBottom.GetComponent<RectTransform>();

        Vector2 Pos = myTransform.offsetMin;

        if (Math.Abs(Pos.y) != 0.0f)
        {
            Pos.y = 0.0f;

            myTransform.offsetMin = Pos;
        }
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
