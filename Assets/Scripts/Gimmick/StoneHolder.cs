using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// シングルトン化する
public class StoneHolder : SingletonMonoBehaviour<StoneHolder>
{
    // 表示している画像    
    public Image[] images;
    // 画像のソース    
    public Sprite[] imageSources;

    public GameObject[] panelBackgrounds;

    // 正解時にボタンの受付を無効にするための変数
    public GameObject[] collisions;

    // UIの表示後に移動できるようにするため、飛行機の当たり判定のオブジェクトを取得する
    public GameObject collisionAirPlane;

    // 現在の番号
    enum Number
    {
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
    }
    Number[] currentNumbers = new Number[3];

    void Start()
    {
        LoadImage();
    }

    // ボタンを押したときの処理
    public void OnButton(int buttonNumber)
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.tap);
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
                        // 状態をセーブする
                        SaveManager.Instance.SetSolvedGimmickFlag(Gimmick.Type.StoneHolder, true);
                        // 表示させるヒントを切り替える
                        HintManager.Instance.SetHintText();
                    }
                }));
            }
        }
    }

    // Number変数を変更する
    void ChangeNumber(int buttonNumber)
    {
        if (currentNumbers[buttonNumber] == Number.Nine)
        {
            currentNumbers[buttonNumber] = Number.One;
        }
        else
        {
            currentNumbers[buttonNumber]++;
        }
    }

    // Number変数に応じた画像を表示する
    void ShowNumberImage(int buttonNumber)
    {
        images[buttonNumber].sprite = imageSources[(int)currentNumbers[buttonNumber]];
    }

    // 正誤判定する
    bool IsCorrect()
    {
        if (currentNumbers[0] == Number.Five && currentNumbers[1] == Number.Six && currentNumbers[2] == Number.Eight)
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

        SEManager.Instance.PlaySE(SEManager.Instance.correct);
        // ギミック解除
        StartCoroutine(this.DelayCoroutine(1.0f, () =>
        {
            IEnumerator coroutine = PanelChanger.Instance.BuildBridgeCoroutine();
            StartCoroutine(coroutine);
        }));
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool solvedGimmick = SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.StoneHolder);

        if (solvedGimmick == true)
        {
            panelBackgrounds[1].SetActive(true);
            collisionAirPlane.SetActive(true);
        }
    }
}
