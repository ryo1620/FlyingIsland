using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdHint : SingletonMonoBehaviour<AdHint>
{
    private RewardedAd rewardedAd;

    // 広告ユニットのIDを設定する
    public string androidAdUnitID;
    public string iosAdUnitID;

    public GameObject menuPanel;
    public GameObject hintPanel;
    public GameObject dummyPanel;
    public GameObject showHintPanel;
    public GameObject errorPanel;

    // 一定時間ごとにリロードを行うための変数
    private float elapsedTime;
    public float timeToReload;

    // リワード広告が表示中かどうかを判定するための変数
    // インタースティシャル広告との競合を防ぐために使う
    [System.NonSerialized] public bool adShown;

    // 1つのヒントに対してリワード広告がすでに見られたかどうかを判定するための変数
    [System.NonSerialized] public bool wasSeen;

    // BGM設定のon/offを判定するためにUIManagerのゲームオブジェクトを取得する
    public GameObject uiManager;
    // UIManagerのスクリプト用の変数を宣言する
    private UIManager uiManagerScript;

    // BGMを再生・停止するためにBgmのゲームオブジェクトを取得する
    public GameObject bgm;
    // BgmのAudioSource用の変数を宣言する
    AudioSource bgmAudioSource;

    void Start()
    {
        CreateAndLoadRewardedAd();

        uiManagerScript = uiManager.GetComponent<UIManager>();
        bgmAudioSource = bgm.GetComponent<AudioSource>();
    }

    // 一定時間ごとに広告がロードされているかどうか確認し、ロードされていなければ再試行する
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= timeToReload)
        {
            elapsedTime = 0.0f;
            if (!(this.rewardedAd.IsLoaded()))
            {
                CreateAndLoadRewardedAd();
            }
        }
    }

    private void ShowAdMenu()
    {
        // メニュー内広告を表示する
        AdMenu.Instance.Show();
    }

    private void HideAdMenu()
    {
        // メニュー内広告を非表示にする
        AdMenu.Instance.Hide();
    }

    public void Destroy()
    {
        if (this.rewardedAd != null)
        {
            this.rewardedAd.Destroy();
        }
    }

    // UIManager.csで使う関数
    public void SetAdShownToFalse()
    {
        adShown = false;
    }

    // ユーザーが「ヒントを見る」選択をしたときに実行される関数
    public void UserChoseToWatchAd()
    {
        if (this.rewardedAd.IsLoaded())
        {
            adShown = true;
            this.rewardedAd.Show();
            hintPanel.SetActive(false);
            dummyPanel.SetActive(true);
        }
        else
        {
            // 「広告を取得できませんでした～」というメッセージを表示する
            hintPanel.SetActive(false);
            errorPanel.SetActive(true);
        }
    }

    public void CreateAndLoadRewardedAd()
    {
#if UNITY_ANDROID
        string adUnitID = androidAdUnitID;
#elif UNITY_IOS
        string adUnitID = iosAdUnitID;        
#endif

        this.rewardedAd = new RewardedAd(adUnitID);

        // Called when an ad request has successfully loaded.
        // this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        // this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    // 広告イベント用関数
    /*
    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
    }
    */

    /*
    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
    }
    */

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        // 広告を途中で閉じたとき用にメニュー画面を表示しておく        
        menuPanel.SetActive(true);

        bool bgmPlayed = uiManagerScript.bgmPlayed;

        // BGM設定がonになっている場合のみ処理を行う
        if (bgmPlayed == true)
        {
            // BGMを一時停止する            
            bgmAudioSource.Pause();
        }

        // メニュー内広告を表示する
        ShowAdMenu();
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        dummyPanel.SetActive(false);
    }


    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        dummyPanel.SetActive(false);

        // 広告を閉じたときに次のリワード広告をプリロードする
        CreateAndLoadRewardedAd();

        bool bgmPlayed = uiManagerScript.bgmPlayed;

        // BGM設定がonになっている場合のみ処理を行う
        if (bgmPlayed == true)
        {
            // BGMの一時停止を解除する            
            bgmAudioSource.UnPause();
        }
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        // メニュー内広告を非表示にする
        HideAdMenu();

        // ヒントパネルを表示する
        menuPanel.SetActive(false);
        showHintPanel.SetActive(true);

        // もう一度同じヒントを見るときに再びリワード広告が表示されないよう変数を操作する
        wasSeen = true;
    }
}