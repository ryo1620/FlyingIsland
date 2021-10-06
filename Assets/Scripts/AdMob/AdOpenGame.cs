using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdOpenGame : MonoBehaviour
{
    // 広告ユニットのIDを設定する
    public string androidAdUnitID;
    public string iosAdUnitID;

    // 一定時間ごとにリロードを行うための変数
    private float elapsedTime;
    public float timeToReload;

    private static AdOpenGame instance;

    private AppOpenAd ad;

    private bool isShowingAd = false;

    public void Start()
    {
        // Load an app open ad when the scene starts
        LoadAd();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= timeToReload)
        {
            elapsedTime = 0.0f;
            if (!IsAdAvailable)
            {
                LoadAd();
            }
        }
    }

    public void OnApplicationPause(bool paused)
    {
        bool hintAdShown = AdHint.Instance.adShown;

        // Display the app open ad when the app is foregrounded
        // リワード広告の表示後にインタースティシャル広告を表示しないようにする
        if (!paused && hintAdShown == false)
        {
            ShowAdIfAvailable();
        }
    }

    public static AdOpenGame Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AdOpenGame();
            }

            return instance;
        }
    }

    private bool IsAdAvailable
    {
        get
        {
            return ad != null;
        }
    }

    public void LoadAd()
    {
#if UNITY_ANDROID
        string adUnitID = androidAdUnitID;
#elif UNITY_IOS
        string adUnitID = iosAdUnitID;        
#endif

        AdRequest request = new AdRequest.Builder().Build();

        // Load an app open ad for portrait orientation
        AppOpenAd.LoadAd(adUnitID, ScreenOrientation.Portrait, request, ((appOpenAd, error) =>
        {
            if (error != null)
            {
                // Handle the error.
                Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
                return;
            }

            // App open ad is loaded.
            ad = appOpenAd;
        }));
    }

    public void ShowAdIfAvailable()
    {
        if (!IsAdAvailable || isShowingAd)
        {
            return;
        }

        ad.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
        ad.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
        ad.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
        // ad.OnAdDidRecordImpression += HandleAdDidRecordImpression;
        // ad.OnPaidEvent += HandlePaidEvent;

        ad.Show();
    }

    private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args)
    {
        // Debug.Log("Closed app open ad");
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        isShowingAd = false;
        // LoadAd();
    }

    private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args)
    {
        // Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        // LoadAd();
    }

    private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args)
    {
        // Debug.Log("Displayed app open ad");
        isShowingAd = true;
    }

    /*
    private void HandleAdDidRecordImpression(object sender, EventArgs args)
    {
        Debug.Log("Recorded ad impression");
    }
    */

    /*
    private void HandlePaidEvent(object sender, AdValueEventArgs args)
    {
        Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
                args.AdValue.CurrencyCode, args.AdValue.Value);
    }
    */
}
