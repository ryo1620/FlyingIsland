using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUIManager : MonoBehaviour
{
    // 表示させるUIを取得
    public GameObject startButton;
    public GameObject continueButton;
    public GameObject startPanel;

    // PlayerPrefsで使用するキー（削除用）
    const string SAVE_KEY = "SAVE_DATA";
    const string BOX00_ITEM_KEY = "BOX00_ITEM_DATA";
    const string BOX01_ITEM_KEY = "BOX01_ITEM_DATA";
    const string BOX02_ITEM_KEY = "BOX02_ITEM_DATA";
    const string BOX03_ITEM_KEY = "BOX03_ITEM_DATA";
    const string BOX04_ITEM_KEY = "BOX04_ITEM_DATA";
    const string SELECTED_BOX_KEY = "SELECTED_BOX_DATA";
    const string SELECTED_ITEM_KEY = "SELECTED_ITEM_DATA";

    void Start()
    {
        // セーブデータがあれば「はじめから」ボタンを下にずらし、「つづきから」ボタンを表示させる
        if (PlayerPrefs.HasKey(SAVE_KEY) == true)
        {
            MoveStartButtonDown();
            ShowContinueButton();
        }
    }

    void MoveStartButtonDown()
    {
        Transform myTransform = startButton.transform;
        Vector3 localPos = myTransform.localPosition;
        localPos.y -= 84;
        myTransform.localPosition = localPos;
    }

    void ShowContinueButton()
    {
        continueButton.SetActive(true);
    }

    public void OnStartButton()
    {
        // セーブデータがあれば確認画面を表示させる
        if (PlayerPrefs.HasKey(SAVE_KEY) == true)
        {
            TitleSEManager.Instance.SoundSelect();
            startPanel.SetActive(true);
        }
        else
        {
            PlayGame();
        }
    }

    public void OnContinueButton()
    {
        PlayGame();
    }

    public void OnYes()
    {
        DeleteSave();
        PlayGame();
    }

    public void OnNo()
    {
        TitleSEManager.Instance.SoundCancel();
        startPanel.SetActive(false);
    }

    void PlayGame()
    {
        TitleSEManager.Instance.SoundStart();
        TitleFadeManager.Instance.FadeOutScene("Game");
    }

    void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.DeleteKey(BOX00_ITEM_KEY);
        PlayerPrefs.DeleteKey(BOX01_ITEM_KEY);
        PlayerPrefs.DeleteKey(BOX02_ITEM_KEY);
        PlayerPrefs.DeleteKey(BOX03_ITEM_KEY);
        PlayerPrefs.DeleteKey(BOX04_ITEM_KEY);
        PlayerPrefs.DeleteKey(SELECTED_BOX_KEY);
        PlayerPrefs.DeleteKey(SELECTED_ITEM_KEY);
    }
}
