using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdBeforeEnding : SingletonMonoBehaviour<AdBeforeEnding>
{
    private InterstitialAd interstitial;

    // 広告ユニットのIDを設定する
    public string androidAdUnitID;
    public string iosAdUnitID;

    // 一定時間ごとにリロードを行うための変数    
    private float elapsedTime;
    public float timeToReload;

    void Start()
    {
        RequestInterstitial();
    }

    // 一定時間ごとに広告がロードされているかどうか確認し、ロードされていなければ再試行する
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= timeToReload)
        {
            elapsedTime = 0.0f;
            if (this.interstitial.IsLoaded() == false)
            {
                RequestInterstitial();
            }
        }
    }

    public void ShowInterstitial()
    {
        // インタースティシャル広告の読み込みが完了していれば表示させる
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
    }

    public void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitID = androidAdUnitID;
#elif UNITY_IOS
        string adUnitID = iosAdUnitID;        
#endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitID);

        // Called when an ad request has successfully loaded.
        // this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        // this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        // this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        // this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        // this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    /*
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }
    */

    /*
    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }
    */

    /*
    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleOnAdOpened event received");
    }
    */

    /*
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleOnAdClosed event received");
    }
    */

    /*
    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }
    */
}
