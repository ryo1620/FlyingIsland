using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdLeavingGame : MonoBehaviour
{
    private InterstitialAd interstitial;

    // 広告ユニットのIDを設定する
    public string androidAdUnitID;
    public string iosAdUnitID;

    // 一定時間ごとにリロードを行うための変数
    private float elapsedTime;
    public float timeToReload;

    // AdHintのゲームオブジェクトを取得する
    public GameObject adHint;
    // AdHintのスクリプト用の変数を宣言する
    private AdHint adHintScript;

    // BGM設定のon/offを判定するためにUIManagerのゲームオブジェクトを取得する
    public GameObject uiManager;
    // UIManagerのスクリプト用の変数を宣言する
    private UIManager uiManagerScript;

    // BGMを再生・停止するためにBgmのゲームオブジェクトを取得する
    public GameObject bgm;
    // BgmのAudioSource用の変数を宣言する
    AudioSource bgmAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        RequestInterstitial();

        // AdHint（ゲームオブジェクト）のコンポーネント（AdHintスクリプト）を変数に代入する
        adHintScript = adHint.GetComponent<AdHint>();

        uiManagerScript = uiManager.GetComponent<UIManager>();
        bgmAudioSource = bgm.GetComponent<AudioSource>();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= timeToReload)
        {
            elapsedTime = 0.0f;
            if (!(this.interstitial.IsLoaded()))
            {
                RequestInterstitial();
            }
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // 起動時はStartメソッドより先に呼び出されるため、adHintScriptの中身がなくエラーになる
        // よってif文でnullチェックを行う
        if (adHintScript != null)
        {
            bool hintAdShown = adHintScript.adShown;

            // リワード広告の表示後にインタースティシャル広告を表示しないようにする
            if (pauseStatus == false && this.interstitial.IsLoaded() && hintAdShown == false)
            {
                this.interstitial.Show();
            }
        }
    }

    private void RequestInterstitial()
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
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
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

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        bool bgmPlayed = uiManagerScript.bgmPlayed;

        // BGM設定がonになっている場合のみ処理を行う
        if (bgmPlayed == true)
        {
            // BGMを一時停止する            
            bgmAudioSource.Pause();
        }
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        bool bgmPlayed = uiManagerScript.bgmPlayed;

        // BGM設定がonになっている場合のみ処理を行う
        if (bgmPlayed == true)
        {
            // BGMの一時停止を解除する            
            bgmAudioSource.UnPause();
        }
    }

    /*
    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }
    */
}
