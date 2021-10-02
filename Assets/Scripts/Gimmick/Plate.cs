using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plate : MonoBehaviour
{
    // 解除時に表示する画像を取得
    public GameObject plate;
    public GameObject plateOpen;
    public GameObject[] panelBackgrounds;

    // 現在の正解数を変数で管理
    int score = 0;

    // 正解時にボタンの受付を無効にするための変数
    public GameObject yellowButton;
    public GameObject blueButton;
    public GameObject pinkButton;
    public GameObject orangeButton;
    Button yellowButtonCollision;
    Button blueButtonCollision;
    Button pinkButtonCollision;
    Button orangeButtonCollision;

    // 削岩しているかどうか判別するためのオブジェクトおよびスクリプトを取得する
    public GameObject golem;
    Golem golemScript;

    void Start()
    {
        golemScript = golem.GetComponent<Golem>();
        yellowButtonCollision = yellowButton.GetComponent<Button>();
        blueButtonCollision = blueButton.GetComponent<Button>();
        pinkButtonCollision = pinkButton.GetComponent<Button>();
        orangeButtonCollision = orangeButton.GetComponent<Button>();

        LoadImage();
    }

    // 黄色ボタンを押したときの処理
    public void OnYellowButton()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.push);

            if (score == 5)
            {
                score = 6;

                // コルーチンの起動
                StartCoroutine(this.DelayCoroutine(0.5f, () =>
                {
                    // 0.5秒後に以下の処理が実行される                
                    if (score == 6)
                    {
                        // 操作の受付を無効にする
                        UIManager.Instance.HideMainUI();
                        yellowButtonCollision.enabled = false;
                        blueButtonCollision.enabled = false;
                        pinkButtonCollision.enabled = false;
                        orangeButtonCollision.enabled = false;

                        SEManager.Instance.PlaySE(SEManager.Instance.correct);
                        // ギミック解除
                        StartCoroutine(this.DelayCoroutine(1.0f, () =>
                        {
                            SEManager.Instance.PlaySE(SEManager.Instance.moveRock);
                            ShowPlateOpen();
                            SelectPanelBackground();
                            UIManager.Instance.ShowMainUI();

                            // 望遠鏡の仕掛けを解けるようにする
                            GimmickFlag.Instance.canSolveTelescope = true;
                            SaveManager.Instance.SetCanSolveGimmickFlag(Gimmick.Type.Telescope, true);

                            // 状態をセーブする
                            SaveManager.Instance.SetSolvedGimmickFlag(Gimmick.Type.Plate, true);

                            // 表示させるヒントを切り替える
                            HintManager.Instance.SetHintText();
                        }));
                    }

                }));
            }
            else if (score == 3)
            {
                score += 1;
            }
            else
            {
                score = 0;
            }
        }

    }

    // 青色ボタンを押したときの処理
    public void OnBlueButton()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.push);
            score = 0;
        }
    }

    // ピンクボタンを押したときの処理
    public void OnPinkButton()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.push);

            if (score == 1 || score == 4)
            {
                score += 1;
            }
            // scoreが3のときはオレンジボタンを押したあとなので、scoreを2に戻す
            else if (score == 3)
            {
                score = 2;
            }
            else
            {
                score = 0;
            }
        }
    }

    // オレンジボタンを押したときの処理
    public void OnOrangeButton()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.push);

            if (score == 0 || score == 2)
            {
                score += 1;
            }
            else
            {
                score = 1;
            }
        }
    }

    // 開いたプレート画像を表示する
    void ShowPlateOpen()
    {
        plateOpen.SetActive(true);
        plate.SetActive(false);
    }

    // 削岩しているか（バッテリーを使用したか）どうかで表示させる遠景を切替える
    void SelectPanelBackground()
    {
        bool usedItem = SaveManager.Instance.GetUsedItemFlag(Item.Type.Battery);

        if (usedItem == true)
        {
            panelBackgrounds[3].SetActive(true);
            panelBackgrounds[2].SetActive(false);
            panelBackgrounds[0].SetActive(false);
        }
        else
        {
            panelBackgrounds[1].SetActive(true);
            panelBackgrounds[0].SetActive(false);
        }
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool solvedGimmick = SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.Plate);

        if (solvedGimmick == true)
        {
            ShowPlateOpen();
            SelectPanelBackground();
        }
    }
}
