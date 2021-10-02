using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// シングルトン化する
public class ItemBoxManager : SingletonMonoBehaviour<ItemBoxManager>
{
    public GameObject[] frames;
    public GameObject[] boxItems00;
    public GameObject[] boxItems01;
    public GameObject[] boxItems02;
    public GameObject[] boxItems03;
    public GameObject[] boxItems04;
    public GameObject[][] itemBoxes;

    // それぞれのアイテムボックスの中に現在格納されているアイテムを判別するための変数
    [System.NonSerialized] public GameObject[] currentBoxItems = new GameObject[5];

    // eventSystemを取得するための変数
    [SerializeField] EventSystem eventSystem = default;

    // 選択されたゲームオブジェクトを取得するための変数
    // 条件判定に使うため、null許容値型にする   
    GameObject selectedObj;
    int? selectedItemBoxNumber = null;
    int? selectedItemNumber = null;

    // 選択されたアイテム画像を取得するための変数（選択時に透明度を変更するため）
    Image selectedItemImage;

    // 不透明・半透明
    Color opaqueColor = new Color(255, 255, 255, 1);
    Color translucentColor = new Color(255, 255, 255, 0.6f);

    // アイテムがすでに選択されているかどうか判別するための変数
    bool itemIsSelected = false;

    // 選択中でないアイテムを削除するときに使う変数
    int unselectedItemBoxNumber;

    // アイテム合成時にアイテムウィンドウの閉じるボタンを非表示にするため、ゲームオブジェクトを取得する
    public GameObject closeButton;

    // PlayerPrefsで使用するキー
    const string BOX00_ITEM_KEY = "BOX00_ITEM_DATA";
    const string BOX01_ITEM_KEY = "BOX01_ITEM_DATA";
    const string BOX02_ITEM_KEY = "BOX02_ITEM_DATA";
    const string BOX03_ITEM_KEY = "BOX03_ITEM_DATA";
    const string BOX04_ITEM_KEY = "BOX04_ITEM_DATA";
    static readonly string[] BOX_ITEM_KEYS = new string[] { BOX00_ITEM_KEY, BOX01_ITEM_KEY, BOX02_ITEM_KEY, BOX03_ITEM_KEY, BOX04_ITEM_KEY };
    const string SELECTED_BOX_KEY = "SELECTED_BOX_DATA";
    const string SELECTED_ITEM_KEY = "SELECTED_ITEM_DATA";

    void Start()
    {
        itemBoxes = new GameObject[][] { boxItems00, boxItems01, boxItems02, boxItems03, boxItems04 };

        // ロード時、アイテムボックスの状態を復元する                
        SetItemWhenLoaded();
        if (PlayerPrefs.HasKey(SELECTED_BOX_KEY) && PlayerPrefs.HasKey(SELECTED_ITEM_KEY))
        {
            selectedItemBoxNumber = PlayerPrefs.GetInt(SELECTED_BOX_KEY);
            selectedItemNumber = PlayerPrefs.GetInt(SELECTED_ITEM_KEY);
            SelectItemWhenLoaded((int)selectedItemBoxNumber, (int)selectedItemNumber);
        }
    }

    // セーブ内容をもとにアイテムを格納する
    void SetItemWhenLoaded()
    {
        int index = 0;

        foreach (string BOX_ITEM_KEY in BOX_ITEM_KEYS)
        {
            if (PlayerPrefs.HasKey(BOX_ITEM_KEY))
            {
                int boxItemNumber = PlayerPrefs.GetInt(BOX_ITEM_KEY);
                itemBoxes[index][boxItemNumber].SetActive(true);
                currentBoxItems[index] = itemBoxes[index][boxItemNumber];
            }

            index += 1;
        }
    }

    // セーブ内容をもとにアイテムを選択する
    void SelectItemWhenLoaded(int itemBoxNumber, int itemNumber)
    {
        // 選択されたアイテムのゲームオブジェクトを取得する
        selectedObj = itemBoxes[itemBoxNumber][itemNumber];

        // 選択されたアイテムの画像を取得する        
        selectedItemImage = selectedObj.GetComponent<Image>();

        // 白枠を表示させる                      
        frames[itemBoxNumber].SetActive(true);

        // アイテム画像を不透明にする
        selectedItemImage.color = opaqueColor;

        itemIsSelected = true;
    }

    // 空のアイテムボックスのうち、最も左側にアイテムを格納する
    public void SetItem(Item.Type item)
    {
        int setItemBoxNumber = SearchEmptyItemBoxNumber();
        itemBoxes[setItemBoxNumber][(int)item].SetActive(true);
        currentBoxItems[setItemBoxNumber] = itemBoxes[setItemBoxNumber][(int)item];

        // 状態をセーブする
        PlayerPrefs.SetInt(BOX_ITEM_KEYS[setItemBoxNumber], (int)item);
    }

    // 空のアイテムボックスを検索する
    int SearchEmptyItemBoxNumber()
    {
        // 初期化
        int emptyItemBoxNumber = 0;

        foreach (GameObject currentBoxItem in currentBoxItems)
        {
            if (currentBoxItem)
            {
                emptyItemBoxNumber += 1;
            }
            else
            {
                break;
            }
        }

        return emptyItemBoxNumber;
    }

