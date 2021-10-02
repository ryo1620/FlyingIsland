using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateArrow : MonoBehaviour
{
    // 解除時に表示する画像を取得
    public GameObject crateArrow;
    public GameObject crateArrowOpen;
    public GameObject crateArrowEmpty;

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
            if (GimmickFlag.Instance.canSolveCrateArrow == true && currentPositions[0] == Position.Top && currentPositions[1] == Position.Left && currentPositions[2] == Position.Right && currentPositions[3] == Position.Bottom)
            {
                // コルーチンの起動
                StartCoroutine(this.DelayCoroutine(0.5f, () =>
                {
                    // 0.5秒後に以下の処理が実行される                
                    if (GimmickFlag.Instance.canSolveCrateArrow == true && currentPositions[0] == Position.Top && currentPositions[1] == Position.Left && currentPositions[2] == Position.Right && currentPositions[3] == Position.Bottom)
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
                            crateArrowOpen.SetActive(true);
                            crateArrow.SetActive(false);
                            UIManager.Instance.ShowMainUI();
                            // 状態をセーブする
                            SaveManager.Instance.SetSolvedGimmickFlag(Gimmick.Type.CrateArrow, true);

                            // 表示させるヒントを切り替える
                            HintManager.Instance.SetHintText();
                        }));
                    }

                }));
            }
        }
    }

    // 矢を取得したときの処理
    public void GetItem()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.getItem);

            crateArrowEmpty.SetActive(true);
            crateArrowOpen.SetActive(false);

            UIManager.Instance.ShowItemWindow(UIManager.Instance.windowItems[(int)Item.Type.Arrow]);
            ItemBoxManager.Instance.SetItem(Item.Type.Arrow);
            // 状態をセーブする
            SaveManager.Instance.SetGotItemFlag(Item.Type.Arrow, true);
        }
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool solvedGimmick = SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.CrateArrow);
        bool gotItem = SaveManager.Instance.GetGotItemFlag(Item.Type.Arrow);

        if (solvedGimmick == true && gotItem == true)
        {
            crateArrowEmpty.SetActive(true);
            crateArrow.SetActive(false);
        }
        else if (solvedGimmick == true && gotItem == false)
        {
            crateArrowOpen.SetActive(true);
            crateArrow.SetActive(false);
        }
    }
}
