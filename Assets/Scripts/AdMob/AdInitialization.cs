﻿using UnityEngine;
using GoogleMobileAds.Api;

public class AdInitialization : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });
    }
}
