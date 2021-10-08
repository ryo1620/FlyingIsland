using System.Collections;
using UnityEngine;

// シングルトン化する
public class PanelChanger : SingletonMonoBehaviour<PanelChanger>
{
    // 同じことを繰り返し書かない：パネルの表示
    // 矢印の表示/非表示
    public GameObject rightArrow;
    public GameObject leftArrow;
    public GameObject backArrow;

    // BGMの音量を操作するためのオブジェクトを取得する
    public GameObject bgm;

    // 洞窟内でバックボタンを押したときに処理を分けるための変数
    bool isObjectInCave = false;

    // 丘の上でバックボタンを押したときに処理を分けるための変数
    bool isObjectOnHill = false;

    // 橋の周辺でバックボタンを押したときに処理を分けるための変数
    bool isObjectOnBridge = false;

    // 使うパネルとオブジェクトを列挙する
    enum Panel
    {
        Panel02,
        Panel01,
        Panel00,
        Panel10,
        Panel20,
        Cave,
        Hill,
        Bridge,
    }

    enum Object
    {
        Object00,
        Object01,
        Object02,
        Object03,
        Object04,
        Object05,
    }

    Panel currentPanel = Panel.Panel00;

    // ズーム中かどうかを判定するための変数
    bool zoomToggle00 = false;
    bool zoomToggle01 = false;

    void Start()
    {
        HideArrows();
        ShowSideArrows();
    }

    // コルーチンでフェードイン・アウトを実装する
    IEnumerator OnRightArrowCoroutine()
    {
        PlayLongFootsteps();

        FadeManager.Instance.FadeOutPanel(FadeManager.Instance.fadeInOutPanelTime);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        ShowPanel(++currentPanel);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        FadeManager.Instance.FadeInPanel();
    }

    IEnumerator OnLeftArrowCoroutine()
    {
        PlayLongFootsteps();

        FadeManager.Instance.FadeOutPanel(FadeManager.Instance.fadeInOutPanelTime);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        ShowPanel(--currentPanel);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        FadeManager.Instance.FadeInPanel();
    }

    // 洞窟に入るときのコルーチン
    IEnumerator EnterCaveCoroutine()
    {
        PlayLongFootsteps();

        // BGMの音量を徐々に下げる
        FadeManager.Instance.TurnDown();

        FadeManager.Instance.FadeOutPanel(FadeManager.Instance.fadeInOutPanelTime);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        ShowPanel(Panel.Cave);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        FadeManager.Instance.FadeInPanel();
    }

    // 洞窟から出るときのコルーチン
    IEnumerator LeaveCaveCoroutine()
    {
        PlayLongFootsteps();

        // BGMの音量を徐々に元に戻す
        FadeManager.Instance.TurnUp();

        FadeManager.Instance.FadeOutPanel(FadeManager.Instance.fadeInOutPanelTime);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        ShowPanel(Panel.Panel01);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        FadeManager.Instance.FadeInPanel();
    }

