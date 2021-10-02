using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    // 表示する画像を取得する
    public GameObject rope;
    public GameObject ropeShooted;
    public GameObject ropeStartedCutting;
    public GameObject ropeFinishedCutting;
    public GameObject panelBackground00;
    public GameObject panelBackground01;

    void Start()
    {
        LoadImage();
    }

    // 布をタップしたときの処理
    public void OnCloth()
    {
        // 弓矢が選択されており、かつアイテムウィンドウが非表示であれば処理を行う
        if (ItemBoxManager.Instance.IsSelected(Item.Type.BowAndArrow) && UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.shoot);
            UIManager.Instance.HideMainUI();
            // 状態をセーブする
            SaveManager.Instance.SetUsedItemFlag(Item.Type.BowAndArrow, true);

            ItemBoxManager.Instance.DeleteItem(Item.Type.BowAndArrow);

            // 表示させるヒントを切り替える
            HintManager.Instance.SetHintText();

            ropeShooted.SetActive(true);
            rope.SetActive(false);

            ChangePanelBackground();

            // コルーチンを使って画像を順次表示させる
            StartCoroutine(this.DelayCoroutine(1.0f, () =>
            {
                ropeStartedCutting.SetActive(true);
                ropeShooted.SetActive(false);

                StartCoroutine(this.DelayCoroutine(1.0f, () =>
                {
                    ropeFinishedCutting.SetActive(true);
                    ropeStartedCutting.SetActive(false);

                    // フェードイン・アウトで遠景に移動する
                    StartCoroutine(this.DelayCoroutine(1.0f, () =>
                    {
                        IEnumerator coroutine = PanelChanger.Instance.MoveAfterCuttingRopeCoroutine();
                        StartCoroutine(coroutine);
                    }));
                }));
            }));
        }
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool usedItem = SaveManager.Instance.GetUsedItemFlag(Item.Type.BowAndArrow);

        if (usedItem == true)
        {
            ChangePanelBackground();
            ropeFinishedCutting.SetActive(true);
            rope.SetActive(false);
        }
    }

    void ChangePanelBackground()
    {
        panelBackground01.SetActive(true);
        panelBackground00.SetActive(false);
    }
}
