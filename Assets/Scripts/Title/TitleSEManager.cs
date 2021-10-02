using UnityEngine;

// シングルトン化する
public class TitleSEManager : SingletonMonoBehaviour<TitleSEManager>
{
    // SEのAudioSource用の変数を宣言する
    AudioSource[] audioSources;
    AudioSource select;
    AudioSource cancel;
    AudioSource start;

    // Start is called before the first frame update
    void Start()
    {
        audioSources = GetComponents<AudioSource>();
        select = audioSources[0];
        cancel = audioSources[1];
        start = audioSources[2];

        // 設定がオフであればSEを停止する
        // 1がオン、0がオフ
        int seSetting = PlayerPrefs.GetInt("SE", 1);

        if (seSetting == 0)
        {
            StopSE();
        }
    }

    // すべてのSEをオンにする
    public void PlaySE()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].mute = false;
        }
    }

    // すべてのSEをオフにする
    public void StopSE()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].mute = true;
        }
    }

    public void SoundSelect()
    {
        select.PlayOneShot(select.clip);
    }

    public void SoundCancel()
    {
        cancel.PlayOneShot(cancel.clip);
    }

    public void SoundStart()
    {
        start.PlayOneShot(start.clip);
    }
}
