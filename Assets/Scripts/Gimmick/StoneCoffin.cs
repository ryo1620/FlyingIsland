using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoneCoffin : MonoBehaviour
{
    // 解除時に表示する画像を取得
    public GameObject stoneCoffin;
    public GameObject stoneCoffinOpen;
    public GameObject stoneCoffinEmpty;

    // 現在の正解数を変数で管理
    int score = 0;

    // 正解時にボタンの受付を無効にするための変数
    public GameObject rightButton;
    public GameObject leftButton;
    Button rightButtonCollision;
    Button leftButtonCollision;

    void Start()
    {
        rightButtonCollision = rightButton.GetComponent<Button>();
        leftButtonCollision = leftButton.GetComponent<Button>();

        LoadImage();
    }

    // 右ボタンを押したときの処理
    public void OnRightButton()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.push);

            if (score == 0 || score == 3)
            {
                score += 1;
            }
            else
            {
                score = 1;
            }
        }
    }

    // 左ボタンを押したときの処理
    public void OnLeftButton()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.push);

            if (score == 4)
            {
                score = 5;

                // コルーチンの起動
                StartCoroutine(this.DelayCoroutine(0.5f, () =>
                {
                    // 0.5秒後に以下の処理が実行される                
                    if (score == 5)
                    {
                        // 操作の受付を無効にする
                        UIManager.Instance.HideMainUI();
                        rightButtonCollision.enabled = false;
                        leftButtonCollision.enabled = false;

                        SEManager.Instance.PlaySE(SEManager.Instance.correct);
                        // ギミック解除
                        StartCoroutine(this.DelayCoroutine(1.0f, () =>
                        {
                            SEManager.Instance.PlaySE(SEManager.Instance.moveRock);
                            stoneCoffinOpen.SetActive(true);
                            stoneCoffin.SetActive(false);
                            UIManager.Instance.ShowMainUI();
                            // 状態をセーブする
                            SaveManager.Instance.SetSolvedGimmickFlag(Gimmick.Type.StoneCoffin, true);

                            // 表示させるヒントを切り替える
                            HintManager.Instance.SetHintText();
                        }));
                    }

                }));
            }
            else if (score == 1 || score == 2)
            {
                score += 1;
            }
            // 正解したもののすぐにボタンを押してコルーチンが起動しなかった場合、右・左・左・右・左（正解ルート）の3番目になる
            else if (score == 5)
            {
                score = 3;
            }
            else
            {
                score = 0;
            }
        }
    }

    // 石版を取得したときの処理
    public void GetItem()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.getItem);

            stoneCoffinEmpty.SetActive(true);
            stoneCoffinOpen.SetActive(false);

            UIManager.Instance.ShowItemWindow(UIManager.Instance.windowItems[(int)Item.Type.Lithograph]);
            ItemBoxManager.Instance.SetItem(Item.Type.Lithograph);
            // 状態をセーブする
            SaveManager.Instance.SetGotItemFlag(Item.Type.Lithograph, true);
        }
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool solvedGimmick = SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.StoneCoffin);
        bool gotItem = SaveManager.Instance.GetGotItemFlag(Item.Type.Lithograph);

        if (solvedGimmick == true && gotItem == true)
        {
            stoneCoffinEmpty.SetActive(true);
            stoneCoffin.SetActive(false);
        }
        else if (solvedGimmick == true && gotItem == false)
        {
            stoneCoffinOpen.SetActive(true);
            stoneCoffin.SetActive(false);
        }
    }
}