    // アイテムを選択する
    public void SelectItem(int itemBoxNumber)
    {
        SEManager.Instance.PlaySE(SEManager.Instance.select);

        // 白枠がすでに表示されていればアイテムウィンドウを表示させ、そうでなければ白枠を表示させる
        if (frames[itemBoxNumber].activeSelf)
        {
            // インスペクターで設定した引数に応じてアイテムウィンドウを表示させる
            UIManager.Instance.ShowItemWindow(UIManager.Instance.windowItems[(int)selectedItemNumber]);
        }
        else
        {
            // すでに他のアイテムが選択されている場合、その白枠を非表示にしてアイテム画像を半透明にする
            if (itemIsSelected)
            {
                Image previousItemImage = selectedItemImage;
                int previousFrameNumber = (int)selectedItemBoxNumber;
                frames[previousFrameNumber].SetActive(false);
                previousItemImage.color = translucentColor;
            }

            // 選択されたアイテムのゲームオブジェクトを取得する
            selectedObj = eventSystem.currentSelectedGameObject.gameObject;

            // 選択されたアイテムの番号・画像および白枠の番号を取得する
            selectedItemNumber = Array.IndexOf(itemBoxes[itemBoxNumber], selectedObj);
            selectedItemImage = selectedObj.GetComponent<Image>();
            selectedItemBoxNumber = itemBoxNumber;

            // 白枠を表示させる                      
            frames[itemBoxNumber].SetActive(true);

            // アイテム画像を不透明にする
            selectedItemImage.color = opaqueColor;

            itemIsSelected = true;

            // 状態をセーブする
            PlayerPrefs.SetInt(SELECTED_BOX_KEY, (int)selectedItemBoxNumber);
            PlayerPrefs.SetInt(SELECTED_ITEM_KEY, (int)selectedItemNumber);
        }
    }

    // 特定のアイテムが選択中かどうか判別する
    public bool IsSelected(Item.Type item)
    {
        if ((int)item == selectedItemNumber)
        {
            return true;
        }
        return false;
    }

    // 特定のアイテムがいずれかのアイテムボックスに存在するかどうか確認する
    public bool ItemExists(Item.Type item)
    {
        unselectedItemBoxNumber = 0;

        foreach (GameObject currentBoxItem in currentBoxItems)
        {
            if (itemBoxes[unselectedItemBoxNumber][(int)item] == currentBoxItem)
            {
                return true;
            }

            unselectedItemBoxNumber += 1;
        }
        return false;
    }

    // 特定のアイテムをアイテムボックスから削除する
    public void DeleteItem(Item.Type item)
    {
        int deletedItemBoxNumber;

        // if：削除するアイテムが選択中の場合の処理        
        if ((int)item == selectedItemNumber)
        {
            deletedItemBoxNumber = (int)selectedItemBoxNumber;

            frames[deletedItemBoxNumber].SetActive(false);
            itemBoxes[deletedItemBoxNumber][(int)item].SetActive(false);

            // 各変数の値をnullにする
            currentBoxItems[deletedItemBoxNumber] = null;
            selectedObj = null;
            selectedItemBoxNumber = null;
            selectedItemNumber = null;
            selectedItemImage = null;

            itemIsSelected = false;

            // 状態をセーブする
            PlayerPrefs.DeleteKey(BOX_ITEM_KEYS[deletedItemBoxNumber]);
            PlayerPrefs.DeleteKey(SELECTED_BOX_KEY);
            PlayerPrefs.DeleteKey(SELECTED_ITEM_KEY);
        }
        // else：削除するアイテムが選択中でない場合の処理
        else if (ItemExists(item))
        {
            deletedItemBoxNumber = unselectedItemBoxNumber;

            itemBoxes[deletedItemBoxNumber][(int)item].SetActive(false);
            currentBoxItems[deletedItemBoxNumber] = null;

            // 状態をセーブする
            PlayerPrefs.DeleteKey(BOX_ITEM_KEYS[deletedItemBoxNumber]);
        }
    }

    // インスペクターに表示させるため、弓と矢を合成するコルーチンをラップする
    public void SynthesizeBowAndArrow(int itemNumberToSynthesize)
    {
        IEnumerator coroutine = SynthesizeBowAndArrowCoroutine(itemNumberToSynthesize);
        StartCoroutine(coroutine);
    }

    // 弓と矢を合成するコルーチン（弓矢のアイテム番号：9）
    IEnumerator SynthesizeBowAndArrowCoroutine(int itemNumberToSynthesize)
    {
        // 引数として指定された特定のアイテムが選択されているとき
        if (selectedItemNumber == itemNumberToSynthesize)
        {
            closeButton.SetActive(false);

            // 現在アイテムウィンドウに表示されているアイテムをフェードアウトさせる
            FadeManager.Instance.FadeOutItem(UIManager.Instance.currentWindowItem);

            yield return new WaitForSeconds(0.2f);

            // 現在アイテムウィンドウに表示されているアイテムと選択中のアイテムを削除する
            DeleteItem((Item.Type)itemNumberToSynthesize);
            DeleteItem((Item.Type)Array.IndexOf(UIManager.Instance.windowItems, UIManager.Instance.currentWindowItem));

            // 弓矢をフェードインさせる
            FadeManager.Instance.FadeInItem(UIManager.Instance.windowItems[(int)Item.Type.BowAndArrow]);

            SEManager.Instance.PlaySE(SEManager.Instance.getItem);

            // 閉じるときにどのアイテムを非表示にするか判断するため、変数に表示したアイテム（弓矢のゲームオブジェクト）を代入
            UIManager.Instance.currentWindowItem = UIManager.Instance.windowItems[(int)Item.Type.BowAndArrow];

            yield return new WaitForSeconds(0.2f);

            // 弓矢を新たに取得する            
            SetItem(Item.Type.BowAndArrow);

            // 状態をセーブする（ヒント用）
            SaveManager.Instance.SetGotItemFlag(Item.Type.BowAndArrow, true);

            // 表示させるヒントを切り替える
            HintManager.Instance.SetHintText();

            closeButton.SetActive(true);
        }
    }
}
