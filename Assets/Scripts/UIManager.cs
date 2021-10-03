using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// シングルトン化する
public class UIManager : SingletonMonoBehaviour<UIManager>
{
    // 表示させるUIを取得
    public GameObject uiTop;
    public GameObject uiBottom;
    public GameObject itemPanel;
    public GameObject menuPanel;
    public GameObject titlePanel;
    public GameObject hintPanel;
    public GameObject showHintPanel;
    public GameObject errorPanel;
    public GameObject soundPanel;
    public GameObject askPanel;
    public GameObject howToPanel;

    // 表示させるアイテム（大）を取得        
    public GameObject[] windowItems;

    // 現在表示中のアイテムを格納するための変数
    [System.NonSerialized] public GameObject currentWindowItem = null;

    // サウンド画面のチェックボックスを操作するためにゲームオブジェクトを取得する
    public GameObject checkBgm;
    public GameObject unCheckBgm;
    public GameObject checkSE;
    public GameObject unCheckSE;

    // BGMを再生・停止するためにBgmのゲームオブジェクトを取得する
    public GameObject bgm;
    // BgmのAudioSource用の変数を宣言する
    AudioSource bgmAudioSource;

    // 広告の処理用にBGM設定のon/offを判定するための変数を宣言する
    public bool bgmPlayed = true;

    // アイテムウィンドウが表示中かどうか判別するための変数
    [System.NonSerialized] public bool itemWindowIsShown = false;

    // アイテムウィンドウで石の鍵を切替えるための画像を取得する
    public GameObject stoneKey00;
    public GameObject stoneKey01;

    // PlayerPrefsで使用するキー
    const string SAVE_BGM_KEY = "BGM";
    const string SAVE_SE_KEY = "SE";
    const string SAVE_KEY = "SAVE_DATA";

    // 遊び方の解説画面におけるゲームオブジェクトを取得する
    public GameObject howToUITop;
    public GameObject howToNextButton;
    public GameObject howToBeforeButton;
    public GameObject[] howToContents;

    // 遊び方の解説画面におけるページ数を操作するための変数    
    int howToPageCount = 0;

    void Start()
    {
        // BGMの設定によってサウンド画面のチェック状態を変更する
        int bgmSetting = PlayerPrefs.GetInt(SAVE_BGM_KEY, 1);

        // オフのときはチェックを外す
        if (bgmSetting == 0)
        {
            checkBgm.SetActive(false);
            unCheckBgm.SetActive(true);
        }

        // SEの設定によってサウンド画面のチェック状態を変更する
        int seSetting = PlayerPrefs.GetInt(SAVE_SE_KEY, 1);

        // オフのときはチェックを外す
        if (seSetting == 0)
        {
            checkSE.SetActive(false);
            unCheckSE.SetActive(true);
        }

        bgmAudioSource = bgm.GetComponent<AudioSource>();

        SetItemWindowSize();

        // セーブデータがない場合、遊び方を見るかどうか確認する
        if (PlayerPrefs.HasKey(SAVE_KEY) == false)
        {
            HideMainUI();
            askPanel.SetActive(true);
        }

        SetItemWhindowSize();
    }

    // アスペクト比が3:5よりも横長であればアイテムウィンドウのサイズを小さくする
    void SetItemWindowSize()
    {
        // アスペクト比を取得する
        float aspectRatio = (float)Screen.width / (float)Screen.height;

        if (aspectRatio > 0.6)
        {
            float itemPanelWidth = 407.0f;
            float itemPanelHeight = 407.0f;
            itemPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(itemPanelWidth, itemPanelHeight);
        }
    }

    // インタースティシャル広告を表示させるために変数を操作する関数
    void SetHintAdShownToFalse()
    {
        bool hintAdShown = AdHint.Instance.adShown;

        if (hintAdShown == true)
        {
            AdHint.Instance.SetAdShownToFalse();
        }
    }

    // メインUI（メニューバー・アイテムボックス・矢印）を表示させる関数
    public void ShowMainUI()
    {
        uiTop.SetActive(true);

        // アイテムウィンドウが表示されていなければ矢印を表示する
        if (itemWindowIsShown == false)
        {
            uiBottom.SetActive(true);
        }
    }

