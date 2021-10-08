using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// シングルトン化する
public class FadeManager : SingletonMonoBehaviour<FadeManager>
{
    // フェードにかかる時間
    public readonly float fadeInStopTime = 0.6f;
    public readonly float fadeInSceneTime = 0.4f;
    public readonly float fadeOutSceneTime = 0.6f;
    public readonly float fadeInOutPanelTime = 0.2f;
    public readonly float turnUpDownTime = 0.4f; // fadeInOutPanelTimeの2倍    

    // フェードに使った時間
    float fadeDeltaTime = 0;

    // フェードに使う画像
    Image fadeImage;

    // BGMを再生するためにBgmのゲームオブジェクトを取得する
    public GameObject bgm;
    // BgmのAudioSource用の変数を宣言する
    AudioSource bgmAudioSource;

    void Start()
    {
        fadeImage = GetComponent<Image>();
        bgmAudioSource = bgm.GetComponent<AudioSource>();
    }

    IEnumerator FadeInSceneCoroutine()
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

        // Titleシーンで広告がロードされていれば表示させる
        if (AdTop.Instance.adLoaded == true)
        {
            AdTop.Instance.bannerView.Show();
        }

        if (AdBottom.Instance.adLoaded == true)
        {
            AdBottom.Instance.bannerView.Show();
        }

