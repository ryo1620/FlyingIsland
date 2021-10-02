using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candle : SingletonMonoBehaviour<Candle>
{
    // スコップを取得したときに表示する画像を取得する    
    public GameObject shovelDrop;
    public GameObject afterGettingShovel;
    // 近景用    
    public GameObject axeAndDropShovel;
    public GameObject axe;
    // 遠景用    
    public GameObject caveIgnition01;
    public GameObject caveIgnition02;

    // スコップのロックを解除したあと、燭台への移動を制限するためにオブジェクトを取得する
    public GameObject objectCandlestick;

    // スコップをUIの表示後に取得できるようにするため、当たり判定のオブジェクトを取得する
    public GameObject collisionShovel;

    // ロウソクの火のオブジェクトを取得する    
    public GameObject[] fires;

    // すべてのロウソクに火をつけたとき、操作の受付を無効にするための変数
    public GameObject[] collisions;

    // それぞれのロウソクに火がついているかどうか判別するための変数
    bool[] candlesAreOnFire = new bool[6];

    // 火をつけたロウソクの順番を格納する変数
    int[] orderOfFires = new int[6];

    // 火がついているロウソクの数を格納する変数
    int numberOfCandlesOnFire = 0;

    void Start()
    {
        LoadImage();
    }

    // ロウソクに火をつけたときの処理
    public void LightFire(int candleNumber)
    {
        // 点火済みたいまつが選択されており、かつそのロウソクに火がついていない、かつアイテムウィンドウが非表示であれば処理を行う
        if (ItemBoxManager.Instance.IsSelected(Item.Type.TorchFire) && candlesAreOnFire[candleNumber] == false && UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.ignition);
            fires[candleNumber].SetActive(true);
            candlesAreOnFire[candleNumber] = true;

            // 火をつけたロウソクの順番を管理する（正誤判定に利用）
            orderOfFires[numberOfCandlesOnFire] = candleNumber;
            numberOfCandlesOnFire++;

            // すべてのロウソクに火がついたら正誤判定を行う
            if (numberOfCandlesOnFire == 6)
            {
                // 操作の受付を無効にする
                UIManager.Instance.HideMainUI();

                foreach (GameObject collision in collisions)
                {
                    collision.SetActive(false);
                }

                // コルーチンの起動
                StartCoroutine(this.DelayCoroutine(0.5f, () =>
                {
                    // 正解時の処理
                    if (orderOfFires[0] == 5 && orderOfFires[1] == 3 && orderOfFires[2] == 1 && orderOfFires[3] == 2 && orderOfFires[4] == 0 && orderOfFires[5] == 4)
                    {
                        SEManager.Instance.PlaySE(SEManager.Instance.correct);

                        // 点火済みたいまつを削除する
                        ItemBoxManager.Instance.DeleteItem(Item.Type.TorchFire);
                        // 状態をセーブする
                        SaveManager.Instance.SetSolvedGimmickFlag(Gimmick.Type.Candle, true);

                        // 表示させるヒントを切り替える
                        HintManager.Instance.SetHintText();

                        StartCoroutine(this.DelayCoroutine(0.5f, () =>
                        {
                            SEManager.Instance.PlaySE(SEManager.Instance.ignition);

                            foreach (GameObject fire in fires)
                            {
                                fire.SetActive(false);
                            }

                            StartCoroutine(this.DelayCoroutine(0.5f, () =>
                            {
                                // フェードイン・アウトでスコップの前まで移動し、ロック解除時の画像を表示させる
                                IEnumerator coroutine = PanelChanger.Instance.UnlockCoroutine(Item.Type.Shovel);
                                StartCoroutine(coroutine);
                            }));
                        }));
                    }
                    // 不正解時の処理
                    else
                    {
                        SEManager.Instance.PlaySE(SEManager.Instance.incorrect);

                        StartCoroutine(this.DelayCoroutine(0.5f, () =>
                        {
                            SEManager.Instance.PlaySE(SEManager.Instance.ignition);

                            // 各変数を初期化する
                            candlesAreOnFire = new bool[6];
                            orderOfFires = new int[6];
                            numberOfCandlesOnFire = 0;

                            foreach (GameObject fire in fires)
                            {
                                fire.SetActive(false);
                            }

                            foreach (GameObject collision in collisions)
                            {
                                collision.SetActive(true);
                            }

                            UIManager.Instance.ShowMainUI();
                        }));
                    }
                }));
            }
        }
    }

    // スコップを取得したときの処理
    public void GetItem()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.getItem);

            ShowImageAfterGettingShovel();

            UIManager.Instance.ShowItemWindow(UIManager.Instance.windowItems[(int)Item.Type.Shovel]);
            ItemBoxManager.Instance.SetItem(Item.Type.Shovel);
            // 状態をセーブする
            SaveManager.Instance.SetGotItemFlag(Item.Type.Shovel, true);

            // 表示させるヒントを切り替える
            HintManager.Instance.SetHintText();
        }
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool solvedGimmick = SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.Candle);
        bool gotItem = SaveManager.Instance.GetGotItemFlag(Item.Type.Shovel);

        if (solvedGimmick == true && gotItem == true)
        {
            objectCandlestick.SetActive(false);
            ShowImageAfterGettingShovel();
        }
        else if (solvedGimmick == true && gotItem == false)
        {
            objectCandlestick.SetActive(false);
            ShowImageAfterDroppingShovel();
            collisionShovel.SetActive(true);
        }
    }

    // スコップ落下後の背景を表示させる
    public void ShowImageAfterDroppingShovel()
    {
        shovelDrop.SetActive(true);
        // 遠景
        caveIgnition01.SetActive(true);
        // 近景
        axeAndDropShovel.SetActive(true);
    }

    // スコップ取得後の背景を表示させる
    void ShowImageAfterGettingShovel()
    {
        afterGettingShovel.SetActive(true);
        // 遠景
        caveIgnition02.SetActive(true);
        // 近景
        axe.SetActive(true);
    }
}
