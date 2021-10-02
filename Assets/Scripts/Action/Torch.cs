using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    // たいまつに火をつけたあと、壁掛けたいまつへの移動を制限するためにオブジェクトを取得する
    public GameObject object00;
    public GameObject object01;
    public GameObject object02;

    void Start()
    {
        RestrictMovementWhenLoaded();
    }

    // たいまつを選択中であれば火をつける処理
    public void OnFire()
    {
        // たいまつが選択されており、かつアイテムウィンドウが非表示であれば処理を行う
        if (ItemBoxManager.Instance.IsSelected(Item.Type.Torch) && UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.ignition);
            UIManager.Instance.HideMainUI();

            StartCoroutine(this.DelayCoroutine(0.5f, () =>
            {
                SEManager.Instance.PlaySE(SEManager.Instance.getItem);

                UIManager.Instance.ShowItemWindow(UIManager.Instance.windowItems[(int)Item.Type.TorchFire]);
                ItemBoxManager.Instance.SetItem(Item.Type.TorchFire);
                ItemBoxManager.Instance.DeleteItem(Item.Type.Torch);

                // 状態をセーブする
                SaveManager.Instance.SetUsedItemFlag(Item.Type.Torch, true);

                // 表示させるヒントを切り替える
                HintManager.Instance.SetHintText();

                RestrictMovement();

                UIManager.Instance.ShowMainUI();
            }));
        }
    }

    // 壁掛けたいまつへの移動を制限する
    void RestrictMovement()
    {
        object00.SetActive(false);
        object01.SetActive(false);
        object02.SetActive(false);
    }

    // ロード時、たいまつを使用済みであれば壁掛けたいまつへの移動を制限する
    void RestrictMovementWhenLoaded()
    {
        bool usedItem = SaveManager.Instance.GetUsedItemFlag(Item.Type.Torch);

        if (usedItem == true)
        {
            RestrictMovement();
        }
    }
}