        fadeImage.enabled = false;
    }

    IEnumerator FadeInPanelCoroutine()
    {
        // 色の不透明度
        float alpha = 1;
        // Imageの色変更に使う
        Color color = new Color(0, 0, 0, alpha);
        // 初期化
        this.fadeDeltaTime = 0;
        // 色の初期化（黒）
        fadeImage.color = color;

        do
        {
            // 次フレームで再開する
            yield return null;
            // 時間を加算する
            this.fadeDeltaTime += Time.unscaledDeltaTime;
            // 透明度を決める
            alpha = 1 - (this.fadeDeltaTime / this.fadeInOutPanelTime);

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
        while (this.fadeDeltaTime <= this.fadeInOutPanelTime);

        fadeImage.enabled = false;
    }

    IEnumerator FadeOutPanelCoroutine(float fadeTime)
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
            alpha = this.fadeDeltaTime / fadeTime;

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
        while (this.fadeDeltaTime <= fadeTime);
    }

    // BGMの音量を徐々に上げる
    IEnumerator TurnUpCoroutine()
    {
        while (bgmAudioSource.volume < 1.0f)
        {
            bgmAudioSource.volume += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // BGMの音量を徐々に下げる
    IEnumerator TurnDownCoroutine()
    {
        while (bgmAudioSource.volume > 0.4f)
        {
            bgmAudioSource.volume -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // BGMの音量を徐々に消す
    IEnumerator TurnOffCoroutine()
    {
        while (bgmAudioSource.volume > 0)
        {
            bgmAudioSource.volume -= 0.05f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // 合成後のアイテムをフェードインさせる
    IEnumerator FadeInItemCoroutine(GameObject fadeInItem)
    {
        Image fadeInItemImage = fadeInItem.GetComponent<Image>();
        fadeInItem.SetActive(true);

        // 色の不透明度
        float alpha = 0;
        // Imageの色変更に使う
        Color color = new Color(1, 1, 1, alpha);
        // 初期化
        this.fadeDeltaTime = 0;
        // 色の初期化
        fadeInItemImage.color = color;

        do
        {
            // 次フレームで再開する
            yield return null;
            // 時間を加算する
            this.fadeDeltaTime += Time.unscaledDeltaTime;

            // 透明度を決める
            alpha = this.fadeDeltaTime / this.fadeInOutPanelTime;

            if (alpha > 1)
            {
                // alphaの値を制限する
                alpha = 1;
            }

            // 色の透明度を決める
            color.a = alpha;
            // 色を代入する
            fadeInItemImage.color = color;
        }
        while (this.fadeDeltaTime <= this.fadeInOutPanelTime);
    }

    // 合成前のアイテムをフェードアウトさせる
    IEnumerator FadeOutItemCoroutine(GameObject fadeOutItem)
    {
        Image fadeOutItemImage = fadeOutItem.GetComponent<Image>();

        // 色の不透明度
        float alpha = 1;
        // Imageの色変更に使う
        Color color = new Color(1, 1, 1, alpha);
        // 初期化
        this.fadeDeltaTime = 0;
        // 色の初期化（黒）
        fadeOutItemImage.color = color;

        do
        {
            // 次フレームで再開する
            yield return null;
            // 時間を加算する
            this.fadeDeltaTime += Time.unscaledDeltaTime;
            // 透明度を決める
            alpha = 1 - (this.fadeDeltaTime / this.fadeInOutPanelTime);

            if (alpha < 0)
            {
                // alphaの値を制限する
                alpha = 0;
            }

            // 色の透明度を決める
            color.a = alpha;
            // 色を代入する
            fadeOutItemImage.color = color;
        }
        while (this.fadeDeltaTime <= this.fadeInOutPanelTime);

        fadeOutItem.SetActive(false);
    }

    // ゲーム画面からタイトル画面に戻るときのフェードアウト
    IEnumerator FadeOutTitleCoroutine()
    {
        DestroyAdsToTitle();
        var fadeOut = StartCoroutine(FadeOutPanelCoroutine(fadeOutSceneTime));
        yield return fadeOut;
        SceneManager.LoadScene("Title");
    }

    // エンディングムービーに入るときのフェードアウト
    IEnumerator FadeOutEndingCoroutine()
    {
        DestroyAdsToEnding();
        FadeOutPanel(fadeInOutPanelTime);
        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime + 1.0f);
        SceneManager.LoadScene("Ending");
        // インタースティシャル広告を表示させる
        AdBeforeEnding.Instance.ShowInterstitial();
    }

    // AdMobの広告を削除する（タイトル画面に遷移するとき）
    void DestroyAdsToTitle()
    {
        AdTop.Instance.Destroy();
        AdBottom.Instance.Destroy();
        AdMenu.Instance.Destroy();
        AdHint.Instance.Destroy();
        AdOpenGame.Instance.Destroy();
        AdBeforeEnding.Instance.Destroy();
    }


    // AdMobの広告を削除する（エンディングに遷移するとき）
    void DestroyAdsToEnding()
    {
        AdTop.Instance.Destroy();
        AdBottom.Instance.Destroy();
        AdMenu.Instance.Destroy();
        AdHint.Instance.Destroy();
        AdOpenGame.Instance.Destroy();
    }

    // 外部から呼び出される関数
    public void FadeInScene()
    {
        IEnumerator coroutine = FadeInSceneCoroutine();
        StartCoroutine(coroutine);
    }

    public void FadeInPanel()
    {
        IEnumerator coroutine = FadeInPanelCoroutine();
        StartCoroutine(coroutine);
    }

    public void FadeOutPanel(float fadeTime)
    {
        IEnumerator coroutine = FadeOutPanelCoroutine(fadeTime);
        StartCoroutine(coroutine);
    }

    public void TurnUp()
    {
        IEnumerator coroutine = TurnUpCoroutine();
        StartCoroutine(coroutine);
    }

    public void TurnDown()
    {
        IEnumerator coroutine = TurnDownCoroutine();
        StartCoroutine(coroutine);
    }

    public void TurnOff()
    {
        IEnumerator coroutine = TurnOffCoroutine();
        StartCoroutine(coroutine);
    }

    public void FadeInItem(GameObject fadeInItem)
    {
        IEnumerator coroutine = FadeInItemCoroutine(fadeInItem);
        StartCoroutine(coroutine);
    }

    public void FadeOutItem(GameObject fadeOutItem)
    {
        IEnumerator coroutine = FadeOutItemCoroutine(fadeOutItem);
        StartCoroutine(coroutine);
    }

    public void FadeOutTitle()
    {
        IEnumerator coroutine = FadeOutTitleCoroutine();
        StartCoroutine(coroutine);
    }

    public void FadeOutEnding()
    {
        IEnumerator coroutine = FadeOutEndingCoroutine();
        StartCoroutine(coroutine);
    }
}
