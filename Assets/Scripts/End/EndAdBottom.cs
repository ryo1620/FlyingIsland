using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class EndAdBottom : SingletonMonoBehaviour<EndAdBottom>
{
    BannerView bannerView;

    // 広告ユニットのIDを設定する
    public string androidAdUnitID;
    public string iosAdUnitID;

    // 一定時間ごとにリロードを行うための変数
    [System.NonSerialized] public bool adLoaded;
    private float elapsedTime;
    public float timeToReload;

    void Start()
    {
        RequestBanner();
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
            Destroy();
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

    public void Destroy()
    {
        this.bannerView.Destroy();
    }

    #region Banner callback handlers

    public void HandleAdLoaded(object sender, EventArgs args)
    {
        // 一定時間ごとにリロードを行うときに判定するための変数
        adLoaded = true;
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        // 一定時間ごとにリロードを行うときに判定するための変数        
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
