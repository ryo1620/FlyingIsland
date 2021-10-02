using UnityEngine;

public class BgmSettings : MonoBehaviour
{
    // BGMを再生・停止するためにBgmのゲームオブジェクトを取得する
    public GameObject bgm;
    // BgmのAudioSource用の変数を宣言する
    AudioSource bgmAudioSource;

    // BGM（風音）を再生・停止するためにBgmのゲームオブジェクトを取得する
    public GameObject wind;
    // Bgm（風音）のAudioSource用の変数を宣言する
    AudioSource windAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        bgmAudioSource = bgm.GetComponent<AudioSource>();
        windAudioSource = wind.GetComponent<AudioSource>();

        // 設定がオンであればBGMを再生する
        // 1がオン、0がオフ
        int bgmSetting = PlayerPrefs.GetInt("BGM", 1);

        if (bgmSetting == 1)
        {
            bgmAudioSource.Play();
            windAudioSource.Play();
        }
    }
}
