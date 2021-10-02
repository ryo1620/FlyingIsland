using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrateStoneKey : SingletonMonoBehaviour<CrateStoneKey>
{
    // 表示している画像    
    public Image[] images;
    // 画像のソース    
    public Sprite[] imageSources;

    // 正解時に切り替える画像
    public GameObject crateStoneKey;
    public GameObject crateStoneKeyOpen;
    public GameObject crateStoneKeyEmpty;

    // 正解時にボタンの受付を無効にするための変数
    public GameObject[] collisions;

    // 長方形の現在の向き
    enum Direction
    {
        Vertical,
        UpperRight,
        Horizontal,
        UpperLeft,
    }
    Direction[] currentDirections = new Direction[4];

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
            ChangeDirection(buttonNumber);
            ShowDirectionImage(buttonNumber);
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

    // Direction変数を変更する
    void ChangeDirection(int buttonNumber)
    {
        if (currentDirections[buttonNumber] == Direction.UpperLeft)
        {
            currentDirections[buttonNumber] = Direction.Vertical;
        }
        else
        {
            currentDirections[buttonNumber]++;
        }
    }

    // Direction変数に応じた画像を表示する
    void ShowDirectionImage(int buttonNumber)
    {
        images[buttonNumber].sprite = imageSources[(int)currentDirections[buttonNumber]];
    }

    // 正誤判定する
    bool IsCorrect()
    {
        if (currentDirections[0] == Direction.Vertical && currentDirections[1] == Direction.UpperLeft && currentDirections[2] == Direction.UpperRight && currentDirections[3] == Direction.Horizontal)
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
            SEManager.Instance.PlaySE(SEManager.Instance.open);
            crateStoneKeyOpen.SetActive(true);
            crateStoneKey.SetActive(false);
            UIManager.Instance.ShowMainUI();
            // 状態をセーブする
            SaveManager.Instance.SetSolvedGimmickFlag(Gimmick.Type.CrateStoneKey, true);

            // 表示させるヒントを切り替える
            HintManager.Instance.SetHintText();
        }));
    }

    // アイテムを取得したときの処理
    public void GetItem()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.getItem);

            crateStoneKeyEmpty.SetActive(true);
            crateStoneKeyOpen.SetActive(false);

            UIManager.Instance.ShowItemWindow(UIManager.Instance.windowItems[(int)Item.Type.StoneKey]);
            ItemBoxManager.Instance.SetItem(Item.Type.StoneKey);
            // 状態をセーブする
            SaveManager.Instance.SetGotItemFlag(Item.Type.StoneKey, true);
        }
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool solvedGimmick = SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.CrateStoneKey);
        bool gotItem = SaveManager.Instance.GetGotItemFlag(Item.Type.StoneKey);

        if (solvedGimmick == true && gotItem == true)
        {
            crateStoneKeyEmpty.SetActive(true);
            crateStoneKey.SetActive(false);
        }
        else if (solvedGimmick == true && gotItem == false)
        {
            crateStoneKeyOpen.SetActive(true);
            crateStoneKey.SetActive(false);
        }
    }
}
