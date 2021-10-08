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

    // シーン切替時の処理で使う変数
    [System.NonSerialized] public bool adTopLoaded;
    [System.NonSerialized] public bool adBottomLoaded;
    public BannerView adTopBannerView;
    public BannerView adBottomBannerView;
    [System.NonSerialized] public float adTopHeight;
    [System.NonSerialized] public float adBottomHeight;

    public GameObject nowLoading;

    // 非同期動作で使用するAsyncOperation
    private AsyncOperation async;

    // 読み込み率を表示するスライダー
    public Slider slider;

    private void Start()
    {
        fadeImage = GetComponent<Image>();
        bgmAudioSource = bgm.GetComponent<AudioSource>();
        windAudioSource = wind.GetComponent<AudioSource>();
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

        // NowLoadingを表示
        nowLoading.SetActive(true);

        // 広告をロードするためのコルーチンを開始
        var loadAds = StartCoroutine(LoadAds(3.0f));
        yield return loadAds;
        slider.value = 0.5f;

        // TitleAdTopスクリプトとTitleAdBottomスクリプトの各変数を代入する
        adTopLoaded = TitleAdTop.Instance.adLoaded;
        adBottomLoaded = TitleAdBottom.Instance.adLoaded;
        adTopBannerView = TitleAdTop.Instance.bannerView;
        adBottomBannerView = TitleAdBottom.Instance.bannerView;
        adTopHeight = TitleAdTop.Instance.adHeight;
        adBottomHeight = TitleAdBottom.Instance.adHeight;

        // イベントに登録
        SceneManager.sceneLoaded += GameSceneLoaded;

        // 次のシーンをロードするためのコルーチンを開始
        StartCoroutine(LoadData(nextScene));
    }

    // 一定の秒数が経過するか、AdMobをロードするまでコルーチンの処理が行われる
    IEnumerator LoadAds(float second)
    {
        float progressVal = 0;
        float deltaTime = 0;

        while (!TitleAdTop.Instance.adLoaded || !TitleAdBottom.Instance.adLoaded)
        {
            progressVal += Time.unscaledDeltaTime / second;
            // AdMobのロード中はゲージを半分まで増やす
            slider.value = progressVal / 2;
            yield return null;
            deltaTime += Time.unscaledDeltaTime;
            // 一定の秒数を超えたらコルーチンを強制終了する
            if (deltaTime >= second)
            {
                yield break;
            }
        }
    }

    IEnumerator LoadData(string nextScene)
    {
        // シーンの読み込みをする
        async = SceneManager.LoadSceneAsync(nextScene);

        // 読み込みが終わるまで進捗状況をスライダーの値に反映させる
        while (!async.isDone)
        {
            var progressVal = Mathf.Clamp01(async.progress / 0.9f);
            slider.value = progressVal / 2 + 0.5f;
            yield return null;
        }
    }

    // シーン切り替え時に呼ばれる関数
    private void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // Titleシーンでアダプティブバナーをロードできた場合のみ処理を行う
        if (adTopLoaded == true)
        {
            // シーン切り替え後の広告インスタンスに代入           
            AdTop.Instance.bannerView = adTopBannerView;

            // シーン切替時にUIの位置調整をしておくことで、ユーザビリティを上げる
            for (int i = 0; i < AdTop.Instance.uiTop.Length; i++)
            {
                // UIのRect Transformを取得する
                RectTransform myTransform = AdTop.Instance.uiTop[i].GetComponent<RectTransform>();

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

            AdTop.Instance.adLoaded = true;
        }

        if (adBottomLoaded == true)
        {
            // シーン切り替え後の広告インスタンスに代入      
            AdBottom.Instance.bannerView = adBottomBannerView;

            // UIのRect Transformを取得する
            RectTransform myTransform = AdBottom.Instance.uiBottom.GetComponent<RectTransform>();

            // offsetMinはxがleft、yがbottomに相当する
            Vector2 Pos = myTransform.offsetMin;

            if (Math.Abs(Pos.y) < adBottomHeight)
            {
                // UIのy座標をアダプティブバナーの高さ分ずらす                   
                Pos.y += adBottomHeight;

                // UIの座標を設定
                myTransform.offsetMin = Pos;
            }

            AdBottom.Instance.adLoaded = true;
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
