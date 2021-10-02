using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// シングルトン化する
public class EndFadeManager : SingletonMonoBehaviour<EndFadeManager>
{
    // フェードにかかる時間        
    public readonly float fadeInSceneTime = 1.0f;
    public readonly float fadeOutSceneTime = 0.6f;

    // フェードに使った時間
    float fadeDeltaTime = 0;

    // フェードに使う画像
    public GameObject fadeInPanel;
    Image fadeInImage;
    public GameObject fadeOutPanel;
    Image fadeOutImage;

    void Start()
    {
        fadeInImage = fadeInPanel.GetComponent<Image>();
        fadeOutImage = fadeOutPanel.GetComponent<Image>();
        FadeInScene();
    }

    IEnumerator FadeInSceneCoroutine()
    {
        // 色の不透明度
        float alpha = 1;
        // Imageの色変更に使う
        Color color = new Color(255, 255, 255, alpha);
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

        fadeInImage.enabled = false;
    }

    IEnumerator FadeOutSceneCoroutine()
    {
        fadeOutImage.enabled = true;

        // 色の不透明度
        float alpha = 0;
        // Imageの色変更に使う
        Color color = new Color(0, 0, 0, alpha);
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

        SceneManager.LoadScene("Title");
    }

    void FadeInScene()
    {
        IEnumerator coroutine = FadeInSceneCoroutine();
        StartCoroutine(coroutine);
    }

    public void FadeOutScene()
    {
        IEnumerator coroutine = FadeOutSceneCoroutine();
        StartCoroutine(coroutine);
    }
}
