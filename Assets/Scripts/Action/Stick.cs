using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour
{
    public GameObject StickTilted;

    // スティックをタップしたときの処理
    public void OnStick()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            IEnumerator coroutine = OnStickCoroutine();
            StartCoroutine(coroutine);
        }
    }

    IEnumerator OnStickCoroutine()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.push);
        UIManager.Instance.HideMainUI();
        StickTilted.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        StickTilted.SetActive(false);
        UIManager.Instance.ShowMainUI();
    }
}
