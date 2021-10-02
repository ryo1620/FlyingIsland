using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GoogleMobileAds.Api;

// シングルトン化する
public class TitleFadeManager : SingletonMonoBehaviour<TitleFadeManager>
{
    // フェードにかかる時間    
    public readonly float fadeInStopTime = 0.6f;
    public readonly float fadeInSceneTime = 0.4f;
    public readonly float fadeOutSceneTime = 0.4f;

    // フェードに使った時間
    float fadeDeltaTime = 0.0f;

    // フェードに使う画像
    Image fadeImage;

    // BGMを再生・停止するためにBgmのゲームオブジェクトを取得する
    public GameObject bgm;
    // BgmのAudioSource用の変数を宣言する
    AudioSource bgmAudioSource;

    // BGM（風音）を再生・停止するためにBgmのゲームオブジェクトを取得する
    public GameObject wind;
    // Bgm（風音）のAudioSource用の変数を宣言する
    AudioSource windAudioSource;

    // シーン切り替え時にデータを渡すためにTitleAdTopとTitleAdBottomのゲームオブジェクトを取得する
    public GameObject titleAdTop;
    public GameObject titleAdBottom;
    // TitleAdTopとTitleAdBottomのスクリプト用の変数を宣言する
    private TitleAdTop titleAdTopScript;
    private TitleAdBottom titleAdBottomScript;

    // シーン切替時の処理で使う変数
    [System.NonSerialized] public bool adTopLoaded;
    [System.NonSerialized] public bool adBottomLoaded;
    public BannerView adTopBannerView;
    public BannerView adBottomBannerView;
    [System.NonSerialized] public float adTopHeight;
    [System.NonSerialized] public float adBottomHeight;

    private void Start()
    {
        fadeImage = GetComponent<Image>();
        bgmAudioSource = bgm.GetComponent<AudioSource>();
        windAudioSource = wind.GetComponent<AudioSource>();
        titleAdTopScript = titleAdTop.GetComponent<TitleAdTop>();
        titleAdBottomScript = titleAdBottom.GetComponent<TitleAdBottom>();
    }

    private IEnumerator FadeInSceneCoroutine()
    {
        // 色の不透明度
        float alpha = 1;
        // Imageの色変更に使う
        Color color = new Color(0, 0, 0, alpha);
        // 初期化
        this.fadeDeltaTime = 0;
        // 色の初期化（黒）
        fadeImage.color = color;

        // フェードイン処理をするまで一定時間止める
        yield return new WaitForSeconds(fadeInStopTime);

        // 設定がオンであればBGMを再生する
        // 1がオン、0がオフ
        int bgmSetting = PlayerPrefs.GetInt("BGM", 1);

        if (bgmSetting == 1)
        {
            bgmAudioSource.Play();
            windAudioSource.Play();
        }

        do
        {
            // 次フレームで再開する
            yield return null;
            // 時間を加算する
            this.fadeDeltaTime += Time.unscaledDeltaTime;
            // 透明度を決める
            alpha = 1 - (this.fadeDeltaTime / this.fadeInSceneTime);

            if (alpha < 0)
            {
                // alphaの値を制限する
                alpha = 0;
            }

            // 色の透明度を決める
            color.a = alpha;
            // 色を代入する
            fadeImage.color = color;
        }
        while (this.fadeDeltaTime <= this.fadeInSceneTime);

        fadeImage.enabled = false;
    }

    private IEnumerator FadeOutSceneCoroutine(string nextScene)
    {
        fadeImage.enabled = true;

        // 色の不透明度
        float alpha = 0;
        // Imageの色変更に使う
        Color color = new Color(0, 0, 0, alpha);
        // 初期化
        this.fadeDeltaTime = 0;
        // 色の初期化
        fadeImage.color = color;

        do
        {
            // 次フレームで再開する
            yield return null;
            // 時間を加算する
            this.fadeDeltaTime += Time.unscaledDeltaTime;
            // 透明度を決める
            alpha = this.fadeDeltaTime / this.fadeOutSceneTime;

            if (alpha > 1)
            {
                // alphaの値を制限する
                alpha = 1;
            }

            // 色の透明度を決める
            color.a = alpha;
            // 色を代入する
            fadeImage.color = color;
        }
        while (this.fadeDeltaTime <= this.fadeOutSceneTime);

        // TitleAdTopスクリプトとTitleAdBottomスクリプトの各変数を代入する
        adTopLoaded = titleAdTopScript.adLoaded;
        adBottomLoaded = titleAdBottomScript.adLoaded;
        adTopBannerView = titleAdTopScript.bannerView;
        adBottomBannerView = titleAdBottomScript.bannerView;
        adTopHeight = titleAdTopScript.adHeight;
        adBottomHeight = titleAdBottomScript.adHeight;

        // イベントに登録
        SceneManager.sceneLoaded += GameSceneLoaded;

        // 指定されたシーンに遷移する
        SceneManager.LoadScene(nextScene);
    }

    // シーン切り替え時に呼ばれる関数
    private void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // Titleシーンでアダプティブバナーをロードできた場合のみ処理を行う
        if (adTopLoaded == true)
        {
            // シーン切り替え後のスクリプトを取得
            var adTopScript = GameObject.FindWithTag("AdTop").GetComponent<AdTop>();

            adTopScript.bannerView = adTopBannerView;

            // シーン切替時にUIの位置調整をしておくことで、ユーザビリティを上げる
            for (int i = 0; i < adTopScript.uiTop.Length; i++)
            {
                // UIのRect Transformを取得する
                RectTransform myTransform = adTopScript.uiTop[i].GetComponent<RectTransform>();

                // offsetMaxはxがright、yがtopに相当する
                Vector2 Pos = myTransform.offsetMax;

                if (Math.Abs(Pos.y) < adTopHeight)
                {
                    // UIのy座標をアダプティブバナーの高さ分ずらす                   
                    Pos.y -= adTopHeight;

                    // UIの座標を設定
                    myTransform.offsetMax = Pos;
                }
            }

            adTopScript.adLoaded = true;
        }

        if (adBottomLoaded == true)
        {
            // シーン切り替え後のスクリプトを取得
            var adBottomScript = GameObject.FindWithTag("AdBottom").GetComponent<AdBottom>();

            adBottomScript.bannerView = adBottomBannerView;

            // UIのRect Transformを取得する
            RectTransform myTransform = adBottomScript.uiBottom.GetComponent<RectTransform>();

            // offsetMinはxがleft、yがbottomに相当する
            Vector2 Pos = myTransform.offsetMin;

            if (Math.Abs(Pos.y) < adBottomHeight)
            {
                // UIのy座標をアダプティブバナーの高さ分ずらす                   
                Pos.y += adBottomHeight;

                // UIの座標を設定
                myTransform.offsetMin = Pos;
            }

            adBottomScript.adLoaded = true;
        }

        // イベントから削除
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

    // 外部から呼び出される関数
    public void FadeInScene()
    {
        IEnumerator coroutine = FadeInSceneCoroutine();
        StartCoroutine(coroutine);
    }

    public void FadeOutScene(string nextScene)
    {
        IEnumerator coroutine = FadeOutSceneCoroutine(nextScene);
        StartCoroutine(coroutine);
    }
}
