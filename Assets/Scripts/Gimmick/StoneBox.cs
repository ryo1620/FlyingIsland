using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBox : MonoBehaviour
{
    // 解除時に表示する画像を取得
    public GameObject stoneBox;
    public GameObject stoneBoxOpen;
    public GameObject stoneBoxEmpty;

    // ボタンの配置
    // 00 01
    // 02 03 
    public GameObject[] lines00;
    public GameObject[] lines01;
    public GameObject[] lines02;
    public GameObject[] lines03;
    GameObject[][] buttons;

    // 正解時にボタンの受付を無効にするための変数
    public GameObject[] collisions;

    // 現在のパターン
    enum Pattern
    {
        Single,
        Right,
        Left,
        All,
    }
    Pattern[] currentPatterns = new Pattern[4];

    void Start()
    {
        buttons = new GameObject[][] { lines00, lines01, lines02, lines03 };

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
            buttons[buttonNumber][(int)currentPatterns[buttonNumber]].SetActive(false);
            if (currentPatterns[buttonNumber] == Pattern.All)
            {
                currentPatterns[buttonNumber] = Pattern.Single;
            }
            else
            {
                currentPatterns[buttonNumber]++;
            }
            buttons[buttonNumber][(int)currentPatterns[buttonNumber]].SetActive(true);

            // 正解したときの処理
            // ボタンの配置
            // 00 01
            // 02 03 
            if (currentPatterns[0] == Pattern.Right && currentPatterns[1] == Pattern.Single && currentPatterns[2] == Pattern.All && currentPatterns[3] == Pattern.Left)
            {
                // コルーチンの起動
                StartCoroutine(this.DelayCoroutine(0.5f, () =>
                {
                    // 0.5秒後に以下の処理が実行される                
                    if (currentPatterns[0] == Pattern.Right && currentPatterns[1] == Pattern.Single && currentPatterns[2] == Pattern.All && currentPatterns[3] == Pattern.Left)
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
                            SEManager.Instance.PlaySE(SEManager.Instance.moveStone);
                            stoneBoxOpen.SetActive(true);
                            stoneBox.SetActive(false);
                            UIManager.Instance.ShowMainUI();
                            // 状態をセーブする
                            SaveManager.Instance.SetSolvedGimmickFlag(Gimmick.Type.StoneBox, true);

                            // 表示させるヒントを切り替える
                            HintManager.Instance.SetHintText();
                        }));
                    }

                }));
            }
        }

    }

    // たいまつを取得したときの処理
    public void GetItem()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.getItem);

            stoneBoxEmpty.SetActive(true);
            stoneBoxOpen.SetActive(false);

            UIManager.Instance.ShowItemWindow(UIManager.Instance.windowItems[(int)Item.Type.Torch]);
            ItemBoxManager.Instance.SetItem(Item.Type.Torch);
            // 状態をセーブする
            SaveManager.Instance.SetGotItemFlag(Item.Type.Torch, true);
        }
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool solvedGimmick = SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.StoneBox);
        bool gotItem = SaveManager.Instance.GetGotItemFlag(Item.Type.Torch);

        if (solvedGimmick == true && gotItem == true)
        {
            stoneBoxEmpty.SetActive(true);
            stoneBox.SetActive(false);
        }
        else if (solvedGimmick == true && gotItem == false)
        {
            stoneBoxOpen.SetActive(true);
            stoneBox.SetActive(false);
        }
    }
}
