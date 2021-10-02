using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grave : MonoBehaviour
{
    // 解除時に表示する画像を取得
    public GameObject grave;
    public GameObject graveOpen;
    public GameObject graveEmpty;

    public GameObject[] triangles00;
    public GameObject[] triangles01;
    public GameObject[] triangles02;
    public GameObject[] triangles03;
    GameObject[][] buttons;

    // 正解時にボタンの受付を無効にするための変数
    public GameObject[] collisions;

    // 現在の三角形の位置
    enum Position
    {
        Top,
        Right,
        Bottom,
        Left,
    }
    Position[] currentPositions = new Position[4];

    void Start()
    {
        buttons = new GameObject[][] { triangles00, triangles01, triangles02, triangles03 };

        LoadImage();
    }

    // ボタンを押したときの処理
    public void OnButton(int buttonNumber)
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.tap);

            // 現在表示されている三角形を非表示にしたあと変数を変更し、次の三角形を表示させる
            buttons[buttonNumber][(int)currentPositions[buttonNumber]].SetActive(false);
            if (currentPositions[buttonNumber] == Position.Left)
            {
                currentPositions[buttonNumber] = Position.Top;
            }
            else
            {
                currentPositions[buttonNumber]++;
            }
            buttons[buttonNumber][(int)currentPositions[buttonNumber]].SetActive(true);

            // 正解したときの処理
            if (currentPositions[0] == Position.Top && currentPositions[1] == Position.Left && currentPositions[2] == Position.Bottom && currentPositions[3] == Position.Right)
            {
                // コルーチンの起動
                StartCoroutine(this.DelayCoroutine(0.5f, () =>
                {
                    // 0.5秒後に以下の処理が実行される                
                    if (currentPositions[0] == Position.Top && currentPositions[1] == Position.Left && currentPositions[2] == Position.Bottom && currentPositions[3] == Position.Right)
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
                            graveOpen.SetActive(true);
                            grave.SetActive(false);
                            UIManager.Instance.ShowMainUI();
                            // 状態をセーブする
                            SaveManager.Instance.SetSolvedGimmickFlag(Gimmick.Type.Grave, true);

                            // 表示させるヒントを切り替える
                            HintManager.Instance.SetHintText();
                        }));
                    }

                }));
            }
        }

    }

    // バッテリーを取得したときの処理
    public void GetItem()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.getItem);

            graveEmpty.SetActive(true);
            graveOpen.SetActive(false);

            UIManager.Instance.ShowItemWindow(UIManager.Instance.windowItems[(int)Item.Type.Battery]);
            ItemBoxManager.Instance.SetItem((int)Item.Type.Battery);
            // 状態をセーブする
            SaveManager.Instance.SetGotItemFlag(Item.Type.Battery, true);
        }
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool solvedGimmick = SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.Grave);
        bool gotItem = SaveManager.Instance.GetGotItemFlag(Item.Type.Battery);

        if (solvedGimmick == true && gotItem == true)
        {
            graveEmpty.SetActive(true);
            grave.SetActive(false);
        }
        else if (solvedGimmick == true && gotItem == false)
        {
            graveOpen.SetActive(true);
            grave.SetActive(false);
        }
    }
}
