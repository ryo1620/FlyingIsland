using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : SingletonMonoBehaviour<HintManager>
{
    // ヒントのテキストを格納する配列
    public GameObject[] hintTexts;

    void Start()
    {
        SetHintText();
    }

    // 適切なヒントのテキストをアクティブにする関数
    // ゲーム開始時や仕掛けを解いたとき、特定のアイテムを取得したときに呼ばれる
    public void SetHintText()
    {
        foreach (GameObject hintText in hintTexts)
        {
            hintText.SetActive(false);
        }
        hintTexts[CheckHintNumber()].SetActive(true);

        // リワード広告が再び表示されるように変数を操作する
        AdHint.Instance.wasSeen = false;
    }

    // 現在表示するべきヒントの番号を返す関数    
    int CheckHintNumber()
    {
        // まだ墓の仕掛けを解いていない場合
        if (SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.Grave) == false)
        {
            return 0;
        }
        // まだバッテリーをゴーレムに使っていない場合
        if (SaveManager.Instance.GetUsedItemFlag(Item.Type.Battery) == false)
        {
            return 1;
        }
        // 石棺の仕掛けを解いていない場合
        if (SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.StoneCoffin) == false)
        {
            return 2;
        }
        // 石版の仕掛けを解いていない場合
        if (SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.Lithograph) == false)
        {
            return 3;
        }
        // 石箱の仕掛けを解いていない場合
        if (SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.StoneBox) == false)
        {
            return 4;
        }
        // たいまつに火をつけていない場合
        if (SaveManager.Instance.GetUsedItemFlag(Item.Type.Torch) == false)
        {
            return 5;
        }
        // 燭台の仕掛けを解いていない場合
        if (SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.Candle) == false)
        {
            return 6;
        }
        // スコップを取得していない場合
        if (SaveManager.Instance.GetGotItemFlag(Item.Type.Shovel) == false)
        {
            return 7;
        }
        // スコップを使っていない場合
        if (SaveManager.Instance.GetUsedItemFlag(Item.Type.Shovel) == false)
        {
            return 8;
        }
        // 巣箱の仕掛けを解いていない場合
        if (SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.BirdHouse) == false)
        {
            return 9;
        }
        // 宝箱を開けていない場合
        if (SaveManager.Instance.GetUsedItemFlag(Item.Type.Key) == false)
        {
            return 10;
        }
        // 木箱（ピーナッツ）の仕掛けを解いていない場合
        if (SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.CratePeanuts) == false)
        {
            return 11;
        }
        // ピーナッツを小鳥にあげていない場合
        if (SaveManager.Instance.GetUsedItemFlag(Item.Type.Peanuts) == false)
        {
            return 12;
        }
        // 木箱（矢）の仕掛けを解いていない場合
        if (SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.CrateArrow) == false)
        {
            return 13;
        }
        // 弓と矢を組み合わせていない場合
        if (SaveManager.Instance.GetGotItemFlag(Item.Type.BowAndArrow) == false)
        {
            return 14;
        }
        // 弓矢を使っていない場合
        if (SaveManager.Instance.GetUsedItemFlag(Item.Type.BowAndArrow) == false)
        {
            return 15;
        }
        // 網を取得していない場合
        if (SaveManager.Instance.GetGotItemFlag(Item.Type.Net) == false)
        {
            return 16;
        }
        // 網を使っていない場合
        if (SaveManager.Instance.GetUsedItemFlag(Item.Type.Net) == false)
        {
            return 17;
        }
        // 木箱（石のカギ）の仕掛けを解いていない場合
        if (SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.CrateStoneKey) == false)
        {
            return 18;
        }
        // デバイスの仕掛けを解いていない場合
        if (SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.Device) == false)
        {
            return 19;
        }
        // オノを取得していない場合
        if (SaveManager.Instance.GetGotItemFlag(Item.Type.Axe) == false)
        {
            return 20;
        }
        // オノを使っていない場合
        if (SaveManager.Instance.GetUsedItemFlag(Item.Type.Axe) == false)
        {
            return 21;
        }
        // 石台の仕掛けを解いていない場合
        if (SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.StoneHolder) == false)
        {
            return 22;
        }
        // プレートの仕掛けを解いていない場合
        if (SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.Plate) == false)
        {
            return 23;
        }
        // 望遠鏡の仕掛けを解いていない場合
        if (SaveManager.Instance.GetSolvedGimmickFlag(Gimmick.Type.Telescope) == false)
        {
            return 24;
        }
        return 25;
    }
}
