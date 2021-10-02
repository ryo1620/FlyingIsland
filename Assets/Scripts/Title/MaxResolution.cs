using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxResolution : MonoBehaviour
{
    // デバイス本来の解像度を保持する（広告の位置調整などに必要なため）
    public static float defaultWidth = (float)Screen.width;
    public static float defaultHeight = (float)Screen.height;

    // Start is called before the first frame update
    void Start()
    {
        // 上限を決める（これは適宜設定を外部に出して読み込むなどして対応してください）
        // オプションなどでこの値を変更できるようにするとユーザーフレンドリーかもしれません
        float maxWidth = 750.0f;
        float maxHeight = 2250.0f;

        // それぞれのオーバーしている倍率を求める
        float scaleWidth = (float)Screen.width / maxWidth;
        float scaleHeight = (float)Screen.height / maxHeight;

        // オーバーし過ぎている方から縮小率を得る
        float rate;
        if (scaleWidth > scaleHeight)
        {
            rate = scaleWidth;
        }
        else
        {
            rate = scaleHeight;
        }

        // 上限よりオーバーしていたら元々のアスペクト比を保ったまま解像度を縮小する
        if (rate > 1.0f)
        {
            // 切り上げで計算（1ドット欠けを防ぐ）
            int setWidth = Mathf.CeilToInt((float)Screen.width / rate);
            int setHeight = Mathf.CeilToInt((float)Screen.height / rate);
            Screen.SetResolution(setWidth, setHeight, true);
        }
    }

    // デバイス本来の解像度を取得するためのゲッター関数
    public static float GetDefaultWidth()
    {
        return defaultWidth;
    }

    public static float GetDefaultHeight()
    {
        return defaultHeight;
    }
}
