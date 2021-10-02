using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public GameObject treeCutDown;
    public GameObject panelBackground02;

    void Start()
    {
        LoadImage();
    }

    // 木をタップしたときの処理
    public void OnTree()
    {
        // オノが選択されており、かつアイテムウィンドウが非表示であれば処理を行う
        if (ItemBoxManager.Instance.IsSelected(Item.Type.Axe) && UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.chop);
            UIManager.Instance.HideMainUI();
            // 状態をセーブする
            SaveManager.Instance.SetUsedItemFlag(Item.Type.Axe, true);

            ItemBoxManager.Instance.DeleteItem(Item.Type.Axe);

            // 表示させるヒントを切り替える
            HintManager.Instance.SetHintText();

            treeCutDown.SetActive(true);

            panelBackground02.SetActive(true);

            StartCoroutine(this.DelayCoroutine(1.0f, () =>
            {
                SEManager.Instance.PlaySE(SEManager.Instance.chop);
                PanelChanger.Instance.ReturnAfterCuttingDownTree();
            }));
        }
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool usedItem = SaveManager.Instance.GetUsedItemFlag(Item.Type.Axe);

        if (usedItem == true)
        {
            panelBackground02.SetActive(true);
        }
    }
}
