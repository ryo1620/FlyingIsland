using UnityEngine;
using GoogleMobileAds.Api;

public class AdInitialization : MonoBehaviour
{
    // ゲーム起動後Awakeより前に一度だけ呼び出す
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitializeAds()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });
    }
}