    // 丘に登るときのコルーチン
    IEnumerator ClimbHillCoroutine()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.climb);

        FadeManager.Instance.FadeOutPanel(FadeManager.Instance.fadeInOutPanelTime);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        ShowPanel(Panel.Hill);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        FadeManager.Instance.FadeInPanel();
    }

    // 丘から降りるときのコルーチン
    IEnumerator ClimbDownCoroutine()
    {
        FadeManager.Instance.FadeOutPanel(FadeManager.Instance.fadeInOutPanelTime);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        ShowPanel(Panel.Panel00);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        SEManager.Instance.PlaySE(SEManager.Instance.land);

        FadeManager.Instance.FadeInPanel();
    }

    // 望遠鏡を覗くときのコルーチン
    IEnumerator OnFinderCoroutine()
    {
        SEManager.Instance.PlaySE(SEManager.Instance.soilShort);

        FadeManager.Instance.FadeOutPanel(FadeManager.Instance.fadeInOutPanelTime);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        Transform myTransform = this.transform;
        Vector2 localPos = myTransform.localPosition;
        localPos.y *= -1.0f;
        myTransform.localPosition = localPos;
        zoomToggle00 = !zoomToggle00;

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        FadeManager.Instance.FadeInPanel();
    }

    // 橋の前に移動するときのコルーチン
    IEnumerator MoveToBridgeCoroutine()
    {
        PlayLongFootsteps();

        FadeManager.Instance.FadeOutPanel(FadeManager.Instance.fadeInOutPanelTime);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        ShowPanel(Panel.Bridge);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        FadeManager.Instance.FadeInPanel();
    }

    // 橋の前から戻るときのコルーチン
    IEnumerator ReturnFromBridgeCoroutine()
    {
        PlayLongFootsteps();

        FadeManager.Instance.FadeOutPanel(FadeManager.Instance.fadeInOutPanelTime);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        ShowPanel(Panel.Panel20);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        FadeManager.Instance.FadeInPanel();
    }

    void HideArrows()
    {
        rightArrow.SetActive(false);
        leftArrow.SetActive(false);
        backArrow.SetActive(false);
    }

    void ShowSideArrows()
    {
        rightArrow.SetActive(true);
        leftArrow.SetActive(true);
    }

    void ShowPanel(Panel nextPanel)
    {
        HideArrows();
        currentPanel = nextPanel;

        switch (nextPanel)
        {
            case Panel.Panel02:
                this.transform.localPosition = new Vector2(2000, 0);
                rightArrow.SetActive(true);
                break;
            case Panel.Panel01:
                this.transform.localPosition = new Vector2(1000, 0);
                ShowSideArrows();
                break;
            case Panel.Panel00:
                this.transform.localPosition = new Vector2(0, 0);
                ShowSideArrows();
                break;
            case Panel.Panel10:
                this.transform.localPosition = new Vector2(-1000, 0);
                ShowSideArrows();
                break;
            case Panel.Panel20:
                this.transform.localPosition = new Vector2(-2000, 0);
                leftArrow.SetActive(true);
                break;
            case Panel.Cave:
                this.transform.localPosition = new Vector2(-5000, 0);
                backArrow.SetActive(true);
                break;
            case Panel.Hill:
                this.transform.localPosition = new Vector2(5000, 0);
                backArrow.SetActive(true);
                break;
            case Panel.Bridge:
                this.transform.localPosition = new Vector2(-4000, 0);
                backArrow.SetActive(true);
                break;
        }
    }

    void ShowObject(Object nextObject)
    {
        PlayShortFootsteps();

        HideArrows();
        Transform myTransform = this.transform;
        Vector2 localPos = myTransform.localPosition;

        if (nextObject == Object.Object00)
        {
            localPos.y -= 2500.0f;
        }
        else if (nextObject == Object.Object01)
        {
            localPos.y -= 5000.0f;
        }
        else if (nextObject == Object.Object02)
        {
            localPos.y -= 7500.0f;
        }
        else if (nextObject == Object.Object03)
        {
            localPos.y -= 10000.0f;
        }
        else if (nextObject == Object.Object04)
        {
            localPos.y -= 12500.0f;
        }
        else if (nextObject == Object.Object05)
        {
            localPos.y -= 15000.0f;
        }

        myTransform.localPosition = localPos;
        backArrow.SetActive(true);
    }

    public void OnRightArrow()
    {
        IEnumerator coroutine = OnRightArrowCoroutine();
        StartCoroutine(coroutine);
    }

    public void OnLeftArrow()
    {
        IEnumerator coroutine = OnLeftArrowCoroutine();
        StartCoroutine(coroutine);
    }

    public void OnBackArrow()
    {
        // ズーム中であればズーム前のオブジェクト画面に戻る
        if (zoomToggle00 == true)
        {
            ZoomInOut00();
        }
        else if (zoomToggle01 == true)
        {
            ZoomInOut01();
        }
        // 洞窟の中、かつ最初のパネル画面ではフィールド画面に移動する
        else if (currentPanel == Panel.Cave && isObjectInCave == false)
        {
            IEnumerator coroutine = LeaveCaveCoroutine();
            StartCoroutine(coroutine);
        }
        // 丘の上、かつ最初のパネル画面では下に降りる
        else if (currentPanel == Panel.Hill && isObjectOnHill == false)
        {
            IEnumerator coroutine = ClimbDownCoroutine();
            StartCoroutine(coroutine);
        }
        // 橋の周辺、かつ最初のパネル画面では湖の前に戻る
        else if (currentPanel == Panel.Bridge && isObjectOnBridge == false)
        {
            IEnumerator coroutine = ReturnFromBridgeCoroutine();
            StartCoroutine(coroutine);
        }
        else
        {
            PlayShortFootsteps();

            if (currentPanel == Panel.Panel10)
            {
                Bird.Instance.DeleteFukidashi();
                ShowPanel(Panel.Panel10);
            }
            else
            {
                ShowPanel(currentPanel);
            }

            if (isObjectInCave == true)
            {
                isObjectInCave = false;
            }

            if (isObjectOnHill == true)
            {
                isObjectOnHill = false;
            }

            if (isObjectOnBridge == true)
            {
                isObjectOnBridge = false;
            }
        }

    }

    public void OnObject(int objectNumber)
    {
        // アイテムウィンドウが非表示のときだけ画面移動させる
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            switch (objectNumber)
            {
                case 0:
                    ShowObject(Object.Object00);
                    break;
                case 1:
                    ShowObject(Object.Object01);
                    break;
                case 2:
                    ShowObject(Object.Object02);
                    break;
                case 3:
                    ShowObject(Object.Object03);
                    break;
                case 4:
                    ShowObject(Object.Object04);
                    break;
                case 5:
                    ShowObject(Object.Object05);
                    break;
            }
            if (currentPanel == Panel.Cave)
            {
                isObjectInCave = true;
            }
            if (currentPanel == Panel.Hill)
            {
                isObjectOnHill = true;
            }
            if (currentPanel == Panel.Bridge)
            {
                isObjectOnBridge = true;
            }
        }
    }

    public void ZoomInOut00()
    {
        // アイテムウィンドウが非表示のときだけ画面移動させる
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            PlayShortFootsteps();

            // ズームインとズームアウトでy軸を反転させる
            Transform myTransform = this.transform;
            Vector2 localPos = myTransform.localPosition;
            localPos.y *= -1.0f;
            myTransform.localPosition = localPos;
            zoomToggle00 = !zoomToggle00;
        }
    }

    public void ZoomInOut01()
    {
        // アイテムウィンドウが非表示のときだけ画面移動させる
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            PlayShortFootsteps();

            // ズームインとズームアウトでy軸を反転させる
            Transform myTransform = this.transform;
            Vector2 localPos = myTransform.localPosition;
            if (zoomToggle01 == false)
            {
                localPos.y *= -5.0f;
            }
            else if (zoomToggle01 == true)
            {
                localPos.y /= -5.0f;
            }
            myTransform.localPosition = localPos;
            zoomToggle01 = !zoomToggle01;
        }
    }

    // 長い足音を再生する
    public void PlayLongFootsteps()
    {
        if (currentPanel == Panel.Cave)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.stone);
        }
        else
        {
            SEManager.Instance.PlaySE(SEManager.Instance.soil);
        }
    }

    // 短い足音を再生する
    public void PlayShortFootsteps()
    {
        if (currentPanel == Panel.Cave)
        {
            SEManager.Instance.PlaySE(SEManager.Instance.stoneShort);
        }
        else
        {
            SEManager.Instance.PlaySE(SEManager.Instance.soilShort);
        }
    }

    // ゴーレムが削岩したあとフィールド画面に戻るときの処理
    public IEnumerator ReturnToFieldCoroutine()
    {
        FadeManager.Instance.FadeOutPanel(FadeManager.Instance.fadeInOutPanelTime);

        yield return new WaitForSeconds(0.5f);

        Transform myTransform = this.transform;
        Vector2 localPos = myTransform.localPosition;
        localPos.y *= -1.0f;
        myTransform.localPosition = localPos;
        zoomToggle00 = !zoomToggle00;
        UIManager.Instance.ShowMainUI();

        yield return new WaitForSeconds(0.5f);

        FadeManager.Instance.FadeInPanel();
    }

    // 洞窟に入るときの処理
    public void EnterCave()
    {
        // アイテムウィンドウが非表示のときだけ画面移動させる
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            IEnumerator coroutine = EnterCaveCoroutine();
            StartCoroutine(coroutine);
        }
    }

    // スコップとオノのロックを解除したときにその前まで移動する処理
    public IEnumerator UnlockCoroutine(Item.Type item)
    {
        FadeManager.Instance.FadeOutPanel(FadeManager.Instance.fadeInOutPanelTime);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        this.transform.localPosition = new Vector2(-5000, -15000);
        // 燭台またはデバイスに移動している時点でisObjectInCaveはtrueなので以下は記述しなくても問題ないが、念のため
        isObjectInCave = true;

        // 燭台またはデバイスに移動したときにzoomToggleがtrueになっているのでfalseに戻す
        switch (item)
        {
            case Item.Type.Shovel:
                zoomToggle00 = !zoomToggle00;
                break;
            case Item.Type.Axe:
                zoomToggle01 = !zoomToggle01;
                break;
        }

        SEManager.Instance.PlaySE(SEManager.Instance.unlockMetal);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        FadeManager.Instance.FadeInPanel();

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime + 1.0f);

        // 落下後の背景を表示する
        switch (item)
        {
            case Item.Type.Shovel:
                Candle.Instance.ShowImageAfterDroppingShovel();
                break;
            case Item.Type.Axe:
                Device.Instance.ShowImageAfterDroppingAxe();
                break;
        }

        SEManager.Instance.PlaySE(SEManager.Instance.dropItem);

        yield return new WaitForSeconds(1.0f);

        // 燭台またはデバイスへの移動を制限する
        switch (item)
        {
            case Item.Type.Shovel:
                Candle.Instance.objectCandlestick.SetActive(false);
                break;
            case Item.Type.Axe:
                Device.Instance.objectDevice.SetActive(false);
                break;
        }

        UIManager.Instance.ShowMainUI();

        switch (item)
        {
            case Item.Type.Shovel:
                Candle.Instance.collisionShovel.SetActive(true);
                break;
            case Item.Type.Axe:
                Device.Instance.collisionAxe.SetActive(true);
                break;
        }
    }

    // ピーナッツ使用後に画面移動するときの処理
    public IEnumerator MoveToBirdsEatingCoroutine()
    {
        FadeManager.Instance.FadeOutPanel(FadeManager.Instance.fadeInOutPanelTime);

        yield return new WaitForSeconds(0.5f);

        Transform myTransform = this.transform;
        Vector2 localPos = myTransform.localPosition;
        localPos.y *= -1.0f;
        myTransform.localPosition = localPos;
        zoomToggle00 = !zoomToggle00;

        Bird.Instance.objectBackground01.SetActive(true);
        Bird.Instance.birdsFlying.SetActive(false);

        UIManager.Instance.ShowMainUI();

        yield return new WaitForSeconds(0.5f);

        FadeManager.Instance.FadeInPanel();
    }

    // ロープを切ったあとに画面移動するときのコルーチン
    public IEnumerator MoveAfterCuttingRopeCoroutine()
    {
        FadeManager.Instance.FadeOutPanel(FadeManager.Instance.fadeInOutPanelTime);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        ShowPanel(currentPanel);
        UIManager.Instance.ShowMainUI();

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        FadeManager.Instance.FadeInPanel();
    }

    // 橋をかけるときのコルーチン    
    public IEnumerator BuildBridgeCoroutine()
    {
        StoneHolder.Instance.panelBackgrounds[0].SetActive(true);

        FadeManager.Instance.FadeOutPanel(FadeManager.Instance.fadeInOutPanelTime);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        ShowPanel(currentPanel);
        isObjectOnBridge = false;

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        SEManager.Instance.PlaySE(SEManager.Instance.creak);

        FadeManager.Instance.FadeInPanel();

        yield return new WaitForSeconds(1.5f);

        SEManager.Instance.PlaySE(SEManager.Instance.buildBridge);

        StoneHolder.Instance.panelBackgrounds[1].SetActive(true);

        yield return new WaitForSeconds(1.0f);

        UIManager.Instance.ShowMainUI();

        StoneHolder.Instance.collisionAirPlane.SetActive(true);
    }

    // 網で木箱を引き上げたあとのコルーチン
    IEnumerator ReturnAfterCatchingCrateCoroutine()
    {
        FadeManager.Instance.FadeOutPanel(FadeManager.Instance.fadeInOutPanelTime);

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        Transform myTransform = this.transform;
        Vector2 localPos = myTransform.localPosition;
        localPos.y *= -1.0f;
        myTransform.localPosition = localPos;
        zoomToggle00 = !zoomToggle00;

        Crate.Instance.crateStoneKey.SetActive(true);
        Crate.Instance.crateStoneKeyCaught.SetActive(false);

        UIManager.Instance.ShowMainUI();

        yield return new WaitForSeconds(FadeManager.Instance.fadeInOutPanelTime);

        FadeManager.Instance.FadeInPanel();
    }

    // オノで木を切り倒したあと画面移動するときのコルーチン
    IEnumerator ReturnAfterCuttingDownTreeCoroutine()
    {
        FadeManager.Instance.FadeOutPanel(FadeManager.Instance.fadeInOutPanelTime);

        yield return new WaitForSeconds(0.5f);

        ShowPanel(currentPanel);
        UIManager.Instance.ShowMainUI();

        yield return new WaitForSeconds(0.5f);

        SEManager.Instance.PlaySE(SEManager.Instance.cutDownTree);

        FadeManager.Instance.FadeInPanel();
    }

    // 丘に登るときの処理
    public void ClimbHill()
    {
        // アイテムウィンドウが非表示のときだけ画面移動させる
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            IEnumerator coroutine = ClimbHillCoroutine();
            StartCoroutine(coroutine);
        }
    }

    // 望遠鏡を覗くときの処理
    public void OnFinder()
    {
        // アイテムウィンドウが非表示のときだけ画面移動させる
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            IEnumerator coroutine = OnFinderCoroutine();
            StartCoroutine(coroutine);
        }
    }

    // 網で木箱を引き上げたあとの処理
    public void ReturnAfterCatchingCrate()
    {
        IEnumerator coroutine = ReturnAfterCatchingCrateCoroutine();
        StartCoroutine(coroutine);
    }

    // オノで木を切り倒したあと画面移動するときの処理
    public void ReturnAfterCuttingDownTree()
    {
        IEnumerator coroutine = ReturnAfterCuttingDownTreeCoroutine();
        StartCoroutine(coroutine);
    }

    // 橋の前に移動するときの処理
    public void MoveToBridge()
    {
        // アイテムウィンドウが非表示のときだけ画面移動させる
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            IEnumerator coroutine = MoveToBridgeCoroutine();
            StartCoroutine(coroutine);
        }
    }
}
