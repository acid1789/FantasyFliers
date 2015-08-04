using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Startup : BaseLoader
{
    bool _loadComplete;

    // Use this for initialization
    IEnumerator Start()
    //void Start()
    {
        _loadComplete = false;

        List<string> loadedBundles = new List<string>();

        yield return StartCoroutine(Initialize());

        // Load UI
        loadedBundles.Add("UI_Main");
        yield return StartCoroutine(Load("ui_main", "UI_Countdown"));
        Globals.UI_Countdown = LastLoadedAsset;

        yield return StartCoroutine(Load("ui_main", "UI_Hud_TimeTrial"));
        Globals.UI_Hud_TimeTrial = LastLoadedAsset;
        
        // Setup flight controller
        Globals.FlightController = new RonsDroneScript();

        // Unload assetBundles.
        foreach (string bundle in loadedBundles)
        {
            AssetBundleManager.UnloadAssetBundle(bundle);
        }

        _loadComplete = true;
    }

    // Update is called once per frame
    new void Update()
    {
        if (_loadComplete)
        {
            if (Input.touchCount > 0 || Input.GetMouseButtonUp(0))
            {
                Application.LoadLevel("HubScene");
            }
        }
    }
}
