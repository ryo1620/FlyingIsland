using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bird : SingletonMonoBehaviour<Bird>
{
    // 鳥の吹き出し画像を取得する
    public GameObject fukidashi00;
    public GameObject fukidashi01;
    public GameObject fukidashi02;

    // フェードに使う画像    
    Image[] fukidashiImages;

    // フェードにかかる時間
    public readonly float fadeInImageTime = 0.12f;

    // フェードに使った時間
    float fadeDeltaTime = 0.0f;

    // ピーナッツ使用時に表示する画像を取得する
    public GameObject birdStanding;
    public GameObject birdHappy;
    public GameObject birdsFlying;

    // ピーナッツ使用後に背景を変更するため、画像を取得する
    public GameObject panelBackground00;
    public GameObject panelBackground01;
    public GameObject objectBackground00;
    public GameObject objectBackground01;

    void Start()
    {
        fukidashiImages = new Image[] { fukidashi00.GetComponent<Image>(), fukidashi01.GetComponent<Image>(), fukidashi02.GetComponent<Image>() };

        LoadImage();
    }

    IEnumerator FadeInFukidashiCoroutine()
    {
        foreach (Image fukidashiImage in fukidashiImages)
        {
            // 色の不透明度
            float alpha = 0;
            // Imageの色変更に使う
            Color color = new Color(255, 255, 255, alpha);
            // 初期化
            this.fadeDeltaTime = 0;
            // 色の初期化
            fukidashiImage.color = color;

            do
            {
                // 次フレームで再開する
                yield return null;
                // 時間を加算する
                this.fadeDeltaTime += Time.unscaledDeltaTime;
                // 透明度を決める
                alpha = this.fadeDeltaTime / this.fadeInImageTime;

                if (alpha > 1)
                {
                    // alphaの値を制限する
                    alpha = 1;
                }

                // 色の透明度を決める
                color.a = alpha;
                // 色を代入する
                fukidashiImage.color = color;
            }
            while (this.fadeDeltaTime <= this.fadeInImageTime);

            yield return new WaitForSeconds(fadeInImageTime);
        }

    }

    public void OnSingleBird()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            // ピーナッツが選択されているかどうかで処理を分ける            
            if (ItemBoxManager.Instance.IsSelected(Item.Type.Peanuts))
            {
                UIManager.Instance.HideMainUI();

                // 木箱（矢）の仕掛けを解けるようにする
                GimmickFlag.Instance.canSolveCrateArrow = true;
                SaveManager.Instance.SetCanSolveGimmickFlag(Gimmick.Type.CrateArrow, true);

                // 状態をセーブする
                SaveManager.Instance.SetUsedItemFlag(Item.Type.Peanuts, true);

                ItemBoxManager.Instance.DeleteItem(Item.Type.Peanuts);

                // 表示させるヒントを切り替える
                HintManager.Instance.SetHintText();

                birdStanding.SetActive(true);
                objectBackground00.SetActive(false);

                ChangePanelBackground();

                // コルーチンを使って画像を順次表示させる
                StartCoroutine(this.DelayCoroutine(1.0f, () =>
                {
                    SEManager.Instance.PlaySE(SEManager.Instance.singingBird);
                    birdHappy.SetActive(true);
                    birdStanding.SetActive(false);

                    StartCoroutine(this.DelayCoroutine(1.0f, () =>
                    {
                        SEManager.Instance.PlaySE(SEManager.Instance.singingBirds);
                        SEManager.Instance.PlaySE(SEManager.Instance.flap);
                        birdsFlying.SetActive(true);
                        birdHappy.SetActive(false);

                        // フェードイン・アウトでピーナッツを食べる鳥たちの画面に移動する
                        StartCoroutine(this.DelayCoroutine(1.0f, () =>
                        {
                            IEnumerator coroutine = PanelChanger.Instance.MoveToBirdsEatingCoroutine();
                            StartCoroutine(coroutine);
                        }));
                    }));
                }));
            }
            else
            {
                DeleteFukidashi();
                SEManager.Instance.PlaySE(SEManager.Instance.singingBird);

                IEnumerator coroutine = FadeInFukidashiCoroutine();
                StartCoroutine(coroutine);
            }

        }
    }

    // エリア移動時と鳥をタップしたときに吹き出し画像を消去（透明に）するための関数
    public void DeleteFukidashi()
    {
        foreach (Image fukidashiImage in fukidashiImages)
        {
            // 色を透明にする
            fukidashiImage.color = new Color(255, 255, 255, 0);
        }
    }

    void LoadImage()
    {
        bool usedItem = SaveManager.Instance.GetUsedItemFlag(Item.Type.Peanuts);

        if (usedItem == true)
        {
            ChangePanelBackground();
            ChangeObjectBackground();
        }
    }

    // 背景10_0を10_1に切り替える
    void ChangePanelBackground()
    {
        panelBackground01.SetActive(true);
        panelBackground00.SetActive(false);
    }

    // 背景10b_0を10b_1に切り替える
    void ChangeObjectBackground()
    {
        objectBackground01.SetActive(true);
        objectBackground00.SetActive(false);
    }
}
