using System;
using UnityEngine;
using GoogleMobileAds.Api;
using System.Runtime.InteropServices;

public class AdMenu : SingletonMonoBehaviour<AdMenu>
{
    private BannerView bannerView;

    // 広告ユニットのIDを設定する
    public string androidAdUnitID;
    public string iosAdUnitID;

    // 一定時間ごとにリロードを行うための変数
    private bool AdLoaded;
    private float elapsedTime;
    public float timeToReload;

    public GameObject menuPanel;

    // RetinaScale.mm内に記述したObjective-C++の関数を定義する
    // IL2CPPによるビルドでエラーが発生するので条件分岐させる
    // 参考：https://www.fixes.pub/program/225023.html
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern float getRetinaScale();
#else
    [DllImport("RetinaScale.mm")]
    private static extern float getRetinaScale();
#endif

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
            if (AdLoaded == false)
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
        Destroy();

#if UNITY_ANDROID
        // このゲームではアスペクト比を保ったまま解像度を下げるスクリプトを使っている
        // AdMobのスクリプトはデバイス本来の解像度基準である
        // よって、デバイス本来の解像度を取得し、pxをdpに変換する（AndroidのAdMobサイズはdp単位）
        float screenHalfWidth = MaxResolution.GetDefaultWidth() / (Screen.dpi / 160.0f) / 2.0f;

        float adHalfWidth = (float)AdSize.MediumRectangle.Width / 2.0f;

        // 広告の左上の位置がx,yに相当し、原点は画面の左上
        // 広告をx軸の中央に配置するために、画面の幅半分から広告の幅半分を引くことでxを求める
        float x = screenHalfWidth - adHalfWidth;

        // 幅のときと同様に、高さもデバイス本来の解像度を取得し、pxをdpに変換する
        float y = MaxResolution.GetDefaultHeight() / (Screen.dpi / 160.0f) / 2.0f;
#elif UNITY_IOS
        // デバイス本来の解像度を取得し、pxをptに変換する（iOSのAdMobサイズはpt単位）
        // Retinaの倍率を取得するために、Objective-C++のプラグインを使う
        float screenHalfWidth = MaxResolution.GetDefaultWidth() / getRetinaScale() / 2.0f;

        float adHalfWidth = (float)AdSize.MediumRectangle.Width / 2.0f;

        float x = screenHalfWidth - adHalfWidth;

        float y = MaxResolution.GetDefaultHeight() / getRetinaScale() / 2.0f;
#endif

        // Create a 300x250 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitID, AdSize.MediumRectangle, (int)x, (int)y);

        // Called when an ad request has successfully loaded.
        this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        // this.bannerView.OnAdOpening += this.HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        // this.bannerView.OnAdClosed += this.HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        // this.bannerView.OnAdLeavingApplication += this.HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);

        // menuPanelが表示されていれば広告を表示し、そうでなければ非表示にする
        if (menuPanel.activeSelf == true)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public void Destroy()
    {
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }
    }

    public void Show()
    {
        if (this.bannerView != null)
        {
            this.bannerView.Show();
        }
    }

    public void Hide()
    {
        if (this.bannerView != null)
        {
            this.bannerView.Hide();
        }
    }

    #region Banner callback handlers

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        // 一定時間ごとにリロードを行うときに判定するための変数
        AdLoaded = true;
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        // 一定時間ごとにリロードを行うときに判定するための変数
        AdLoaded = false;
    }

    /*
    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }
    */

    /*
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }
    */

    /*
    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }
    */

    #endregion
}