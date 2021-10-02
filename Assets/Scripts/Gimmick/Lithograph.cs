using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lithograph : MonoBehaviour
{
    // 解除時に表示する画像を取得
    public GameObject lithographDark;
    public GameObject lithographIgnited;
    public GameObject caveDark;
    public GameObject caveIgnited;

    public GameObject[] letters00;
    public GameObject[] letters01;
    public GameObject[] letters02;
    public GameObject[] letters03;
    GameObject[][] letters;

    // 正解時にボタンの受付を無効にするための変数
    public GameObject[] collisions;

    // 現在の文字の色
    enum Color
    {
        White,
        Orange,
        Blue,
    }
    Color[] currentColors = new Color[4];

    void Start()
    {
        letters = new GameObject[][] { letters00, letters01, letters02, letters03 };

        LoadImage();
    }

    // 文字を押したときの処理
    public void OnLetter(int letterNumber)
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.tap);

            // 文字の色を切り替える
            GameObject previousColor = letters[letterNumber][(int)currentColors[letterNumber]];
            if (currentColors[letterNumber] == Color.Blue)
            {
                currentColors[letterNumber] = Color.White;
            }
            else
            {
                currentColors[letterNumber]++;
            }
            letters[letterNumber][(int)currentColors[letterNumber]].SetActive(true);
            previousColor.SetActive(false);
        }
    }

    // ボタンを押したときの処理
    public void OnButton()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.push);

            // 操作の受付を無効にする
            UIManager.Instance.HideMainUI();
            foreach (GameObject collision in collisions)
            {
                collision.SetActive(false);
            }

            // コルーチンの起動
            StartCoroutine(this.DelayCoroutine(0.5f, () =>
            {
                if (ItemBoxManager.Instance.ItemExists(Item.Type.Lithograph) && currentColors[0] == Color.White && currentColors[1] == Color.Blue && currentColors[2] == Color.Orange && currentColors[3] == Color.Orange)
                {
                    ItemBoxManager.Instance.DeleteItem(Item.Type.Lithograph);
                    // 状態をセーブする
                    SaveManager.Instance.SetSolvedGimmickFlag(Gimmick.Type.Lithograph, true);

                    // 表示させるヒントを切り替える
                    HintManager.Instance.SetHintText();

                    SEManager.Instance.PlaySE(SEManager.Instance.correct);
                    // ギミック解除
                    StartCoroutine(this.DelayCoroutine(1.0f, () =>
                    {
                        SEManager.Instance.PlaySE(SEManager.Instance.ignition);
                        ShowLithographIgnited();
                        ShowCaveIgnited();
                        UIManager.Instance.ShowMainUI();
                    }));
                }
                // 不正解のときの処理
                else
                {
                    SEManager.Instance.PlaySE(SEManager.Instance.incorrect);
                    foreach (GameObject collision in collisions)
                    {
                        collision.SetActive(true);
                    }
                    UIManager.Instance.ShowMainUI();
                }
            }));
        }
    }

    void ShowLithographIgnited()
    {
        lithographIgnited.SetActive(true);
        lithographDark.SetActive(false);
    }

    void ShowCaveIgnited()
    {
        caveIgnited.SetActive(true);
        caveDark.SetActive(false);
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool solvedGimmick = SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.Lithograph);

        if (solvedGimmick == true)
        {
            ShowLithographIgnited();
            ShowCaveIgnited();
        }
    }
}
