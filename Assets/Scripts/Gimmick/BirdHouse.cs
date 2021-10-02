using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdHouse : MonoBehaviour
{
    // 解除時に表示する画像を取得
    public GameObject birdHouse;
    public GameObject birdHouseOpen;
    public GameObject birdHouseEmpty;

    public GameObject[] rectangles00;
    public GameObject[] rectangles01;
    public GameObject[] rectangles02;
    public GameObject[] rectangles03;
    GameObject[][] buttons;

    // 正解時にボタンの受付を無効にするための変数
    public GameObject[] collisions;

    // 現在の長方形の長さ
    enum Length
    {
        None,
        Short,
        Middle,
        Long,
    }
    Length[] currentLengths = new Length[4];

    void Start()
    {
        buttons = new GameObject[][] { rectangles00, rectangles01, rectangles02, rectangles03 };

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
            buttons[buttonNumber][(int)currentLengths[buttonNumber]].SetActive(false);
            if (currentLengths[buttonNumber] == Length.Long)
            {
                currentLengths[buttonNumber] = Length.None;
            }
            else
            {
                currentLengths[buttonNumber]++;
            }
            buttons[buttonNumber][(int)currentLengths[buttonNumber]].SetActive(true);

            // 正解したときの処理
            if (currentLengths[0] == Length.Middle && currentLengths[1] == Length.None && currentLengths[2] == Length.Long && currentLengths[3] == Length.Short)
            {
                // コルーチンの起動
                StartCoroutine(this.DelayCoroutine(0.5f, () =>
                {
                    // 0.5秒後に以下の処理が実行される                
                    if (currentLengths[0] == Length.Middle && currentLengths[1] == Length.None && currentLengths[2] == Length.Long && currentLengths[3] == Length.Short)
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
                            birdHouseOpen.SetActive(true);
                            birdHouse.SetActive(false);
                            UIManager.Instance.ShowMainUI();
                            // 状態をセーブする
                            SaveManager.Instance.SetSolvedGimmickFlag(Gimmick.Type.BirdHouse, true);

                            // 表示させるヒントを切り替える
                            HintManager.Instance.SetHintText();
                        }));
                    }

                }));
            }
        }
    }

    // 鍵を取得したときの処理
    public void GetItem()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.getItem);

            birdHouseEmpty.SetActive(true);
            birdHouseOpen.SetActive(false);

            UIManager.Instance.ShowItemWindow(UIManager.Instance.windowItems[(int)Item.Type.Key]);
            ItemBoxManager.Instance.SetItem(Item.Type.Key);
            // 状態をセーブする
            SaveManager.Instance.SetGotItemFlag(Item.Type.Key, true);
        }
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool solvedGimmick = SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.BirdHouse);
        bool gotItem = SaveManager.Instance.GetGotItemFlag(Item.Type.Key);

        if (solvedGimmick == true && gotItem == true)
        {
            birdHouseEmpty.SetActive(true);
            birdHouse.SetActive(false);
        }
        else if (solvedGimmick == true && gotItem == false)
        {
            birdHouseOpen.SetActive(true);
            birdHouse.SetActive(false);
        }
    }
}
