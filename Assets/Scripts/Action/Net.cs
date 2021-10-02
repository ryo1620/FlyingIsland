using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net : MonoBehaviour
{
    // 網をタップしたときに表示する画像を取得する
    public GameObject golemHoldingNet;
    public GameObject golemLookingAtNet;
    public GameObject golemGivingNet;
    public GameObject golemStanding;
    public GameObject[] panelBackgrounds;

    void Start()
    {
        LoadImage();
    }

    public void OnNet()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.golemStart);
            UIManager.Instance.HideMainUI();

            golemLookingAtNet.SetActive(true);
            golemHoldingNet.SetActive(false);

            SelectPanelBackground();

            // コルーチンを使って画像を順次表示させる
            StartCoroutine(this.DelayCoroutine(1.0f, () =>
            {
                golemGivingNet.SetActive(true);
                golemLookingAtNet.SetActive(false);

                StartCoroutine(this.DelayCoroutine(1.0f, () =>
                {
                    UIManager.Instance.ShowMainUI();
                    GetItem();
                }));
            }));
        }
    }

    // 網を取得したときの処理
    public void GetItem()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.getItem);

        golemStanding.SetActive(true);
        golemGivingNet.SetActive(false);

        UIManager.Instance.ShowItemWindow(UIManager.Instance.windowItems[(int)Item.Type.Net]);
        ItemBoxManager.Instance.SetItem(Item.Type.Net);
        // 状態をセーブする
        SaveManager.Instance.SetGotItemFlag(Item.Type.Net, true);

        // 表示させるヒントを切り替える
        HintManager.Instance.SetHintText();
    }

    // 背景画像（遠景）05_0～05_3を切り替える
    // 望遠鏡の蓋が開いているか（仕掛けを解いたか）どうかによって処理を分ける
    void SelectPanelBackground()
    {
        bool solvedGimmick = SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.Telescope);

        if (solvedGimmick == true)
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
        bool gotItem = SaveManager.Instance.GetGotItemFlag(Item.Type.Net);

        if (gotItem == true)
        {
            SelectPanelBackground();
            golemStanding.SetActive(true);
            golemHoldingNet.SetActive(false);
        }
    }
}
