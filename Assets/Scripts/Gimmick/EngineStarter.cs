using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EngineStarter : MonoBehaviour
{
    // 表示している画像    
    public Image[] images;
    // 画像のソース    
    public Sprite[] imageSources;

    public GameObject[] panelBackgrounds;

    // 正解時にボタンの受付を無効にするための変数
    public GameObject[] collisions;

    enum Color
    {
        White,
        Blue,
        Purple,
        Red,
        Yellow,
    }
    Color[] currentColors = new Color[4];

    // ボタンを押したときの処理
    public void OnButton(int buttonNumber)
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.push);
            ChangeNumber(buttonNumber);
            ShowNumberImage(buttonNumber);
            if (IsCorrect())
            {
                // コルーチンの起動
                StartCoroutine(this.DelayCoroutine(0.5f, () =>
                {
                    if (IsCorrect())
                    {
                        SolveGimmick();
                    }
                }));
            }
        }
    }

    // Number変数を変更する
    void ChangeNumber(int buttonNumber)
    {
        if (currentColors[buttonNumber] == Color.Yellow)
        {
            currentColors[buttonNumber] = Color.White;
        }
        else
        {
            currentColors[buttonNumber]++;
        }
    }

    // Number変数に応じた画像を表示する
    void ShowNumberImage(int buttonNumber)
    {
        images[buttonNumber].sprite = imageSources[(int)currentColors[buttonNumber]];
    }

    // 正誤判定する
    bool IsCorrect()
    {
        if (currentColors[0] == Color.Red && currentColors[1] == Color.Yellow && currentColors[2] == Color.Blue && currentColors[3] == Color.Purple)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 正解したときの処理
    void SolveGimmick()
    {
        // 操作の受付を無効にする
        UIManager.Instance.HideMainUI();
        foreach (GameObject collision in collisions)
        {
            collision.SetActive(false);
        }

        FadeManager.Instance.TurnOff();

        SEManager.Instance.PlaySE(SEManager.Instance.correct);
        // ギミック解除
        StartCoroutine(this.DelayCoroutine(1.0f, () =>
        {
            panelBackgrounds[0].SetActive(true);
            StartCoroutine(this.DelayCoroutine(0.2f, () =>
            {
                SEManager.Instance.PlaySE(SEManager.Instance.push);
                panelBackgrounds[1].SetActive(true);
                StartCoroutine(this.DelayCoroutine(1.0f, () =>
                {
                    // フェードアウトでエンディングのシーンに遷移する
                    FadeManager.Instance.FadeOutEnding();
                }));
            }));
        }));
    }
}
