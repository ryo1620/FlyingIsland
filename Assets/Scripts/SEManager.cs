using UnityEngine;

// シングルトン化する
public class SEManager : SingletonMonoBehaviour<SEManager>
{
    // SEのAudioSource用の変数を宣言する
    AudioSource[] audioSources;
    [System.NonSerialized] public AudioSource soil;
    [System.NonSerialized] public AudioSource stone;
    [System.NonSerialized] public AudioSource ladder;
    [System.NonSerialized] public AudioSource select;
    [System.NonSerialized] public AudioSource cancel;
    [System.NonSerialized] public AudioSource start;
    [System.NonSerialized] public AudioSource correct;
    [System.NonSerialized] public AudioSource incorrect;
    [System.NonSerialized] public AudioSource getItem;
    [System.NonSerialized] public AudioSource good;
    [System.NonSerialized] public AudioSource singingBird;
    [System.NonSerialized] public AudioSource singingBirds;
    [System.NonSerialized] public AudioSource soilShort;
    [System.NonSerialized] public AudioSource push;
    [System.NonSerialized] public AudioSource moveRock;
    [System.NonSerialized] public AudioSource tap;
    [System.NonSerialized] public AudioSource moveStone;
    [System.NonSerialized] public AudioSource open;
    [System.NonSerialized] public AudioSource golemStart;
    [System.NonSerialized] public AudioSource pickaxe;
    [System.NonSerialized] public AudioSource stoneShort;
    [System.NonSerialized] public AudioSource ignition;
    [System.NonSerialized] public AudioSource unlockMetal;
    [System.NonSerialized] public AudioSource unlockKey;
    [System.NonSerialized] public AudioSource dropItem;
    [System.NonSerialized] public AudioSource dig;
    [System.NonSerialized] public AudioSource shoot;
    [System.NonSerialized] public AudioSource climb;
    [System.NonSerialized] public AudioSource land;
    [System.NonSerialized] public AudioSource cat;
    [System.NonSerialized] public AudioSource openCap;
    [System.NonSerialized] public AudioSource water;
    [System.NonSerialized] public AudioSource insertStoneKey;
    [System.NonSerialized] public AudioSource chop;
    [System.NonSerialized] public AudioSource cutDownTree;
    [System.NonSerialized] public AudioSource flap;
    [System.NonSerialized] public AudioSource buildBridge;
    [System.NonSerialized] public AudioSource creak;

    // PlayerPrefsで使用するキー
    const string SAVE_SE_KEY = "SE";

    void Start()
    {
        audioSources = GetComponents<AudioSource>();
        soil = audioSources[0];
        stone = audioSources[1];
        ladder = audioSources[2];
        select = audioSources[3];
        cancel = audioSources[4];
        start = audioSources[5];
        correct = audioSources[6];
        incorrect = audioSources[7];
        getItem = audioSources[8];
        good = audioSources[9];
        singingBird = audioSources[10];
        singingBirds = audioSources[11];
        soilShort = audioSources[12];
        push = audioSources[13];
        moveRock = audioSources[14];
        tap = audioSources[15];
        moveStone = audioSources[16];
        open = audioSources[17];
        golemStart = audioSources[18];
        pickaxe = audioSources[19];
        stoneShort = audioSources[20];
        ignition = audioSources[21];
        unlockMetal = audioSources[22];
        unlockKey = audioSources[23];
        dropItem = audioSources[24];
        dig = audioSources[25];
        shoot = audioSources[26];
        climb = audioSources[27];
        land = audioSources[28];
        cat = audioSources[29];
        openCap = audioSources[30];
        water = audioSources[31];
        insertStoneKey = audioSources[32];
        chop = audioSources[33];
        cutDownTree = audioSources[34];
        flap = audioSources[35];
        buildBridge = audioSources[36];
        creak = audioSources[37];

        // 設定がオフであればSEを停止する
        // 1がオン、0がオフ
        int seSetting = PlayerPrefs.GetInt(SAVE_SE_KEY, 1);

        if (seSetting == 0)
        {
            TurnOffSE();
        }
    }

    // すべてのSEをオンにする
    public void TurnOnSE()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].mute = false;
        }
    }

    // すべてのSEをオフにする
    public void TurnOffSE()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].mute = true;
        }
    }

    // それぞれのSEを鳴らす関数    
    public void PlaySE(AudioSource se)
    {
        se.PlayOneShot(se.clip);
    }

    // 猫をタップしたときに鳴き声を再生する
    public void OnCat()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            PlaySE(cat);
        }

    }

    // ゴーレムをタップしたときに起動音を再生する
    public void OnGolem()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            PlaySE(golemStart);
        }
    }

    // 鳥をタップしたときに鳴き声を再生する
    public void OnBird()
    {
        // アイテムウィンドウが非表示のときだけ処理を行う
        if (UIManager.Instance.itemWindowIsShown == false)
        {
            PlaySE(singingBird);
        }
    }
}
