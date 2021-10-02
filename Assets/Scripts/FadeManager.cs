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

    // フェードイン後に広告を表示させるためにオブジェクトを取得する
    public GameObject adTop;
    public GameObject adBottom;
    // 広告のスクリプト用の変数を宣言する
    AdTop adTopScript;
    AdBottom adBottomScript;

    void Start()
    {
        fadeImage = GetComponent<Image>();
        bgmAudioSource = bgm.GetComponent<AudioSource>();
        adTopScript = adTop.GetComponent<AdTop>();
        adBottomScript = adBottom.GetComponent<AdBottom>();
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
        if (adTopScript.adLoaded == true)
        {
            adTopScript.bannerView.Show();
        }

        if (adBottomScript.adLoaded == true)
        {
            adBottomScript.bannerView.Show();
        }

        fadeImage.enabled = false;
    }

    IEnumerator FadeOutSceneCoroutine(string nextScene)
    {
        SEManager.Instance.PlaySE(SEManager.Instance.start);

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

        // 指定されたシーンに遷移する
        SceneManager.LoadScene(nextScene);
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

    IEnumerator FadeOutPanelCoroutine()
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
            alpha = this.fadeDeltaTime / this.fadeInOutPanelTime;

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
        while (this.fadeDeltaTime <= this.fadeInOutPanelTime);
    }

    // BGMの音量を徐々に上げる
    IEnumerator TurnUpCoroutine()
    {
        while (bgmAudioSource.volume < 0.5f)
        {
            bgmAudioSource.volume += 0.05f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // BGMの音量を徐々に下げる
    IEnumerator TurnDownCoroutine()
    {
        while (bgmAudioSource.volume > 0.2f)
        {
            bgmAudioSource.volume -= 0.05f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // BGMの音量を徐々に消す
    IEnumerator TurnOffCoroutine()
    {
        while (bgmAudioSource.volume > 0)
        {
            bgmAudioSource.volume -= 0.025f;
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

    // エンディングムービーに入るときのフェードアウト
    IEnumerator FadeOutEndingCoroutine()
    {
        FadeOutPanel();
        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime + 1.0f);
        SceneManager.LoadScene("Ending");
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

    public void FadeInPanel()
    {
        IEnumerator coroutine = FadeInPanelCoroutine();
        StartCoroutine(coroutine);
    }

    public void FadeOutPanel()
    {
        IEnumerator coroutine = FadeOutPanelCoroutine();
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

    public void FadeOutEnding()
    {
        IEnumerator coroutine = FadeOutEndingCoroutine();
        StartCoroutine(coroutine);
    }
}
