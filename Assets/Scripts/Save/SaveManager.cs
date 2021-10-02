using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// シングルトン化する
public class SaveManager : SingletonMonoBehaviour<SaveManager>
{
    SaveData saveData = new SaveData();

    // PlayerPrefsで使用するキー
    const string SAVE_KEY = "SAVE_DATA";

    void Start()
    {
        Load();
    }

    // セーブする関数
    public void Save()
    {
        // クラスをJson化する
        string json = JsonUtility.ToJson(saveData);
        // 文字列にしたもの（Json）を保存する
        PlayerPrefs.SetString(SAVE_KEY, json);
    }

    // ロードする
    public void Load()
    {
        // セーブデータが存在するかどうかで処理を分ける
        if (PlayerPrefs.HasKey(SAVE_KEY) == true)
        {
            // 保存データのJsonを取得する
            string json = PlayerPrefs.GetString(SAVE_KEY);
            // Jsonからセーブデータを復元する
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            SaveData saveData = new SaveData();
        }
    }

    // 仕掛けを解くためのフラグが立ったことをセーブする
    public void SetCanSolveGimmickFlag(Gimmick.Type gimmick, bool flag)
    {
        saveData.canSolveGimmickFlags[(int)gimmick] = flag;
        Save();
    }

    // 仕掛けを解いたことをセーブする
    public void SetSolvedGimmickFlag(Gimmick.Type gimmick, bool flag)
    {
        saveData.solvedGimmickFlags[(int)gimmick] = flag;
        Save();
    }

    // アイテムを取得したことをセーブする
    public void SetGotItemFlag(Item.Type item, bool flag)
    {
        saveData.gotItemFlags[(int)item] = flag;
        Save();
    }

    // アイテムを使用したことをセーブする
    public void SetUsedItemFlag(Item.Type item, bool flag)
    {
        saveData.usedItemFlags[(int)item] = flag;
        Save();
    }

    // 仕掛けを解くためのフラグが立ったかどうかを取得する
    public bool GetCanSolveGimmickFlag(Gimmick.Type gimmick)
    {
        return saveData.canSolveGimmickFlags[(int)gimmick];
    }

    // 仕掛けを解いたかどうかを取得する
    public bool GetSolvedGimmickFlag(Gimmick.Type gimmick)
    {
        return saveData.solvedGimmickFlags[(int)gimmick];
    }

    // アイテムを取得したかどうかを取得する
    public bool GetGotItemFlag(Item.Type item)
    {
        return saveData.gotItemFlags[(int)item];
    }

    // アイテムを使用したかどうかを取得する
    public bool GetUsedItemFlag(Item.Type item)
    {
        return saveData.usedItemFlags[(int)item];
    }
}

// セーブ形式を考える：SaveDataクラスを作成する
// true/falseの配列
public class SaveData
{
    // 仕掛けを解くためのフラグが立っているかどうか
    public bool[] canSolveGimmickFlags = new bool[(int)Gimmick.Type.Total];
    // 仕掛けを解いたかどうか
    public bool[] solvedGimmickFlags = new bool[(int)Gimmick.Type.Total];
    // アイテムを取得したかどうか
    public bool[] gotItemFlags = new bool[(int)Item.Type.Total];
    // アイテムを使用したかどうか
    public bool[] usedItemFlags = new bool[(int)Item.Type.Total];
}