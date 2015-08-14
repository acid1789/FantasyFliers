using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Startup : BaseLoader
{
    bool _loadComplete;
    bool _signedIn;

    SignInUI _signIn;

    // Use this for initialization
    IEnumerator Start()
    {
        _signedIn = false;
        _loadComplete = false;

        List<string> loadedBundles = new List<string>();

        yield return StartCoroutine(Initialize());

        // Start the network
        Globals.Network = new NetworkManager("192.168.1.200", 1255);

        // Load UI
        loadedBundles.Add("UI_Main");
        yield return StartCoroutine(Load("ui_main", "UI_Countdown"));
        Globals.UI_Countdown = LastLoadedAsset;

        yield return StartCoroutine(Load("ui_main", "UI_Hud_TimeTrial"));
        Globals.UI_Hud_TimeTrial = LastLoadedAsset;

        yield return StartCoroutine(Load("ui_main", "UI_ScoreScreen"));
        Globals.UI_ScoreScreen = LastLoadedAsset;

        yield return StartCoroutine(Load("ui_main", "UI_SignIn"));
        Globals.UI_SignIn = LastLoadedAsset;

        // Load Item Template map
        yield return StartCoroutine(LoadTextFile("item_templates", "TemplateMap"));
        Globals.Inventory = new Inventory(LastLoadedText);
        
        // Setup flight controller
        Globals.FlightController = new ArcadeFlightController();
        Globals.FlightController.SetAxisInput(new DualVirtualJoysticks());

        loadedBundles.Add("UI_FLIGHT");
        yield return StartCoroutine(Load("ui_flight", "UI_DualVirtualJoysticks"));
        Globals.FlightController.GetAxisInput().SetUIPrefab(LastLoadedAsset);

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
            if (!_signedIn)
            {
                DoSignIn();
            }
            else if (Input.touchCount > 0 || Input.GetMouseButtonUp(0))
            {
                Application.LoadLevel("HubScene");
            }
        }
    }

    void DoSignIn()
    {
        if (_signIn == null)
        {
            GameObject ui = (GameObject)Instantiate(Globals.UI_SignIn);
            _signIn = ui.transform.GetComponent<SignInUI>();
        }
        else
        {
            if (Globals.Network.Online == NetworkManager.SignInStatus.SignedIn)
            {
                // Kill the sign in and move on
                DestroyObject(_signIn.gameObject);
                _signIn = null;
                _signedIn = true;
            }
        }
    }
}
