using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CratePeanuts : MonoBehaviour
{
    // 解除時に表示する画像を取得する
    public GameObject cratePeanuts;
    public GameObject cratePeanutsOpen;
    public GameObject cratePeanutsEmpty;

    // ボタンの配置
    // 00 01 02
    // 03 04 05
    public GameObject[] rounds00;
    public GameObject[] rounds01;
    public GameObject[] rounds02;
    public GameObject[] rounds03;
    public GameObject[] rounds04;
    public GameObject[] rounds05;
    GameObject[][] buttons;

    // 正解時にボタンの受付を無効にするための変数
    public GameObject[] collisions;

    // 現在の丸の大きさ
    enum Size
    {
        Small,
        Medium,
        Big,
    }
    Size[] currentSizes = new Size[6];

    void Start()
    {
        buttons = new GameObject[][] { rounds00, rounds01, rounds02, rounds03, rounds04, rounds05 };

        LoadImage();
    }

    // ボタンを押したときの処理
    public void OnButton(int buttonNumber)
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.tap);

            // 現在表示されている長方形を非表示にしたあと変数を変更し、次の長方形を表示させる
            buttons[buttonNumber][(int)currentSizes[buttonNumber]].SetActive(false);
            if (currentSizes[buttonNumber] == Size.Big)
            {
                currentSizes[buttonNumber] = Size.Small;
            }
            else
            {
                currentSizes[buttonNumber]++;
            }
            buttons[buttonNumber][(int)currentSizes[buttonNumber]].SetActive(true);

            // 正解したときの処理
            // ボタンの配置
            // 00 01 02
            // 03 04 05
            if (currentSizes[0] == Size.Small && currentSizes[1] == Size.Medium && currentSizes[2] == Size.Small && currentSizes[3] == Size.Medium && currentSizes[4] == Size.Big && currentSizes[5] == Size.Big)
            {
                // コルーチンの起動
                StartCoroutine(this.DelayCoroutine(0.5f, () =>
                {
                    // 0.5秒後に以下の処理が実行される                
                    if (currentSizes[0] == Size.Small && currentSizes[1] == Size.Medium && currentSizes[2] == Size.Small && currentSizes[3] == Size.Medium && currentSizes[4] == Size.Big && currentSizes[5] == Size.Big)
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
                            SEManager.Instance.PlaySE(SEManager.Instance.open);
                            cratePeanutsOpen.SetActive(true);
                            cratePeanuts.SetActive(false);
                            UIManager.Instance.ShowMainUI();
                            // 状態をセーブする
                            SaveManager.Instance.SetSolvedGimmickFlag(Gimmick.Type.CratePeanuts, true);

                            // 表示させるヒントを切り替える
                            HintManager.Instance.SetHintText();
                        }));
                    }

                }));
            }
        }
    }

    // ピーナッツを取得したときの処理
    public void GetItem()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.getItem);

            cratePeanutsEmpty.SetActive(true);
            cratePeanutsOpen.SetActive(false);

            UIManager.Instance.ShowItemWindow(UIManager.Instance.windowItems[(int)Item.Type.Peanuts]);
            ItemBoxManager.Instance.SetItem(Item.Type.Peanuts);
            // 状態をセーブする
            SaveManager.Instance.SetGotItemFlag(Item.Type.Peanuts, true);
        }
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool solvedGimmick = SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.CratePeanuts);
        bool gotItem = SaveManager.Instance.GetGotItemFlag(Item.Type.Peanuts);

        if (solvedGimmick == true && gotItem == true)
        {
            cratePeanutsEmpty.SetActive(true);
            cratePeanuts.SetActive(false);
        }
        else if (solvedGimmick == true && gotItem == false)
        {
            cratePeanutsOpen.SetActive(true);
            cratePeanuts.SetActive(false);
        }
    }
}
