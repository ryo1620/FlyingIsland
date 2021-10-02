using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// シングルトン化する
public class Device : SingletonMonoBehaviour<Device>
{
    // 表示している画像    
    public Image[] images;
    // 画像のソース    
    public Sprite[] imageSources;

    // 鍵を使ったときに切り替える画像を取得する
    public GameObject device;
    public GameObject deviceWithStoneKey;

    // オノを取得したときに表示する画像を取得する    
    public GameObject axeOpen;
    public GameObject axeDrop;
    public GameObject afterGettingAxe;
    // 近景用            
    public GameObject dropAxe;
    // 遠景用        
    public GameObject caveIgnited03;
    public GameObject caveIgnited04;

    // 正解時にボタンの受付を無効にするための変数
    public GameObject[] collisions;

    // オノのロックを解除したあと、デバイスへの移動を制限するためにオブジェクトを取得する
    public GameObject objectDevice;

    // オノをUIの表示後に取得できるようにするため、当たり判定のオブジェクトを取得する
    public GameObject collisionAxe;

    enum Alphabet
    {
        G,
        I,
        O,
        P,
        Q,
    }
    Alphabet[] currentAlphabets = new Alphabet[3];

    void Start()
    {
        LoadImage();
    }

    // ボタンを押したときの処理
    public void OnButton(int buttonNumber)
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.tap);
            ChangeDirection(buttonNumber);
            ShowDirectionImage(buttonNumber);
        }
    }

    // 鍵穴をタップしたときの処理
    public void OnKeyHole()
    {
        if (ItemBoxManager.Instance.IsSelected(Item.Type.StoneKey) && UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.insertStoneKey);
            deviceWithStoneKey.SetActive(true);
            // 操作の受付を無効にする
            UIManager.Instance.HideMainUI();
            foreach (GameObject collision in collisions)
            {
                collision.SetActive(false);
            }

            // コルーチンの起動
            StartCoroutine(this.DelayCoroutine(0.5f, () =>
            {
                if (IsCorrect())
                {
                    SolveGimmick();
                }
                else
                {
                    mistakeGimmick();
                }
            }));
        }
    }

    // Alphabet変数を変更する
    void ChangeDirection(int buttonNumber)
    {
        if (currentAlphabets[buttonNumber] == Alphabet.Q)
        {
            currentAlphabets[buttonNumber] = Alphabet.G;
        }
        else
        {
            currentAlphabets[buttonNumber]++;
        }
    }

    // Alphabet変数に応じた画像を表示する
    void ShowDirectionImage(int buttonNumber)
    {
        images[buttonNumber].sprite = imageSources[(int)currentAlphabets[buttonNumber]];
    }

    // 正誤判定する
    bool IsCorrect()
    {
        if (currentAlphabets[0] == Alphabet.G && currentAlphabets[1] == Alphabet.I && currentAlphabets[2] == Alphabet.Q)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 正解したときの処理
    void SolveGimmick()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.correct);

        // 状態をセーブする
        SaveManager.Instance.SetSolvedGimmickFlag(Gimmick.Type.Device, true);

        // アイテムを削除する
        ItemBoxManager.Instance.DeleteItem(Item.Type.StoneKey);

        // 表示させるヒントを切り替える
        HintManager.Instance.SetHintText();

        axeOpen.SetActive(true);

        StartCoroutine(this.DelayCoroutine(0.5f, () =>
        {
            // フェードイン・アウトでオノの前まで移動し、ロック解除時の画像を表示させる
            IEnumerator coroutine = PanelChanger.Instance.UnlockCoroutine(Item.Type.Axe);
            StartCoroutine(coroutine);
        }));
    }

    // 不正解のときの処理
    void mistakeGimmick()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.incorrect);

        StartCoroutine(this.DelayCoroutine(0.5f, () =>
        {
            deviceWithStoneKey.SetActive(false);

            foreach (GameObject collision in collisions)
            {
                collision.SetActive(true);
            }

            UIManager.Instance.ShowMainUI();
        }));
    }

    // オノを取得したときの処理
    public void GetItem()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.getItem);

            ShowImageAfterGettingAxe();

            UIManager.Instance.ShowItemWindow(UIManager.Instance.windowItems[(int)Item.Type.Axe]);
            ItemBoxManager.Instance.SetItem(Item.Type.Axe);
            // 状態をセーブする
            SaveManager.Instance.SetGotItemFlag(Item.Type.Axe, true);

            // 表示させるヒントを切り替える
            HintManager.Instance.SetHintText();
        }
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool solvedGimmick = SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.Device);
        bool gotItem = SaveManager.Instance.GetGotItemFlag(Item.Type.Axe);

        if (solvedGimmick == true && gotItem == true)
        {
            objectDevice.SetActive(false);
            ShowImageAfterGettingAxe();
        }
        else if (solvedGimmick == true && gotItem == false)
        {
            objectDevice.SetActive(false);
            ShowImageAfterDroppingAxe();
            collisionAxe.SetActive(true);
        }
    }

    // オノ落下後の背景を表示させる
    public void ShowImageAfterDroppingAxe()
    {
        axeDrop.SetActive(true);
        // 遠景
        caveIgnited03.SetActive(true);
        // 近景
        dropAxe.SetActive(true);
    }

    // オノ取得後の背景を表示させる
    public void ShowImageAfterGettingAxe()
    {
        afterGettingAxe.SetActive(true);
        // 遠景
        caveIgnited04.SetActive(true);
        // 近景
        dropAxe.SetActive(true);
    }
}