    // メインUI（メニューバー・アイテムボックス・矢印）を非表示にする関数
    public void HideMainUI()
    {
        uiTop.SetActive(false);
        uiBottom.SetActive(false);
    }

    // ゲーム画面において「メニューバー」を押したときの処理
    public void OnMenuBar()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.select);
        HideMainUI();
        menuPanel.SetActive(true);
    }

    // メニュー画面において「タイトル」を押したときの処理
    public void OnTitle()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.select);
        SetHintAdShownToFalse();
        menuPanel.SetActive(false);
        titlePanel.SetActive(true);
    }

    // メニュー画面において「ヒント」を押したときの処理
    public void OnHint()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.select);
        SetHintAdShownToFalse();
        menuPanel.SetActive(false);
        // すでに広告を見ているかどうかで処理を分ける
        if (AdHint.Instance.wasSeen == true)
        {
            showHintPanel.SetActive(true);
        }
        else
        {
            hintPanel.SetActive(true);
        }
    }

    // メニュー画面において「サウンド」を押したときの処理
    public void OnSound()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.select);
        SetHintAdShownToFalse();
        menuPanel.SetActive(false);
        soundPanel.SetActive(true);
    }

    // メニュー画面において「遊び方」を押したときの処理
    public void OnHowTo()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.select);
        SetHintAdShownToFalse();
        menuPanel.SetActive(false);
        howToPanel.SetActive(true);
    }

    // メニュー画面において「戻る」を押したときの処理
    public void OnReturnInMenuPanel()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.cancel);
        SetHintAdShownToFalse();
        menuPanel.SetActive(false);
        ShowMainUI();
    }

    // ヒントを表示する画面において「戻る」を押したときの処理
    public void OnReturnInShowHintPanel()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.cancel);
        SetHintAdShownToFalse();
        showHintPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    // エラー画面において「戻る」を押したときの処理
    public void OnReturnInErrorPanel()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.cancel);
        errorPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    // サウンド画面において「戻る」を押したときの処理
    public void OnReturnInSoundPanel()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.cancel);
        soundPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    // 遊び方画面において「戻る」を押したときの処理
    public void OnReturnInHowToPanel()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.cancel);
        howToPanel.SetActive(false);
        menuPanel.SetActive(true);
        SetHowToContentDefault();
    }

    // タイトルに戻るかどうか確認する画面において「いいえ」を押したときの処理
    public void OnNoInTitlePanel()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.cancel);

        titlePanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    // ヒントを見るかどうか確認する画面において「いいえ」を押したときの処理
    public void OnNoInHintPanel()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.cancel);

        hintPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    // ヒントを表示する画面において「閉じる」を押したときの処理
    public void OnCloseInShowHintPanel()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.cancel);
        SetHintAdShownToFalse();
        showHintPanel.SetActive(false);
        ShowMainUI();
    }

    // エラー画面において「閉じる」を押したときの処理
    public void OnCloseInErrorPanel()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.cancel);
        errorPanel.SetActive(false);
        ShowMainUI();
    }

    // サウンド画面において「BGM」をチェックしたときの処理
    public void CheckBgm()
    {
        unCheckBgm.SetActive(false);
        checkBgm.SetActive(true);

        bgmPlayed = true;

        // 設定をセーブする
        PlayerPrefs.SetInt(SAVE_BGM_KEY, 1);
        PlayerPrefs.Save();

        // BGMを再生する        
        bgmAudioSource.Play();
    }

    // サウンド画面において「BGM」のチェックを外したときの処理
    public void UnCheckBgm()
    {
        checkBgm.SetActive(false);
        unCheckBgm.SetActive(true);

        bgmPlayed = false;

        // 設定をセーブする
        PlayerPrefs.SetInt(SAVE_BGM_KEY, 0);
        PlayerPrefs.Save();

        // BGMを停止する        
        bgmAudioSource.Stop();
    }

    // サウンド画面において「SE」をチェックしたときの処理
    public void CheckSE()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.select);
        unCheckSE.SetActive(false);
        checkSE.SetActive(true);

        // 設定をセーブする
        PlayerPrefs.SetInt(SAVE_SE_KEY, 1);
        PlayerPrefs.Save();

        // SEを再生する
        SEManager.Instance.TurnOnSE();
    }

    // サウンド画面において「SE」のチェックを外したときの処理    
    public void UnCheckSE()
    {
        checkSE.SetActive(false);
        unCheckSE.SetActive(true);

        // 設定をセーブする
        PlayerPrefs.SetInt(SAVE_SE_KEY, 0);
        PlayerPrefs.Save();

        // SEを停止する
        SEManager.Instance.TurnOffSE();
    }

    // 遊び方を見るかどうか確認する画面で「はい」を押したときの処理
    public void OnYesInAskPanel()
    {
        // 初回の遊び方の解説画面では戻るボタンを非表示にする
        howToUITop.SetActive(false);
        SEManager.Instance.PlaySE(SEManager.Instance.select);
        askPanel.SetActive(false);
        howToPanel.SetActive(true);
    }

    // 遊び方を見るかどうか確認する画面で「いいえ」を押したときの処理
    public void OnNoInAskPanel()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.cancel);
        askPanel.SetActive(false);
        ShowMainUI();
    }

    // 遊び方画面で「次へ」を押したときの処理
    public void OnNextInHowToPanel()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.select);
        howToContents[howToPageCount].SetActive(false);
        howToPageCount += 1;
        howToContents[howToPageCount].SetActive(true);

        if (howToPageCount == 1)
        {
            howToBeforeButton.SetActive(true);
        }
        if (howToPageCount == 4)
        {
            howToNextButton.SetActive(false);
        }
    }

    // 遊び方画面で「前へ」を押したときの処理
    public void OnBeforeInHowToPanel()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.cancel);
        howToContents[howToPageCount].SetActive(false);
        howToPageCount -= 1;
        howToContents[howToPageCount].SetActive(true);

        if (howToPageCount == 0)
        {
            howToBeforeButton.SetActive(false);
        }
        if (howToPageCount == 3)
        {
            howToNextButton.SetActive(true);
        }
    }

    // 遊び方画面で「閉じる」を押したときの処理
    public void OnCloseInHowToPanel()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.cancel);
        howToPanel.SetActive(false);
        ShowMainUI();
        if (howToUITop.activeSelf == false)
        {
            howToUITop.SetActive(true);
        }
        SetHowToContentDefault();
    }

    // 遊び方画面の状態を初期化する処理
    void SetHowToContentDefault()
    {
        howToPageCount = 0;
        howToContents[0].SetActive(true);
        for (int i = 1; i <= 4; i++)
        {
            howToContents[i].SetActive(false);
        }
        howToBeforeButton.SetActive(false);
        howToNextButton.SetActive(true);
    }

    // アイテムウィンドウを表示させる関数
    public void ShowItemWindow(GameObject item)
    {
        uiBottom.SetActive(false);

        // アイテムウィンドウがすでに表示中かどうかで処理を分ける
        if (itemWindowIsShown)
        {
            currentWindowItem.SetActive(false);
        }
        else
        {
            itemWindowIsShown = true;
            itemPanel.SetActive(true);
        }

        item.SetActive(true);

        // 閉じるときにどのアイテムを非表示にするか判断するため、変数に表示したアイテムを代入する
        currentWindowItem = item;
    }

    // アイテムウィンドウを閉じる関数
    public void CloseItemWindow()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.cancel);
        itemWindowIsShown = false;
        uiBottom.SetActive(true);
        currentWindowItem.SetActive(false);
        itemPanel.SetActive(false);
        currentWindowItem = null;
    }

    // アイテムウィンドウで石の鍵をタップしたときに画像を切り替える
    public void OnStoneKey()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.select);

        if (stoneKey00.activeSelf == true)
        {
            stoneKey01.SetActive(true);
            stoneKey00.SetActive(false);
        }
        else
        {
            stoneKey00.SetActive(true);
            stoneKey01.SetActive(false);
        }
    }

    // デバイスのアスペクト比によってアイテムウィンドウのサイズを変える
    void SetItemWhindowSize()
    {
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        if (aspectRatio > 0.65f)
        {
            itemPanel.transform.localPosition = new Vector3(4, 0, 0);
            itemPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(408, 408);
        }
    }
}
