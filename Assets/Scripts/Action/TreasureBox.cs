using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    // 表示する画像を取得する
    public GameObject treasureBoxBuried;
    public GameObject treasureBoxDigged;
    public GameObject treasureBoxClosed;
    public GameObject treasureBoxUsedKey;
    public GameObject treasureBoxOpen;
    public GameObject treasureBoxEmpty;
    public GameObject panelBackground00;
    public GameObject panelBackground01;

    void Start()
    {
        LoadImage();
    }

    // 土をタップしたときの処理
    public void onEarth()
    {
        // スコップが選択されており、かつアイテムウィンドウが非表示であれば処理を行う
        if (ItemBoxManager.Instance.IsSelected(Item.Type.Shovel) && UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.dig);
            UIManager.Instance.HideMainUI();
            // 状態をセーブする
            SaveManager.Instance.SetUsedItemFlag(Item.Type.Shovel, true);

            ItemBoxManager.Instance.DeleteItem(Item.Type.Shovel);

            // 表示させるヒントを切り替える
            HintManager.Instance.SetHintText();

            treasureBoxDigged.SetActive(true);
            treasureBoxBuried.SetActive(false);

            ChangePanelBackground();

            // コルーチンを使って土を掘ったあとの画像を表示させる
            StartCoroutine(this.DelayCoroutine(1.0f, () =>
            {
                SEManager.Instance.PlaySE(SEManager.Instance.dig);
                treasureBoxClosed.SetActive(true);
                treasureBoxDigged.SetActive(false);
                UIManager.Instance.ShowMainUI();
            }));
        }
    }

    // 鍵穴をタップしたときの処理
    public void OnKeyhole()
    {
        // 鍵が選択されており、かつアイテムウィンドウが非表示であれば処理を行う
        if (ItemBoxManager.Instance.IsSelected(Item.Type.Key) && UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.unlockKey);
            UIManager.Instance.HideMainUI();
            ItemBoxManager.Instance.DeleteItem(Item.Type.Key);
            // 状態をセーブする
            SaveManager.Instance.SetUsedItemFlag(Item.Type.Key, true);

            // 表示させるヒントを切り替える
            HintManager.Instance.SetHintText();

            treasureBoxUsedKey.SetActive(true);
            treasureBoxClosed.SetActive(false);

            // コルーチンを使って宝箱を開いた画像を表示させる
            StartCoroutine(this.DelayCoroutine(1.0f, () =>
            {
                SEManager.Instance.PlaySE(SEManager.Instance.open);
                treasureBoxOpen.SetActive(true);
                treasureBoxUsedKey.SetActive(false);
                UIManager.Instance.ShowMainUI();
            }));
        }
    }

    // 弓を取得したときの処理
    public void GetItem()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.getItem);

            treasureBoxEmpty.SetActive(true);
            treasureBoxOpen.SetActive(false);

            UIManager.Instance.ShowItemWindow(UIManager.Instance.windowItems[(int)Item.Type.Bow]);
            ItemBoxManager.Instance.SetItem(Item.Type.Bow);
            // 状態をセーブする
            SaveManager.Instance.SetGotItemFlag(Item.Type.Bow, true);
        }
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool usedShovel = SaveManager.Instance.GetUsedItemFlag(Item.Type.Shovel);
        bool usedKey = SaveManager.Instance.GetUsedItemFlag(Item.Type.Key);
        bool gotItem = SaveManager.Instance.GetGotItemFlag(Item.Type.Bow);

        if (usedKey == true && gotItem == true)
        {
            ChangePanelBackground();
            treasureBoxEmpty.SetActive(true);
            treasureBoxBuried.SetActive(false);
        }
        else if (usedKey == true && gotItem == false)
        {
            ChangePanelBackground();
            treasureBoxOpen.SetActive(true);
            treasureBoxBuried.SetActive(false);
        }
        else if (usedShovel == true && usedKey == false)
        {
            ChangePanelBackground();
            treasureBoxClosed.SetActive(true);
            treasureBoxBuried.SetActive(false);
        }
    }

    // 遠景を変更する
    void ChangePanelBackground()
    {
        panelBackground01.SetActive(true);
        panelBackground00.SetActive(false);
    }
}
