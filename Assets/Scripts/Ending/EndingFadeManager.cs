using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class EndingFadeManager : SingletonMonoBehaviour<EndingFadeManager>
{
    // フェードにかかる時間        
    public readonly float fadeInSceneTime = 1.0f;
    public readonly float fadeOutSceneTime = 2.0f;

    // フェードに使った時間
    float fadeDeltaTime = 0;

    // フェードに使う画像
    public GameObject fadeInPanel;
    Image fadeInImage;
    public GameObject fadeOutPanel;
    Image fadeOutImage;

    // MovieのVideoPlayerとAudioSource
    public GameObject movie;
    VideoPlayer videoPlayer;
    AudioSource bgmAudioSource;

    private void Start()
    {
        fadeInImage = fadeInPanel.GetComponent<Image>();
        fadeOutImage = fadeOutPanel.GetComponent<Image>();
        videoPlayer = movie.GetComponent<VideoPlayer>();
        bgmAudioSource = movie.GetComponent<AudioSource>();

        // 読込完了時のコールバックを設定
        videoPlayer.prepareCompleted += OnCompletePrepare;
        // 動画の読込開始
        videoPlayer.Prepare();
    }

    // 動画の読込完了時のコールバック
    void OnCompletePrepare(VideoPlayer player)
    {
        PlayMovie();
        PlayBgm();
        FadeInScene();

        // エンディングが26秒なので、24秒後にフェードアウトを開始する
        Invoke(nameof(FadeOutScene), 24.0f);
    }

    void PlayMovie()
    {
        videoPlayer.Play();
    }

    void PlayBgm()
    {
        // 設定がオンであればBGMを再生する
        // 1がオン、0がオフ
        int bgmSetting = PlayerPrefs.GetInt("BGM", 1);

        if (bgmSetting == 1)
        {
            bgmAudioSource.Play();
        }
    }

    IEnumerator FadeInSceneCoroutine()
    {
        // 自動スリープを無効にする
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // 色の不透明度
        float alpha = 1;
        // Imageの色変更に使う
        Color color = new Color(0, 0, 0, alpha);
        // 初期化
        this.fadeDeltaTime = 0;
        // 色の初期化（黒）
        fadeInImage.color = color;

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
            fadeInImage.color = color;
        }
        while (this.fadeDeltaTime <= this.fadeInSceneTime);
    }

    IEnumerator FadeOutSceneCoroutine()
    {
        // 色の不透明度
        float alpha = 0;
        // Imageの色変更に使う
        Color color = new Color(255, 255, 255, alpha);
        // 初期化
        this.fadeDeltaTime = 0;
        // 色の初期化
        fadeOutImage.color = color;

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
            fadeOutImage.color = color;
        }
        while (this.fadeDeltaTime <= this.fadeOutSceneTime);

        // エンディング後のシーンに遷移する
        SceneManager.LoadScene("End");
    }

    void FadeInScene()
    {
        IEnumerator coroutine = FadeInSceneCoroutine();
        StartCoroutine(coroutine);
    }

    void FadeOutScene()
    {
        IEnumerator coroutine = FadeOutSceneCoroutine();
        StartCoroutine(coroutine);
    }
}
