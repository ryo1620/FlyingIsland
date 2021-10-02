using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 拡張メソッドを定義するクラス
public static class Extends
{
    // 一定時間後に処理を呼び出すコルーチン
    public static IEnumerator DelayCoroutine(this MonoBehaviour self, float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }
}
