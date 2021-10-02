using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : MonoBehaviour
{
    // バッテリー使用時に表示する画像を取得する
    public GameObject batteryHole;
    public GameObject batteryIn;
    public GameObject[] startingGolems;

    // ゴーレムが岩を砕いたあとに背景を変更するため、画像を取得する    
    public GameObject[] panelBackgrounds;
    public GameObject objectBackground00;
    public GameObject objectBackground01;

    // プレートが開いているか判別するためのオブジェクトおよびスクリプトを取得する
    public GameObject plateGimmick;
    Plate plateScript;

    void Start()
    {
        plateScript = plateGimmick.GetComponent<Plate>();

        LoadImage();
    }

    // ゴーレムのバッテリーホールをタップしたときの処理
    public void OnBatteryHole()
    {
        // バッテリーが選択されており、かつアイテムウィンドウが非表示であれば処理を行う
        if (ItemBoxManager.Instance.IsSelected((int)Item.Type.Battery) && UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.open);
            UIManager.Instance.HideMainUI();
            // 状態をセーブする
            SaveManager.Instance.SetUsedItemFlag(Item.Type.Battery, true);

            ItemBoxManager.Instance.DeleteItem((int)Item.Type.Battery);

            // 表示させるヒントを切り替える
            HintManager.Instance.SetHintText();

            batteryIn.SetActive(true);
            batteryHole.SetActive(false);

            ChangeObjectBackground();
            SelectPanelBackground();

            // コルーチンを使って画像を順次表示させる
            StartCoroutine(this.DelayCoroutine(1.0f, () =>
            {
                SEManager.Instance.PlaySE(SEManager.Instance.golemStart);
                startingGolems[0].SetActive(true);
                batteryIn.SetActive(false);

                StartCoroutine(this.DelayCoroutine(1.0f, () =>
                {
                    startingGolems[1].SetActive(true);
                    startingGolems[0].SetActive(false);

                    StartCoroutine(this.DelayCoroutine(1.0f, () =>
                    {
                        startingGolems[2].SetActive(true);
                        startingGolems[1].SetActive(false);

                        StartCoroutine(this.DelayCoroutine(1.0f, () =>
                        {
                            SEManager.Instance.PlaySE(SEManager.Instance.pickaxe);
                            startingGolems[3].SetActive(true);
                            startingGolems[2].SetActive(false);

                            // フェードイン・アウトで削岩後の画面に移動する
                            StartCoroutine(this.DelayCoroutine(1.0f, () =>
                            {
                                SEManager.Instance.PlaySE(SEManager.Instance.pickaxe);
                                IEnumerator coroutine = PanelChanger.Instance.ReturnToFieldCoroutine();
                                StartCoroutine(coroutine);
                            }));
                        }));
                    }));
                }));
            }));
        }
    }

    // 背景画像（近景）01a_0を01a_1に切り替える
    void ChangeObjectBackground()
    {
        objectBackground01.SetActive(true);
        objectBackground00.SetActive(false);
    }

    // 背景画像（遠景）01_0～01_3を切り替える
    // プレートが開いているかどうかによって処理を分ける
    void SelectPanelBackground()
    {
        bool solvedGimmick = SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.Plate);

        if (solvedGimmick == true)
        {
            panelBackgrounds[3].SetActive(true);
            panelBackgrounds[1].SetActive(false);
            panelBackgrounds[0].SetActive(false);
        }
        else
        {
            panelBackgrounds[2].SetActive(true);
            panelBackgrounds[0].SetActive(false);
        }
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool usedItem = SaveManager.Instance.GetUsedItemFlag(Item.Type.Battery);

        if (usedItem == true)
        {
            ChangeObjectBackground();
            SelectPanelBackground();
        }
    }
}
