using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : SingletonMonoBehaviour<Crate>
{
    public GameObject crateStoneKeyCaught;
    public GameObject crateStoneKey;
    public GameObject objectBackground01;
    public GameObject panelBackground01;

    void Start()
    {
        LoadImage();
    }

    // 水上の木箱をタップしたときの処理
    public void OnCrateStoneKey()
    {
        // 網が選択されており、かつアイテムウィンドウが非表示であれば処理を行う
        if (ItemBoxManager.Instance.IsSelected(Item.Type.Net) && UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.water);
            UIManager.Instance.HideMainUI();
            // 状態をセーブする
            SaveManager.Instance.SetUsedItemFlag(Item.Type.Net, true);

            ItemBoxManager.Instance.DeleteItem(Item.Type.Net);

            // 表示させるヒントを切り替える
            HintManager.Instance.SetHintText();

            crateStoneKeyCaught.SetActive(true);

            objectBackground01.SetActive(true);
            panelBackground01.SetActive(true);

            StartCoroutine(this.DelayCoroutine(1.0f, () =>
            {
                PanelChanger.Instance.ReturnAfterCatchingCrate();
            }));
        }
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool usedItem = SaveManager.Instance.GetUsedItemFlag(Item.Type.Net);

        if (usedItem == true)
        {
            panelBackground01.SetActive(true);
            objectBackground01.SetActive(true);
            crateStoneKey.SetActive(true);
        }
    }
}
