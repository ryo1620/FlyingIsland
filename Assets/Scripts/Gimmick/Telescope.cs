using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Telescope : MonoBehaviour
{
    // 表示している画像    
    public Image[] images;
    // 画像のソース    
    public Sprite[] imageSources;

    // 正解時に切り替える画像
    public GameObject lid;
    public GameObject finder;
    public GameObject rainbow;
    public GameObject telescopeWithLid;
    public GameObject telescopeWithFinder;
    public GameObject[] panelBackgrounds;

    // 正解時にボタンの受付を無効にするための変数
    public GameObject[] collisions;

    // 現在の番号
    enum Number
    {
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
    }
    Number[] currentNumbers = new Number[4];

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
            ChangeNumber(buttonNumber);
            ShowNumberImage(buttonNumber);
            if (IsCorrect())
            {
                // コルーチンの起動
                StartCoroutine(this.DelayCoroutine(0.5f, () =>
                {
                    if (IsCorrect())
                    {
                        SolveGimmick();
                    }
                }));
            }
        }
    }

    // Number変数を変更する
    void ChangeNumber(int buttonNumber)
    {
        if (currentNumbers[buttonNumber] == Number.Nine)
        {
            currentNumbers[buttonNumber] = Number.One;
        }
        else
        {
            currentNumbers[buttonNumber]++;
        }
    }

    // Number変数に応じた画像を表示する
    void ShowNumberImage(int buttonNumber)
    {
        images[buttonNumber].sprite = imageSources[(int)currentNumbers[buttonNumber]];
    }

    // 正誤判定する
    bool IsCorrect()
    {
        if (GimmickFlag.Instance.canSolveTelescope == true && currentNumbers[0] == Number.Five && currentNumbers[1] == Number.Two && currentNumbers[2] == Number.Four && currentNumbers[3] == Number.Seven)
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
        // 操作の受付を無効にする
        UIManager.Instance.HideMainUI();
        foreach (GameObject collision in collisions)
        {
            collision.SetActive(false);
        }

        SEManager.Instance.PlaySE(SEManager.Instance.correct);
        // ギミック解除
        StartCoroutine(this.DelayCoroutine(1.0f, () =>
        {
            SEManager.Instance.PlaySE(SEManager.Instance.openCap);
            finder.SetActive(true);
            lid.SetActive(false);
            ChangeObjectBackground();
            SelectPanelBackground();
            StartCoroutine(this.DelayCoroutine(1.0f, () =>
            {
                ShowRainbow();

                // エンジンの仕掛けを解けるようにする
                GimmickFlag.Instance.canSolveEngineStarter = true;
                SaveManager.Instance.SetCanSolveGimmickFlag(Gimmick.Type.EngineStarter, true);

                // 状態をセーブする
                SaveManager.Instance.SetSolvedGimmickFlag(Gimmick.Type.Telescope, true);

                // 表示させるヒントを切り替える
                HintManager.Instance.SetHintText();
            }));
        }));
    }

    // フェードイン・アウトで虹を表示する
    void ShowRainbow()
    {
        IEnumerator coroutine = ShowRainbowCoroutine();
        StartCoroutine(coroutine);
    }

    IEnumerator ShowRainbowCoroutine()
    {
        FadeManager.Instance.FadeOutPanel();

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        rainbow.SetActive(true);
        finder.SetActive(false);
        UIManager.Instance.ShowMainUI();

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        FadeManager.Instance.FadeInPanel();
    }

    // フラグの状態によって表示する画像を変える
    void LoadImage()
    {
        bool solvedGimmick = SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.Telescope);

        if (solvedGimmick == true)
        {
            SelectPanelBackground();
            ChangeObjectBackground();
            rainbow.SetActive(true);
            lid.SetActive(false);
        }
    }

    // 背景画像（遠景）05_0～05_3を切り替える
    // 網を取得したか（ゴーレムがいるか）どうかによって処理を分ける
    void SelectPanelBackground()
    {
        bool gotItem = SaveManager.Instance.GetGotItemFlag(Item.Type.Net);

        if (gotItem == true)
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

    // 望遠鏡の画像を切り替える
    void ChangeObjectBackground()
    {
        telescopeWithFinder.SetActive(true);
        telescopeWithLid.SetActive(false);
    }
}
